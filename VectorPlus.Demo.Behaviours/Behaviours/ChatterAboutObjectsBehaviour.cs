using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anki.Vector;
using VectorPlus.Capabilities.Vision.Yolo;
using VectorPlus.Demo.Behaviour.Actions;
using VectorPlus.Lib;
using VectorPlus.Lib.Vision;

namespace VectorPlus.Demo.Behaviour.Behaviours
{
    public class ChatterAboutObjectsBehaviour : AbstractVectorBehaviourPlus
    {
        private TimeSpan timeoutOnSpeech;

        public override string Name => "Observational chatter";

        public override string Description => "Vector recognises 20 different objects from the YOLO model, and will chatter about them when he sees them.";

        public ChatterAboutObjectsBehaviour(int id) : base(id)
        {
            SetRefectoryPeriod(TimeSpan.FromSeconds(60));
            timeoutOnSpeech = TimeSpan.FromSeconds(3);
        }

        private Type[] frameProcessorTypes = new[] { typeof(YoloCameraFrameProcessor) };
        public override ICollection<Type> RequestedFrameProcessors => frameProcessorTypes;

        protected override async Task IssueCommandsOnConnectionAsync()
        {
        }

        protected override async Task MainLoopAsync()
        {
        }

        protected override async Task RegisterWithRobotEventsAsync(Robot robot)
        {
            controller.OnCameraFrameProcessingResult += Controller_OnCameraFrameProcessingResult;
        }

        private async Task Controller_OnCameraFrameProcessingResult(CameraFrameProcessingResult result)
        {
            if (RecoveredSinceTrigger() && result != null && result.Boxes.Count() > 0)
            {
                // This is the minimum confidence for Vector to talk about.
                var thresholdConfidence = 0.5f;

                var sortedBoxes = result.Boxes
                    .Where(b => b.Confidence > thresholdConfidence && !string.IsNullOrWhiteSpace(b.Label))
                    .OrderByDescending(b => b.Dimensions.Width * b.Dimensions.Height * b.Confidence);

                if (sortedBoxes.Count() > 0)
                {
                    RecordTrigger();
                    var label = sortedBoxes.First().Label;
                    var confidence = sortedBoxes.First().Confidence;


                    var msg = "I'm not sure, but I thought I saw ";
                    switch (confidence)
                    {
                        case float n when n >= 0.6f && n < 0.7f:
                            msg = "I think I see ";
                            break;
                        case float n when n >= 0.7f && n < 0.8f:
                            msg = "I see ";
                            break;
                        case float n when n >= 0.8f:
                            msg = "I'm confident I see ";
                            break;
                    }

                    var description = YoloNamer.ParseLabel(label);
                    var speech = msg + (description ?? label);
                    controller.EnqueueAction(new SimpleSpeechAction(this, timeoutOnSpeech, speech));
                }
            }
        }

        protected override async Task UnregisterFromRobotEventsAsync(Robot robot)
        {
            controller.OnCameraFrameProcessingResult -= Controller_OnCameraFrameProcessingResult;
        }

        public override async Task ReceiveKeypressAsync(char c)
        {
        }
    }
}
