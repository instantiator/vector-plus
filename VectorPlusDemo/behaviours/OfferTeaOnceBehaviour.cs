using System;
using System.Threading.Tasks;
using Anki.Vector;
using Anki.Vector.Events;
using VectorPlusDemo.Actions;
using VectorPlusLib;

namespace VectorPlusDemo.behaviours
{
    public class OfferTeaOnceBehaviour : AbstractVectorBehaviourPlus
    {
        public OfferTeaOnceBehaviour() : base(false, false, false, false, false)
        {
        }

        protected override async Task IssueCommandsOnConnectionAsync()
        {
            controller.EnqueueAction(new StopAllMotorsAction(this, null));
            controller.EnqueueAction(new OfferTeaAction(this, null));
        }

        protected override async Task MainLoopAsync()
        {
        }

        protected override async Task RegisterWithRobotEventsAsync(EventComponent events)
        {
            // NOP
        }

        protected override async Task UnregisterFromRobotEventsAsync(EventComponent events)
        {
            // NOP
        }
    }
}
