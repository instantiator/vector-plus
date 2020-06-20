using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Anki.Vector;
using VectorPlus.Capabilities.Vision.Yolo;
using VectorPlus.Lib;

namespace VectorPlus.Capabilities.Tests
{
    public class StubVisionBehaviour : AbstractVectorBehaviourPlus
    {
        public StubVisionBehaviour(int id)
            : base(
                  id,
                  needsPermanentControl: false,
                  needsPermanentObjectAppearanceMonitoring: false,
                  usesMotionDetection: false,
                  usesFaces: false,
                  usesCustomObjects: false,
                  usesMirrorMode: false)
        {
        }

        public override string Name => "Stub Vision Behaviour";

        public override string Description => "A simple behaviour for testing vision.";

        public override ICollection<Type> RequestedFrameProcessors => new[] { typeof(YoloCameraFrameProcessor) };

        public override async Task ReceiveKeypressAsync(char c) { }

        protected override async Task IssueCommandsOnConnectionAsync() { }

        protected override async Task MainLoopAsync() { }

        protected override async Task RegisterWithRobotEventsAsync(Robot robot) { }

        protected override async Task UnregisterFromRobotEventsAsync(Robot robot) { }
    }
}
