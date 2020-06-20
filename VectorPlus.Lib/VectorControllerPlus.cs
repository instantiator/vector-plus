using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Anki.Vector;
using Anki.Vector.Events;
using Anki.Vector.Types;
using VectorPlus.Lib.Vision;
using static VectorPlus.Lib.IVectorActionPlus;
using static VectorPlus.Lib.IVectorControllerPlus;

namespace VectorPlus.Lib
{
    public class VectorControllerPlus : IAsyncDisposable, IVectorControllerPlus
    {
        private RobotConfiguration robotConfig;
        private VectorControllerPlusConfig controllerConfig;
        private ConnectedState connection;
        private bool shouldHaveControlForCurrentAction;
        private bool actionExecutorRunning;
        private bool mainLoopRunning;

        // tasks
        private Task task_connection;
        private Task task_actions;

        // lists
        public List<IVectorBehaviourPlus> Behaviours { get; private set; }
        public Queue<IVectorActionPlus> Actions { get; private set; }
        public List<VectorBehaviourPlusReport> Reports { get; private set; }
        public List<ICameraFrameProcessor> FrameProcessors { get; private set; }

        // object monitoring
        private readonly TimeSpan objectGoneThreshold = TimeSpan.FromSeconds(5);
        private Dictionary<int, ObjectSeenState> objectSeenStates;

        // camera frame processing
        private ImageEncoding recentEncoding;
        private byte[] recentImage;

        public string LastConnectionError { get; private set; }
        public Exception LastConnectionException { get; private set; }

        public event Action<char> OnKeyPress;
        public event Func<ConnectedState, Task> OnConnectionChanged;
        public event Func<IVectorActionPlus, ActionEvent, Task> OnActionEvent;
        public event Func<List<IVectorBehaviourPlus>, BehaviourEvent, Task> OnBehaviourEvent;
        public event Func<VectorBehaviourPlusReport, Task> OnBehaviourReport;
        public event Func<ObjectSeenState, Task> OnObjectAppeared;
        public event Func<ObjectSeenState, Task> OnObjectDisappeared;
        public event Func<CameraFrameProcessingResult, Task> OnCameraFrameProcessingResult;

        public VectorControllerPlus()
        {
            this.connection = ConnectedState.Disconnected;
            this.Behaviours = new List<IVectorBehaviourPlus>();
            this.Actions = new Queue<IVectorActionPlus>();
            this.Reports = new List<VectorBehaviourPlusReport>();
            this.objectSeenStates = new Dictionary<int, ObjectSeenState>();
            this.FrameProcessors = new List<ICameraFrameProcessor>();
            OnConnectionChanged += async (state) => await RespondToConnectionAsync(state);
        }

        public Robot Robot { get; private set; }

        public IEnumerable<VectorBehaviourPlusReport> BehaviourReports { get { return Reports; } }

        public ConnectedState Connection
        {
            get { return connection; }
            private set
            {
                connection = value;
                OnConnectionChanged?.Invoke(connection);
            }
        }

        public bool MainLoopRunning => mainLoopRunning;

        public async Task StartMainLoopAsync(CancellationToken cancel, char? haltOn = ' ')
        {
            mainLoopRunning = true;
            var examineConsole = true;
            char? lastKeyPress = null;
            while (!cancel.IsCancellationRequested && mainLoopRunning && (haltOn == null || lastKeyPress != haltOn))
            {
                await Task.Delay(100);

                try
                {
                    if (examineConsole)
                    {
                        lastKeyPress = Console.KeyAvailable ? Console.ReadKey().KeyChar : (char?)null;
                        if (lastKeyPress.HasValue)
                        {
                            OnKeyPress?.Invoke(lastKeyPress.Value);
                            await OfferKeypressToBehavioursAsync(lastKeyPress.Value);
                        }
                    }
                }
                catch (InvalidOperationException e)
                {
                    await ReportAsync(VectorBehaviourPlusReport.FromException(e));
                    examineConsole = false;
                }

                if (AnyBehavioursNeedCameraProcessing)
                {
                    ProcessCameraFrame();
                }

                if (AnyBehavioursNeedPermanentObjectMonitoring)
                {
                    UpdateObjectMonitoring();
                }
            }
        }

