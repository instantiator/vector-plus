using System;
using System.Collections.Generic;
using System.Linq;
using VectorPlus.Lib.ML.Onnx;
using VectorPlus.Lib.ML.YoloParsing;

namespace VectorPlus.Lib.ML
{
    public class CameraFrameProcessor
    {
        private OnnxModelScorer modelScorer;

        public CameraFrameProcessor()
        {
            modelScorer = new OnnxModelScorer();
        }

        public CameraFrameProcessingResult Process(byte[] image)
        {
            try
            {
                // This is a list of float arrays, one per image.
                // We are set up to only process 1 image at a time.
                IEnumerable<float[]> probabilities = modelScorer.ScoreYolo(new[] { image });

                // This is the minimum confidence we consider returning for any bounding box.
                var thresholdConfidence = 0.5f;

                YoloOutputParser parser = new YoloOutputParser();
                var boundingBoxSets =
                    probabilities
                    .Select(probability => parser.ParseOutputs(probability))
                    .Select(boxes => parser.FilterBoundingBoxes(boxes, 10, thresholdConfidence));

                return new CameraFrameProcessingResult()
                {
                    Image = image,                  // the original image for reference.
                    Boxes = boundingBoxSets.First() // bounding boxes for the first (only) image.
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }

        }
    }
}
