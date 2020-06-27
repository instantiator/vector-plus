using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Anki.Vector;
using VectorPlus.Lib;
using VectorPlus.Lib.Vision;

namespace VectorPlus.Demo.Behaviour.Behaviours
{
    // TODO: this is not yet implemented
    public class LaserChasingBehaviour : AbstractVectorBehaviourPlus
    {
        public LaserChasingBehaviour(int id) : base(id, usesMirrorMode: true)
        {
        }

        public override string Name => "Laser chaser";

        public override string Description => "Vector loves a good chase! Throw down a laser spot and if he sees it, the game begins!";

        public override ICollection<Type> RequestedFrameProcessors => null; // this is the one

        protected override async Task IssueCommandsOnConnectionAsync() { }

        protected override async Task MainLoopAsync() { }

        protected override async Task RegisterWithRobotEventsAsync(Robot robot)
        {
            controller.OnCameraFrameProcessingResult += Controller_OnCameraFrameProcessingResult;
        }

        private async Task Controller_OnCameraFrameProcessingResult(CameraFrameProcessingResult arg)
        {
            // TODO: if it came from the spot detector, and Vector's head is in a good orientation, use that to initiate or inform a chase
            // TODO: if the spot detector hasn't seen the spot for a while, abandon the chase
        }

        protected override async Task UnregisterFromRobotEventsAsync(Robot robot)
        {
            controller.OnCameraFrameProcessingResult -= Controller_OnCameraFrameProcessingResult;
        }

        public override async Task ReceiveKeypressAsync(char c) { }
    }
}
