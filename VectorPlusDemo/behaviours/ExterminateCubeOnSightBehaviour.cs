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

        public ExterminateCubeOnSightBehaviour() : base(false, false, false, false, false)
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
            // This would have been 'object appeared' under the python SDK - but that's not available to us...
            events.ObservedObject += Events_ObservedObject;
        }

        private void Events_ObservedObject(object sender, Anki.Vector.Events.RobotObservedObjectEventArgs e)
        {
            if (e.ObjectType != lastObservation)
            {
                Console.WriteLine("= Observed object of type: " + e.ObjectType.ToString());
                lastObservation = e.ObjectType;
            }
            if (e.ObjectType == ObjectType.BlockLightcube1 && lastExtermination + exterminationRefectory < DateTime.Now)
            {
                lastExtermination = DateTime.Now;
                controller.EnqueueAction(new ExterminateAction(this, null)); // TODO: why doesn't timeout work?
            }
        }

        protected override async Task UnregisterFromRobotEventsAsync(EventComponent events)
        {
            events.ObservedObject -= Events_ObservedObject;
        }
    }
}
