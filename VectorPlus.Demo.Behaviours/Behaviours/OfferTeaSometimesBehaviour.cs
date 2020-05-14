using System;
using System.Linq;
using System.Threading.Tasks;
using Anki.Vector;
using VectorPlus.Demo.Behaviour.Actions;
using VectorPlus.Lib;

namespace VectorPlus.Demo.Behaviour.Behaviours
{
    public class OfferTeaSometimesBehaviour : AbstractVectorBehaviourPlus
    {
        public override string Name => "Offer tea";

        public override string Description => "Vector will offer you a cup of tea.";

        public OfferTeaSometimesBehaviour(int id) : base(id, false, true, false, false, false, false)
        {
            SetRefectoryPeriod(TimeSpan.FromMinutes(5));
        }

        protected override async Task IssueCommandsOnConnectionAsync()
        {
            await ReportAsync("Press 'T' for tea!");
        }

        protected override async Task MainLoopAsync()
        {
        }

        protected override async Task RegisterWithRobotEventsAsync(EventComponent events)
        {
            events.ChangedObservedFaceId += Events_ChangedObservedFaceId;
        }

        private async void Events_ChangedObservedFaceId(object sender, Anki.Vector.Events.RobotChangedObservedFaceIdEventArgs e)
        {
            var faces = await controller.Robot.Faces.GetEnrolledFaces();
            var face = faces.Where(f => f.FaceId == e.NewId).SingleOrDefault();
            var name = face?.Name;

            if (RecoveredSinceTrigger)
            {
                RecordTrigger();
                controller.EnqueueAction(new StopAllMotorsAction(this, null));
                controller.EnqueueAction(new OfferTeaAction(this, null, name));
            }
        }

        protected override async Task UnregisterFromRobotEventsAsync(EventComponent events)
        {
            events.ChangedObservedFaceId -= Events_ChangedObservedFaceId;
        }

        public override async Task ReceiveKeypressAsync(char c)
        {
            if (c == 'T' || c == 't')
            {
                controller.EnqueueAction(new StopAllMotorsAction(this, null));
                controller.EnqueueAction(new FaceSomeoneAction(this, null));
                controller.EnqueueAction(new OfferTeaAction(this, null));
            }
        }
    }
}
