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

        protected override async Task RegisterWithRobotEventsAsync(EventComponent events)
        {
            events.FeatureStatus += Events_FeatureStatusAsync;
            events.ChangedObservedFaceId += Events_ChangedObservedFaceIdAsync;
            events.StimulationInfo += Events_StimulationInfoAsync;
            events.AttentionTransfer += Events_AttentionTransferAsync;
            events.ObservedFace += Events_ObservedFace;
            events.ObservedObject += Events_ObservedObject;
            events.RobotEvent += Events_RobotEvent;
            events.RobotState += Events_RobotState;
            events.UserIntent += Events_UserIntent;
            events.WakeWordBegin += Events_WakeWordBegin;
            events.WakeWordEnd += Events_WakeWordEnd;
        }

        protected override async Task UnregisterFromRobotEventsAsync(EventComponent events)
        {
            events.FeatureStatus -= Events_FeatureStatusAsync;
            events.ChangedObservedFaceId -= Events_ChangedObservedFaceIdAsync;
            events.StimulationInfo -= Events_StimulationInfoAsync;
            events.AttentionTransfer -= Events_AttentionTransferAsync;
            events.ObservedFace -= Events_ObservedFace;
            events.ObservedObject -= Events_ObservedObject;
            events.RobotEvent -= Events_RobotEvent;
            events.RobotState -= Events_RobotState;
            events.UserIntent -= Events_UserIntent;
            events.WakeWordBegin -= Events_WakeWordBegin;
            events.WakeWordEnd -= Events_WakeWordEnd;
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
