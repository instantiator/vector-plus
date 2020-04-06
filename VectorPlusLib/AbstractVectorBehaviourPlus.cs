using System;
using System.Threading;
using System.Threading.Tasks;
using Anki.Vector;
using Anki.Vector.Events;
using static VectorPlusLib.IVectorControllerPlus;

namespace VectorPlusLib
{
    public abstract class AbstractVectorBehaviourPlus : IVectorBehaviourPlus
    {
        public Guid Id { get; private set; }

        protected IVectorControllerPlus controller;
        protected bool usesCustomObjectDetection;
        protected bool usesFaceDetection;
        protected bool usesMirrorMode;
        protected bool usesMotionDetection;

        protected bool mainLoopActive;
        protected Task mainLoopTask;
        protected CancellationTokenSource cancelMainLoop;

        protected AbstractVectorBehaviourPlus(bool needsPermanentControl, bool usesMotionDetection = false, bool usesFaces = false, bool usesCustomObjects = false, bool usesMirrorMode = false)
        {
            Id = Guid.NewGuid();

            this.NeedsPermanentRobotControl = needsPermanentControl;
            this.usesCustomObjectDetection = usesCustomObjects;
            this.usesFaceDetection = usesFaces;
            this.usesMirrorMode = usesMirrorMode;
            this.usesMotionDetection = usesMotionDetection;
        }

        public bool NeedsPermanentRobotControl { get; private set; }

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

        protected abstract Task UnregisterFromRobotEventsAsync(EventComponent events);
        protected abstract Task RegisterWithRobotEventsAsync(EventComponent events);
        protected abstract Task IssueCommandsOnConnectionAsync();
        protected abstract Task MainLoopAsync();


    }
}
