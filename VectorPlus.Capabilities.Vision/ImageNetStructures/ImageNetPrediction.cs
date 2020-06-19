using System;
using Microsoft.ML.Data;

namespace VectorPlus.Capabilities.Vision.ImageNetStructures
{
    public class ImageNetPrediction
    {
        [ColumnName("grid")]
        public float[] PredictedLabels;
    }
}
