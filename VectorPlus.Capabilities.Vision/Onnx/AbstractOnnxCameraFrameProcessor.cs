using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using VectorPlus.Capabilities.Vision.Yolo;
using VectorPlus.Lib.Helpers;
using VectorPlus.Lib.Vision;

namespace VectorPlus.Capabilities.Vision.Onnx
{
    public abstract class AbstractOnnxCameraFrameProcessor : ICameraFrameProcessor
    {
        private  AbstractOnnxModelScorer scorer;

        protected AbstractOnnxCameraFrameProcessor()
        {
            ModelPath = ModelPath ?? UnpackModelFile();
            scorer = CreateModelScorer(ModelPath);
        }

        public void Dispose()
        {
            if (File.Exists(ModelPath))
            {
                File.Delete(ModelPath);
            }
        }

        protected string UnpackModelFile()
        {
            if (!string.IsNullOrWhiteSpace(ModelResourceId))
            {
                var assembly = GetType().Assembly;
                return PathHelper.CopyResourceToFile(assembly, ModelResourceId);
            }
            else
            {
                return null;
            }
            
        }

        protected abstract string ModelResourceId { get; }

        public bool Ready => !string.IsNullOrWhiteSpace(ModelPath) && File.Exists(ModelPath);

        public string ModelPath { get; protected set; }

        protected abstract AbstractOnnxModelScorer CreateModelScorer(string path);

        public int FramesProcessed { get; protected set; }

        public CameraFrameProcessingResult Process(byte[] image)
        {
            if (image == null || image.Length == 0) { return null; }

            try
            {
                // This is a list of float arrays, one per image.
                // We are set up to only process 1 image at a time.
                IEnumerable<float[]> probabilities = scorer.Score(new[] { image });

                // This is the minimum confidence we consider returning for any bounding box.
                var thresholdConfidence = 0.5f;

                YoloOutputParser parser = new YoloOutputParser();
                var boundingBoxSets =
                    probabilities
                    .Select(probability => parser.ParseOutputs(probability))
                    .Select(boxes => parser.FilterBoundingBoxes(boxes, 10, thresholdConfidence));

                FramesProcessed++;

                return new CameraFrameProcessingResult()
                {
                    Processor = this,               // the processor that generated this result.
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
