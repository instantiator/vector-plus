using System;
using System.Collections.Generic;
using VectorPlus.Lib.ML.YoloParsing;

namespace VectorPlus.Lib.ML
{
    public class CameraFrameProcessingResult
    {
        public byte[] Image { get; set; }
        public IEnumerable<YoloBoundingBox> Boxes { get; set; }
    }
}
