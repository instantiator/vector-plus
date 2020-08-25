using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Anki.Vector;
using VectorPlus.Lib;
using VectorPlus.Lib.Actions;
using VectorPlus.Lib.Vision;

namespace VectorPlus.Demo.Behaviour.Behaviours
{
    // TODO: this is not yet implemented
    [Obsolete("Not yet implemented")]
    public class LaserChasingBehaviour : AbstractVectorBehaviourPlus
    {
        public LaserChasingBehaviour(int id) : base(id, usesMirrorMode: true)
        {
        }

        public override string Name => "Laser chaser";

        public override string Description => "Vector loves a good chase! Throw down a laser spot and if he sees it, the game begins!";

        public override ICollection<Type> RequestedFrameProcessors => null; // this is the one

        public override IVectorActionPlus ActionOnAdded => new SimpleSpeechAction(this, "I'm ready to catch that little red dot.");
        public override IVectorActionPlus ActionOnRemoved => new SimpleSpeechAction(this, "Lets chase the dot another time.");

        protected override async Task IssueCommandsOnConnectionAsync() { }

        protected override async Task MainLoopAsync() { }

        protected override async Task RegisterWithRobotEventsAsync(Robot robot)
        {
            controller.OnCameraFrameProcessingResult += Controller_OnCameraFrameProcessingResult;
        }

        private async Task Controller_OnCameraFrameProcessingResult(CameraFrameProcessingResult arg)
        {
            // TODO: if the spot detector is confident there's a laser and has been for X seconds then:
            // - move head if the spot is high, lower head if it's low
            // - use angle of head to determine whether spot is far or near by height
            // - if spot is left, turn left a little (and vice versa for right)

            // TODO: if the spot seemed to be near, and then it disappeared, we might have caught it
            // TODO: if the spot is nearby and not moving, initiate a pounce?

            // TODO: if the spot detector hasn't seen the spot for a while, abandon the chase
        }

        protected override async Task UnregisterFromRobotEventsAsync(Robot robot)
        {
            controller.OnCameraFrameProcessingResult -= Controller_OnCameraFrameProcessingResult;
        }

        public override async Task ReceiveKeypressAsync(char c) { }
    }
}
