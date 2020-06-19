using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.ML;
using VectorPlus.Capabilities.Vision.Onnx;
using VectorPlus.Lib.Helpers;

namespace VectorPlus.Capabilities.Vision.Yolo
{
    public class YoloModelScorer : AbstractOnnxModelScorer
    {
        public YoloModelScorer() : base()
        {
        }

        protected override string ModelPath => Path.Combine(PathHelper.GetAbsolutePath("assets"), "Model", "TinyYolo2_model.onnx");

        protected override string ModelInputColumn => "image";

        protected override string ModelOutputColumn => "grid";
    }
}
