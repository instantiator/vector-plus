using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Anki.Vector;
using Anki.Vector.Events;
using VectorPlus.Lib;

namespace VectorPlus.Demo.Behaviour.Behaviours
{
    public class MonitoringEventsBehaviour : AbstractVectorBehaviourPlus
    {
        public override string Name => "Monitor events";

        public override string Description => "A number of Vector's events will be logged.";

        public override ICollection<Type> RequestedFrameProcessors => null;

        public MonitoringEventsBehaviour(int id, bool takeControl) : base(
            id: id,
            needsPermanentControl: takeControl,
            needsPermanentObjectAppearanceMonitoring: false,
            usesMotionDetection: true,
            usesFaces: true)
        {
        }

        protected override async Task IssueCommandsOnConnectionAsync()
        {
        }

        protected override async Task MainLoopAsync()
        {
        }

        protected override async Task RegisterWithRobotEventsAsync(Robot robot)
        {
            robot.Events.FeatureStatus += Events_FeatureStatusAsync;
            robot.Events.ChangedObservedFaceId += Events_ChangedObservedFaceIdAsync;
            robot.Events.StimulationInfo += Events_StimulationInfoAsync;
            robot.Events.AttentionTransfer += Events_AttentionTransferAsync;
            robot.Events.ObservedFace += Events_ObservedFace;
            robot.Events.ObservedObject += Events_ObservedObject;
            robot.Events.RobotEvent += Events_RobotEvent;
            robot.Events.RobotState += Events_RobotState;
            robot.Events.UserIntent += Events_UserIntent;
            robot.Events.WakeWordBegin += Events_WakeWordBegin;
            robot.Events.WakeWordEnd += Events_WakeWordEnd;
        }

        protected override async Task UnregisterFromRobotEventsAsync(Robot robot)
        {
            robot.Events.FeatureStatus -= Events_FeatureStatusAsync;
            robot.Events.ChangedObservedFaceId -= Events_ChangedObservedFaceIdAsync;
            robot.Events.StimulationInfo -= Events_StimulationInfoAsync;
            robot.Events.AttentionTransfer -= Events_AttentionTransferAsync;
            robot.Events.ObservedFace -= Events_ObservedFace;
            robot.Events.ObservedObject -= Events_ObservedObject;
            robot.Events.RobotEvent -= Events_RobotEvent;
            robot.Events.RobotState -= Events_RobotState;
            robot.Events.UserIntent -= Events_UserIntent;
            robot.Events.WakeWordBegin -= Events_WakeWordBegin;
            robot.Events.WakeWordEnd -= Events_WakeWordEnd;
        }

        private async void Events_AttentionTransferAsync(object sender, AttentionTransferEventArgs e)
        {
            await ReportAsync("AttentionTransfer: " + e.Reason + " " + e.SecondsAgo + " secs ago.");
        }

        private async void Events_StimulationInfoAsync(object sender, StimulationInfoEventArgs e)
        {
            await ReportAsync("StimulationInfo: " + e.EventType + " " + string.Join(", ", e.EmotionEvents));
        }

        private async void Events_ChangedObservedFaceIdAsync(object sender, RobotChangedObservedFaceIdEventArgs e)
        {
            await ReportAsync("ChangeObservedFaceId: " + e.EventType + ", " + e.NewId);
        }

        private async void Events_FeatureStatusAsync(object sender, FeatureStatusEventArgs e)
        {
            await ReportAsync("FeatureStatus: " + e.EventType.ToString() + ", " + e.FeatureName + ", " + e.StatusType + ", " + e.Source);
        }

        private async void Events_WakeWordEnd(object sender, WakeWordEndEventArgs e)
        {
            await ReportAsync("WakeWordEnd: " + e.WakeWordEventType);
        }

        private async void Events_WakeWordBegin(object sender, WakeWordBeginEventArgs e)
        {
            await ReportAsync("WakeWordEnd: " + e.WakeWordEventType);
        }

        private async void Events_UserIntent(object sender, UserIntentEventArgs e)
        {
            await ReportAsync("UserIntent: " + e.Intent + "; " + e.IntentData);
        }

        private async void Events_RobotState(object sender, RobotStateEventArgs e)
        {
            await ReportAsync("RobotState: " + e.EventType + "; status: " + e.Status);
        }

        private async void Events_RobotEvent(object sender, RobotEventArgs e)
        {
            await ReportAsync("RobotEvent: " + e.EventType);
        }

        private async void Events_ObservedObject(object sender, RobotObservedObjectEventArgs e)
        {
            if (e.ObjectType == Anki.Vector.Objects.ObjectType.CustomObject)
            {
                await ReportAsync("ObservedObject " + e.ObjectType + ", " + e.CustomObjectType + ": " + e.EventType);
            }
            else
            {
                await ReportAsync("ObservedObject " + e.ObjectType + ": " + e.EventType);
            }
        }

        private async void Events_ObservedFace(object sender, RobotObservedFaceEventArgs e)
        {
            await ReportAsync("ObservedFace " + e.EventType + ", " + e.FaceId + ", " + e.Name);
        }

        public override async Task ReceiveKeypressAsync(char c)
        {
            // NOP
        }
    }
}
