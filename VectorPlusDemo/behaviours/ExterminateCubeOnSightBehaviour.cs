using System;
using System.Threading.Tasks;
using Anki.Vector;
using Anki.Vector.Objects;
using VectorPlusDemo.Actions;
using VectorPlusLib;

namespace VectorPlusDemo.behaviours
{
    public class ExterminateCubeOnSightBehaviour : AbstractVectorBehaviourPlus
    {
        private DateTime lastExtermination = DateTime.MinValue;
        private readonly TimeSpan exterminationRefectory = TimeSpan.FromSeconds(10); // can't just keep yelling exterminate like a bloody dalek
        private ObjectType lastObservation;

        public ExterminateCubeOnSightBehaviour() : base(false, true, false, false, false, false)
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
            controller.OnObjectAppeared += Controller_OnObjectAppeared;
        }

        protected override async Task UnregisterFromRobotEventsAsync(EventComponent events)
        {
            controller.OnObjectAppeared -= Controller_OnObjectAppeared;
        }

        private async Task Controller_OnObjectAppeared(ObjectSeenState arg)
        {
            if (arg.ObjectType == ObjectType.BlockLightcube1)
            {
                controller.EnqueueAction(new ExterminateAction(this, null));
            }
        }
    }
}