        private void ProcessCameraFrame()
        {
            if (recentImage != null && FrameProcessors.Count > 0)
            {
                foreach (var processor in FrameProcessors)
                {
                    var result = processor.Process(recentImage);
                    OnCameraFrameProcessingResult?.Invoke(result);
                }
            }
        }

        public bool IsConnected {  get { return Connection == ConnectedState.Connected; } }

        private async Task OfferKeypressToBehavioursAsync(char c)
        {
            foreach (var behaviour in Behaviours)
            {
                await behaviour.ReceiveKeypressAsync(c);
            }
        }

        public void StopMainLoop() { mainLoopRunning = false; }

        /// <summary>
        /// Attempts to connect to the Robot.
        /// </summary>
        /// <param name="controllerConfig"></param>
        /// <param name="robotConfig"></param>
        /// <returns>True if the connection initially succeeded.</returns>
        public async Task<bool> ConnectAsync(VectorControllerPlusConfig controllerConfig, RobotConfiguration robotConfig = null)
        {
            this.controllerConfig = controllerConfig;
            this.robotConfig = robotConfig;

            if (Robot == null || !Robot.IsConnected)
            {
                try
                {
                    Connection = ConnectedState.Connecting;
                    Robot = await (robotConfig == null ? Robot.NewConnection() : Robot.NewConnection(robotConfig));
                    Robot.Disconnected += OnDisconnect;
                    Connection = ConnectedState.Connected;
                    LastConnectionError = null;
                    LastConnectionException = null;
                    return true;
                }
                catch (Exception e)
                {
                    Connection = ConnectedState.Disconnected;
                    LastConnectionError = e.Message;
                    LastConnectionException = e;
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        private async Task RespondToConnectionAsync(ConnectedState state)
        {
            switch (state)
            {
                case ConnectedState.Connected:
                    await UnregisterBehavioursAsync(Behaviours);
                    await RegisterBehavioursAsync(Behaviours);
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
                // reconnect
                task_connection = Task.Run(async delegate
                {
                    Connection = ConnectedState.Connecting;
                    await Task.Delay(controllerConfig.ReconnectDelay_ms);
                    do
                    {
                        if (Connection == ConnectedState.Connecting) { await ConnectAsync(controllerConfig, robotConfig); }
                        if (Connection == ConnectedState.Connecting) { await Task.Delay(controllerConfig.ReconnectDelay_ms); }
                    }
                    while (Connection == ConnectedState.Connecting);
                });

                // it might be better to spin off this thread and let it run
                // TODO: this can throw an AggregateException
                task_connection.Wait();
            }
        }

        public async Task DisconnectAsync()
        {
            if (Robot != null && Robot.IsConnected)
            {
                Connection = ConnectedState.Disconnecting;
                Robot.Disconnected -= OnDisconnect;
                await UnregisterBehavioursAsync(Behaviours);
                if (Robot.IsConnected) { await Robot.Disconnect(); }
                Robot.Dispose();
                Robot = null;
                Connection = ConnectedState.Disconnected;
            }
        }

        public async ValueTask DisposeAsync()
        {
            await DisconnectAsync();
            Behaviours.ToList().ForEach(async b => await RemoveBehaviourAsync(b.UniqueReference));
        }

        public async Task AddBehaviourAsync(IVectorBehaviourPlus behaviour)
        {
            if (!Behaviours.Any(b => b.UniqueReference == behaviour.UniqueReference))
            {
                Behaviours.Add(behaviour);
                UpdateFrameProcessors();

                if (Connection == ConnectedState.Connected)
                {
                    await ActOnAnyBehaviourPermanentRequirements();
                    await RegisterBehavioursAsync(new[] { behaviour });
                }

                OnBehaviourEvent?.Invoke(Behaviours, BehaviourEvent.Add);
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

            if (AnyBehavioursNeedCameraProcessing)
            {
                UpdateFrameProcessors();
                Robot.Camera.ImageReceived += Camera_ImageReceived;
                if (!Robot.Camera.IsFeedActive)
                {
                    await Robot.Camera.StartFeed();
                }
            }
            else
            {
                UpdateFrameProcessors();
                Robot.Camera.ImageReceived -= Camera_ImageReceived;
                if (Robot.Camera.IsFeedActive)
                {
                    await Robot.Camera.StopFeed();
                }
            }
        }

        private void Camera_ImageReceived(object sender, Anki.Vector.Events.ImageReceivedEventArgs e)
        {
            recentEncoding = e.ImageEncoding;
            recentImage = e.ImageData;
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
                    objectSeenStates[e.ObjectId].BelievedPresent = true;
                    OnObjectAppeared?.Invoke(objectSeenStates[e.ObjectId]);
                }
            }
        }

        private void UpdateFrameProcessors()
        {
            var requiredTypes = Behaviours.SelectMany(b => b.RequestedFrameProcessors).Distinct();
            var presentTypes = FrameProcessors.Select(p => p.GetType()).Distinct();

            var toRemove = presentTypes.Where(t => !requiredTypes.Contains(t)).ToList();
            var toAdd = requiredTypes.Where(t => !presentTypes.Contains(t));
            
            FrameProcessors.Where(p => toRemove.Contains(p.GetType())).ToList().ForEach(p => p.Dispose());
            FrameProcessors.RemoveAll(p => toRemove.Contains(p.GetType()));
            FrameProcessors.AddRange(toAdd.Select(t => (ICameraFrameProcessor)Activator.CreateInstance(t)));
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
                        seenState.Value.BelievedPresent = false;
                        OnObjectDisappeared?.Invoke(seenState.Value);
                    }

                }
            }
        }

