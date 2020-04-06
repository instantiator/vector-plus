using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Anki.Vector;
using Anki.Vector.Events;
using Anki.Vector.Objects;
using static VectorPlusLib.IVectorActionPlus;
using static VectorPlusLib.IVectorControllerPlus;

namespace VectorPlusLib
{
    public class VectorControllerPlus : IAsyncDisposable, IVectorControllerPlus
    {
        private RobotConfiguration robotConfig;
        private VectorControllerPlusConfig controllerConfig;
        private ConnectedState connection;
        private bool shouldHaveControlForCurrentAction;
        private bool actionExecutorRunning;
        private bool mainLoopRunning;

        private readonly TimeSpan objectGoneThreshold = TimeSpan.FromSeconds(2);

        private Task task_connection;
        private Task task_actions;

        private List<IVectorBehaviourPlus> behaviours;
        private Queue<IVectorActionPlus> actions;
        private List<VectorBehaviourPlusReport> reports;
        Dictionary<int, ObjectSeenState> objectSeenStates;

        public event Func<ConnectedState, Task> OnConnectionChanged;
        public event Func<IVectorActionPlus, ActionEvent, Task> OnActionEvent;
        public event Func<IVectorBehaviourPlus, BehaviourEvent, Task> OnBehaviourEvent;
        public event Func<VectorBehaviourPlusReport, Task> OnBehaviourReport;
        public event Func<ObjectSeenState, Task> OnObjectAppeared;
        public event Func<ObjectSeenState, Task> OnObjectDisappeared;

        public VectorControllerPlus(VectorControllerPlusConfig controllerConfig, RobotConfiguration robotConfig = null)
        {
            this.controllerConfig = controllerConfig;
            this.robotConfig = robotConfig;
            this.connection = ConnectedState.Disconnected;
            this.behaviours = new List<IVectorBehaviourPlus>();
            this.actions = new Queue<IVectorActionPlus>();
            this.reports = new List<VectorBehaviourPlusReport>();
            this.objectSeenStates = new Dictionary<int, ObjectSeenState>();
            OnConnectionChanged += async (state) => await RespondToConnectionAsync(state);
        }

        public Robot Robot { get; private set; }

        public IEnumerable<VectorBehaviourPlusReport> BehaviourReports { get { return reports; } }

        public ConnectedState Connection
        {
            get { return connection; }
            private set
            {
                connection = value;
                OnConnectionChanged?.Invoke(connection);
            }
        }

        public async Task StartMainLoopAsync(bool stopOnKeypress = true)
        {
            mainLoopRunning = true;
            while (mainLoopRunning && (!Console.KeyAvailable || !stopOnKeypress))
            {
                await Task.Delay(100);
                if (AnyBehavioursNeedPermanentObjectMonitoring)
                {
                    UpdateObjectMonitoring();
                }
            }
        }

        public void StopMainLoop() { mainLoopRunning = false; }

        public async Task ConnectAsync()
        {
            if (Robot == null || !Robot.IsConnected)
            {
                Connection = ConnectedState.Connecting;
                Robot = await (robotConfig == null ? Robot.NewConnection() : Robot.NewConnection(robotConfig));
                Robot.Disconnected += OnDisconnect;
                Connection = ConnectedState.Connected;
            }
        }

        private async Task RespondToConnectionAsync(ConnectedState state)
        {
            switch (state)
            {
                case ConnectedState.Connected:
                    await UnregisterBehavioursAsync(behaviours);
                    await RegisterBehavioursAsync(behaviours);
                    await ActOnAnyBehaviourPermanentRequirements();
                    ResumeActionExecutor();
                    break;
                case ConnectedState.Disconnected:
                    HaltActionExecutor();
                    break; // already lost control of the robot?
            }

        }

        private void OnDisconnect(object src, DisconnectedEventArgs args)
        {
            if (Connection != ConnectedState.Connecting && Connection != ConnectedState.Disconnecting)
            {
                task_connection = Task.Run(async delegate
                {
                    Connection = ConnectedState.Connecting;
                    await Task.Delay(controllerConfig.ReconnectDelay_ms);
                    if (Connection == ConnectedState.Connecting) { await ConnectAsync(); }
                });

                // just jam up for now... later, do this quietly in the background
                // TODO: can throw an aggregate exception -- deal with that later
                task_connection.Wait();
            }
        }

        public async Task DisconnectAsync()
        {
            if (Robot != null && Robot.IsConnected)
            {
                Connection = ConnectedState.Disconnecting;
                Robot.Disconnected -= OnDisconnect;
                await UnregisterBehavioursAsync(behaviours);
                if (Robot.IsConnected) { await Robot.Disconnect(); }
                Robot.Dispose();
                Robot = null;
                Connection = ConnectedState.Disconnected;
            }
        }

        public async ValueTask DisposeAsync()
        {
            await DisconnectAsync();
        }

        public async Task AddBehaviourAsync(IVectorBehaviourPlus behaviour)
        {
            if (!behaviours.Any(b => b.Id == behaviour.Id))
            {
                Console.WriteLine("+ Adding behaviour: " + behaviour.GetType().Name);
                behaviours.Add(behaviour);

                if (Connection == ConnectedState.Connected)
                {
                    await ActOnAnyBehaviourPermanentRequirements();
                    await RegisterBehavioursAsync(new[] { behaviour });
                }
            }
        }

