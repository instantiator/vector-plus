using System;
using System.Threading;
using System.Threading.Tasks;
using Anki.Vector;
using Anki.Vector.Events;
using static VectorPlus.Lib.IVectorControllerPlus;

namespace VectorPlus.Lib
{
    public abstract class AbstractVectorBehaviourPlus : IVectorBehaviourPlus
    {
        public Guid Id { get; private set; }

        public string UniqueReference { get; private set; }

        protected IVectorControllerPlus controller;
        protected bool usesCustomObjectDetection;
        protected bool usesFaceDetection;
        protected bool usesMirrorMode;
        protected bool usesMotionDetection;

        protected bool mainLoopActive;
        protected Task mainLoopTask;
        protected CancellationTokenSource cancelMainLoop;

        protected DateTime lastTrigger;
        protected TimeSpan refectoryPeriod;

        protected AbstractVectorBehaviourPlus(
            int id,
            bool needsPermanentControl,
            bool needsPermanentObjectAppearanceMonitoring,
            bool usesMotionDetection = false,
            bool usesFaces = false,
            bool usesCustomObjects = false,
            bool usesMirrorMode = false)
        {
            this.Id = Guid.NewGuid();
            this.UniqueReference = GetType().FullName + ":" + id;

            this.NeedsPermanentObjectAppearanceMonitoring = needsPermanentObjectAppearanceMonitoring;
            this.NeedsPermanentRobotControl = needsPermanentControl;
            this.usesCustomObjectDetection = usesCustomObjects;
            this.usesFaceDetection = usesFaces;
            this.usesMirrorMode = usesMirrorMode;
            this.usesMotionDetection = usesMotionDetection;
        }

        public abstract string Name { get; }
        public abstract string Description { get; }

        public bool NeedsPermanentObjectAppearanceMonitoring { get; private set; }

        public bool NeedsPermanentRobotControl { get; private set; }

        protected void SetRefectoryPeriod(TimeSpan span) { this.refectoryPeriod = span; }

        protected void RecordTrigger() { this.lastTrigger = DateTime.Now; }

        protected bool RecoveredSinceTrigger
        {
            get
            {
                return
                    lastTrigger == null ||
                    refectoryPeriod == null ||
                    DateTime.Now - lastTrigger > refectoryPeriod;
            }
        }

        public async Task SetControllerAsync(IVectorControllerPlus controller)
        {
            if (this.controller != controller && this.controller != null) { this.controller.OnConnectionChanged -= Controller_OnConnectionChanged; }
            if (this.controller != controller && controller != null) { controller.OnConnectionChanged += Controller_OnConnectionChanged; }
            this.controller = controller;
            await HandleConnectionStateAsync();
        }

        private async Task Controller_OnConnectionChanged(ConnectedState state) { await HandleConnectionStateAsync(); }

        private async Task HandleConnectionStateAsync()
        {
            if (controller == null)
            {
                await ActionsOnRobotDisconnectedAsync();
            }
            else
            {
                switch (controller.Connection)
                {
                    case ConnectedState.Connected:
                        await ActionsOnRobotConnectedAsync(controller.Robot);
                        break;
                    case ConnectedState.Disconnected:
                    case ConnectedState.Disconnecting:
                        await ActionsOnRobotDisconnectedAsync();
                        break;
                }
            }
        }

        protected async Task ActionsOnRobotConnectedAsync(Robot robot)
        {
            // TODO: capture task error codes
            if (usesCustomObjectDetection) { await robot.Vision.EnableCustomObjectDetection(); }
            if (usesFaceDetection) { await robot.Vision.EnableFaceDetection(); }
            if (usesMirrorMode) { await robot.Vision.EnableMirrorMode(); }
            if (usesMotionDetection) { await robot.Vision.EnableMotionDetection(); }
            await RegisterWithRobotEventsAsync(controller.Robot.Events);
            await IssueCommandsOnConnectionAsync();
            StartMainLoop();
        }

        protected async Task ActionsOnRobotDisconnectedAsync()
        {
            StopMainLoop();
            if (controller != null) { await UnregisterFromRobotEventsAsync(controller.Robot.Events); }
        }

        public void StartMainLoop()
        {
            if (!mainLoopActive)
            {
                mainLoopActive = true;
                cancelMainLoop = new CancellationTokenSource();
                mainLoopTask = Task.Run(async () => await MainLoopAsync(), cancelMainLoop.Token);
            }
        }

        public void StopMainLoop()
        {
            if (mainLoopActive)
            {
                mainLoopActive = false;
                cancelMainLoop.Cancel();
            }
        }

        protected async Task ReportAsync(string description)
        {
            var report = new VectorBehaviourPlusReport()
            {
                Description = description,
                Controller = controller,
                Robot = controller.Robot,
                Behaviour = this
            };
            await controller.ReportAsync(report);
        }

        public abstract Task ReceiveKeypressAsync(char c);

        protected abstract Task UnregisterFromRobotEventsAsync(EventComponent events);
        protected abstract Task RegisterWithRobotEventsAsync(EventComponent events);
        protected abstract Task IssueCommandsOnConnectionAsync();
        protected abstract Task MainLoopAsync();

    }
}
