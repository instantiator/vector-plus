using System;
using VectorPlus.Capabilities.Vision.Onnx;

namespace VectorPlus.Capabilities.Vision.Yolo
{
    public class YoloCameraFrameProcessor : AbstractOnnxCameraFrameProcessor
    {
        public YoloCameraFrameProcessor() : base()
        {
        }

        protected override AbstractOnnxModelScorer CreateModelScorer()
        {
            return new YoloModelScorer();
        }
    }
}