        private async Task ActOnAnyBehaviourPermanentRequirements()
        {
            if (AnyBehavioursNeedPermanentRobotControl)
            {
                await TakeControlForActionAsync();
            }
            else
            {
                await ReleaseControlForActionAsync();
            }

            if (AnyBehavioursNeedPermanentObjectMonitoring)
            {
                Robot.Events.ObservedObject += Events_ObservedObject;
            }
            else
            {
                Robot.Events.ObservedObject -= Events_ObservedObject;
            }
        }

        private void Events_ObservedObject(object sender, RobotObservedObjectEventArgs e)
        {
            if (e.ObjectEventType == ObjectEventType.RobotObservedObject)
            {
                if (!objectSeenStates.ContainsKey(e.ObjectId))
                {
                    objectSeenStates.Add(e.ObjectId, new ObjectSeenState()
                    {
                        ObjectId = e.ObjectId,
                        ObjectType = e.ObjectType,
                        BelievedPresent = false
                    });
                }

                objectSeenStates[e.ObjectId].LastSeen = DateTime.Now;
                if (objectSeenStates[e.ObjectId].BelievedPresent == false)
                {
                    Console.WriteLine("+ Object appeared: " + objectSeenStates[e.ObjectId].ObjectType.ToString());
                    objectSeenStates[e.ObjectId].BelievedPresent = true;
                    OnObjectAppeared?.Invoke(objectSeenStates[e.ObjectId]);
                }
            }
        }

        private void UpdateObjectMonitoring()
        {
            foreach (var seenState in objectSeenStates)
            {
                if (seenState.Value.BelievedPresent)
                {
                    var diff = DateTime.Now - seenState.Value.LastSeen;
                    if (diff.Ticks > objectGoneThreshold.Ticks)
                    {
                        Console.WriteLine("- Object disappeared: " + seenState.Value.ObjectType.ToString());
                        seenState.Value.BelievedPresent = false;
                        OnObjectDisappeared?.Invoke(seenState.Value);
                    }

                }
            }
        }

        public async Task RemoveBehaviourAsync(IVectorBehaviourPlus behaviour)
        {
            Console.WriteLine("- Removing behaviour: " + behaviour.GetType().Name);
            var found = behaviours.Where(b => b.Id == behaviour.Id);
            behaviours.RemoveAll(b => b.Id == behaviour.Id);
            if (Connection == ConnectedState.Connected)
            {
                await ActOnAnyBehaviourPermanentRequirements();
                await UnregisterBehavioursAsync(found);
            }
        }

        private async Task RegisterBehavioursAsync(IEnumerable<IVectorBehaviourPlus> behaviours)
        {
            foreach (var behaviour in behaviours)
            {
                Console.WriteLine(". Registering behaviour: " + behaviour.GetType().Name);
                await behaviour.SetControllerAsync(this);
            }
        }

        private bool AnyBehavioursNeedPermanentObjectMonitoring
        {
            get
            {
                return behaviours.Any(b => b.NeedsPermanentObjectAppearanceMonitoring);
            }
        }

        private bool AnyBehavioursNeedPermanentRobotControl
        {
            get
            {
                return behaviours.Any(b => b.NeedsPermanentRobotControl);
            }
        }

        private async Task UnregisterBehavioursAsync(IEnumerable<IVectorBehaviourPlus> behaviours)
        {
            foreach (var behaviour in behaviours)
            {
                Console.WriteLine(". Unregistering behaviour: " + behaviour.GetType().Name);
                await behaviour.SetControllerAsync(null);
            }
            if (!AnyBehavioursNeedPermanentRobotControl && !shouldHaveControlForCurrentAction)
            {
                await ReleaseControlForActionAsync();
            }
        }

        public async Task TakeControlForActionAsync()
        {
            shouldHaveControlForCurrentAction = true;
            if (!Robot.Control.HasControl)
            {
                await Robot.Control.RequestControl();
            }
        }

        public async Task ReleaseControlForActionAsync()
        {
            shouldHaveControlForCurrentAction = false;
            if (Robot.Control.HasControl)
            {
                await Robot.Control.ReleaseControl();
            }
        }

        public async Task ReportAsync(VectorBehaviourPlusReport report)
        {
            reports.Add(report);
            Console.WriteLine("< Report from " + report.Behaviour.GetType().Name + ": " + report.Description);
            OnBehaviourReport?.Invoke(report);
        }

        public void EnqueueAction(IVectorActionPlus action)
        {
            actions.Enqueue(action);
            OnActionEvent?.Invoke(action, ActionEvent.Add);
            ResumeActionExecutor();
        }

        private void ResumeActionExecutor()
        {
            if (!actionExecutorRunning)
            {
                actionExecutorRunning = true;
                task_actions = ActionExecutor();
            }
        }

        private async Task ActionExecutor()
        {
            IVectorActionPlus action, nextAction;
            while (actionExecutorRunning && actions.TryDequeue(out action))
            {
                Console.WriteLine("> Running action: " + action.GetType().Name);
                if (action.NeedsControl)
                {
                    await TakeControlForActionAsync();
                }

                ActionState result = await action.ExecuteAsync(this);
                Console.WriteLine("< Action result: " + result.ToString());
                // TODO: handle action state properly - manage retries etc.

                bool ok = actions.TryPeek(out nextAction);
                if (ok)
                {
                    if (shouldHaveControlForCurrentAction && !nextAction.NeedsControl && !AnyBehavioursNeedPermanentRobotControl)
                    {
                        await ReleaseControlForActionAsync();
                    }
                }
                else if (!AnyBehavioursNeedPermanentRobotControl)
                {
                    await ReleaseControlForActionAsync();
                }
            }
            actionExecutorRunning = false;
        }

        private void HaltActionExecutor()
        {
            actionExecutorRunning = false;
        }

    }
}