        public async Task RemoveBehaviourAsync(string reference)
        {
            var found = Behaviours.Where(b => b.UniqueReference == reference);
            Behaviours.RemoveAll(b => b.UniqueReference == reference);
            UpdateFrameProcessors();
            if (Connection == ConnectedState.Connected)
            {
                await ActOnAnyBehaviourPermanentRequirements();
                await UnregisterBehavioursAsync(found);
            }
            OnBehaviourEvent?.Invoke(Behaviours, BehaviourEvent.Remove);
        }

        private async Task RegisterBehavioursAsync(IEnumerable<IVectorBehaviourPlus> behaviours)
        {
            foreach (var behaviour in behaviours)
            {
                await behaviour.SetControllerAsync(this);
            }
        }

        private bool AnyBehavioursNeedCameraProcessing
        {
            get
            {
                return Behaviours.Any(b => b.NeedsFrameProcessing);
            }
        }

        private bool AnyBehavioursNeedPermanentObjectMonitoring
        {
            get
            {
                return Behaviours.Any(b => b.NeedsPermanentObjectAppearanceMonitoring);
            }
        }

        private bool AnyBehavioursNeedPermanentRobotControl
        {
            get
            {
                return Behaviours.Any(b => b.NeedsPermanentRobotControl);
            }
        }

        private async Task UnregisterBehavioursAsync(IEnumerable<IVectorBehaviourPlus> behaviours)
        {
            foreach (var behaviour in behaviours)
            {
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
            Reports.Add(report);
            OnBehaviourReport?.Invoke(report);
        }

        public void EnqueueAction(IVectorActionPlus action)
        {
            Actions.Enqueue(action);
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
            while (actionExecutorRunning && Actions.TryDequeue(out action))
            {
                if (action.NeedsControl)
                {
                    await TakeControlForActionAsync();
                }

                ActionState result = await action.ExecuteAsync(this);
                // TODO: handle action state properly - manage retries etc.

                bool ok = Actions.TryPeek(out nextAction);
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
