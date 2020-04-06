using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Anki.Vector;
using Anki.Vector.Events;
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

        private Task task_connection;
        private Task task_actions;

        private List<IVectorBehaviourPlus> behaviours;
        private Queue<IVectorActionPlus> actions;
        private List<VectorBehaviourPlusReport> reports;

        public event Func<ConnectedState, Task> OnConnectionChanged;
        public event Func<IVectorActionPlus, ActionEvent, Task> OnActionEvent;
        public event Func<IVectorBehaviourPlus, BehaviourEvent, Task> OnBehaviourEvent;
        public event Func<VectorBehaviourPlusReport, Task> OnBehaviourReport;

        public VectorControllerPlus(VectorControllerPlusConfig controllerConfig, RobotConfiguration robotConfig = null)
        {
            this.controllerConfig = controllerConfig;
            this.robotConfig = robotConfig;
            this.connection = ConnectedState.Disconnected;
            this.behaviours = new List<IVectorBehaviourPlus>();
            this.actions = new Queue<IVectorActionPlus>();
            this.reports = new List<VectorBehaviourPlusReport>();
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
                    await RegisterBehavioursAsync(new[] { behaviour });
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
                await UnregisterBehavioursAsync(found);
            }
        }

        private async Task RegisterBehavioursAsync(IEnumerable<IVectorBehaviourPlus> behaviours)
        {
            if (AnyBehavioursNeedPermanentRobotControl)
            {
                await TakeControlForActionAsync();
            }
            foreach (var behaviour in behaviours)
            {
                Console.WriteLine(". Registering behaviour: " + behaviour.GetType().Name);
                await behaviour.SetControllerAsync(this);
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
