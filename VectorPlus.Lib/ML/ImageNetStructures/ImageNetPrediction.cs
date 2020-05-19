using System;
using Microsoft.ML.Data;

namespace VectorPlus.Lib.ML.ImageNetStructures
{
    public class ImageNetPrediction
    {
        [ColumnName("grid")]
        public float[] PredictedLabels;
    }
}
