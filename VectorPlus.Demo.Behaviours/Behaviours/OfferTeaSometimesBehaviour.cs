using System;
using System.Collections.Generic;
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

        public override string Description => "Vector will offer you a cup of tea every once in a while.";

        public override ICollection<Type> RequestedFrameProcessors => null;

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

        protected override async Task RegisterWithRobotEventsAsync(Robot robot)
        {
            robot.Events.ChangedObservedFaceId += Events_ChangedObservedFaceId;
        }

        private void Events_ChangedObservedFaceId(object sender, Anki.Vector.Events.RobotChangedObservedFaceIdEventArgs e)
        {
            var worldFace = controller.Robot.World.GetFaceById(e.NewId);
            var name = worldFace.Name;

            if (RecoveredSinceTrigger(name))
            {
                RecordTrigger(name);
                controller.EnqueueAction(new StopAllMotorsAction(this, null));
                if (worldFace != null)
                {
                    controller.EnqueueAction(new TurnToFaceAction(this, null, worldFace));
                }
                controller.EnqueueAction(new OfferTeaAction(this, null, name));
            }
        }

        protected override async Task UnregisterFromRobotEventsAsync(Robot robot)
        {
            robot.Events.ChangedObservedFaceId -= Events_ChangedObservedFaceId;
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
