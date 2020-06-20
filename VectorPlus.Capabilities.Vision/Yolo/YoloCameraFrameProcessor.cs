using System;
using VectorPlus.Capabilities.Vision.Onnx;

namespace VectorPlus.Capabilities.Vision.Yolo
{
    public class YoloCameraFrameProcessor : AbstractOnnxCameraFrameProcessor
    {
        public YoloCameraFrameProcessor() : base()
        {
        }

        protected override string ModelResourceId => "VectorPlus.Capabilities.Vision.assets.Model.TinyYolo2_model.onnx";

        protected override AbstractOnnxModelScorer CreateModelScorer(string path) => new YoloModelScorer(path);
    }
}
