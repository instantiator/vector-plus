using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Anki.Vector;
using Anki.Vector.Objects;
using VectorPlus.Demo.Behaviour.Actions;
using VectorPlus.Lib;
using VectorPlus.Lib.Actions;

namespace VectorPlus.Demo.Behaviour.Behaviours
{
    public class ExterminateCubeOnSightBehaviour : AbstractVectorBehaviourPlus
    {
        public override string Name => "Exterminate cube on sight";

        public override string Description => "Vector exterminates the cube every once in a while when he sees it.";

        public override ICollection<Type> RequestedFrameProcessors => null;

        public ExterminateCubeOnSightBehaviour(int id) : base(id, false, true, false, false, false, false)
        {
            SetRefectoryPeriod(TimeSpan.FromMinutes(5));
        }

        public override IVectorActionPlus ActionOnAdded => new SimpleSpeechAction(this, "Seek! Locate! Destroy!");
        public override IVectorActionPlus ActionOnRemoved => new SimpleSpeechAction(this, "You would make a good Dalek.");

        protected override async Task IssueCommandsOnConnectionAsync()
        {
            await ReportAsync("Press 'X' for a quick extermination.");
        }

        protected override async Task MainLoopAsync()
        {
            // NOP
        }

        protected override async Task RegisterWithRobotEventsAsync(Robot robot)
        {
            controller.OnObjectAppeared += Controller_OnObjectAppeared;
        }

        protected override async Task UnregisterFromRobotEventsAsync(Robot robot)
        {
            controller.OnObjectAppeared -= Controller_OnObjectAppeared;
        }

        private async Task Controller_OnObjectAppeared(ObjectSeenState arg)
        {
            if (arg.ObjectType == ObjectType.BlockLightcube1 && RecoveredSinceTrigger())
            {
                RecordTrigger();
                controller.EnqueueAction(new ExterminateAction(this, null));
            }
        }

        public override async Task ReceiveKeypressAsync(char c)
        {
            if (c == 'x' || c == 'X')
            {
                controller.EnqueueAction(new ExterminateAction(this, null));
            }
        }
    }
}
