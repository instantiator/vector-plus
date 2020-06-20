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
        public YoloModelScorer(string modelPath) : base(modelPath)
        {
        }

        protected override string ModelInputColumn => "image";

        protected override string ModelOutputColumn => "grid";
    }
}
