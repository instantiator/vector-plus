using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Data;
using VectorPlus.Lib.Helpers;
using VectorPlus.Lib.ML.ImageNetStructures;
using VectorPlus.Lib.ML.YoloParsing;

namespace VectorPlus.Lib.ML.Onnx
{
    public class OnnxModelScorer
    {
        public struct ImageNetSettings
        {
            public const int imageHeight = 416;
            public const int imageWidth = 416;
        }

        private readonly MLContext mlContext;

        public OnnxModelScorer(MLContext mlContext = null)
        {
            this.mlContext = mlContext ?? new MLContext();
        }

        private ITransformer CreateModel(string path, string modelInput, string modelOutput)
        {
            var data = mlContext.Data.LoadFromEnumerable(new List<ImageNetData>());

            var pipeline = mlContext.Transforms
                .LoadImages(outputColumnName: "image", imageFolder: "", inputColumnName: nameof(ImageNetData.ImagePath))
                .Append(mlContext.Transforms.ResizeImages(outputColumnName: "image", imageWidth: ImageNetSettings.imageWidth, imageHeight: ImageNetSettings.imageHeight, inputColumnName: "image"))
                .Append(mlContext.Transforms.ExtractPixels(outputColumnName: "image"))
                .Append(mlContext.Transforms.ApplyOnnxModel(modelFile: path, outputColumnNames: new[] { modelOutput }, inputColumnNames: new[] { modelInput }));

            var model = pipeline.Fit(data);
            return model;
        }

        private IEnumerable<float[]> PredictDataUsingModel(IDataView testData, ITransformer model, string modelOutput)
        {
            IDataView scoredData = model.Transform(testData);
            IEnumerable<float[]> probabilities = scoredData.GetColumn<float[]>(modelOutput);
            return probabilities;
        }

        public IEnumerable<float[]> ScoreYolo(IEnumerable<byte[]> images)
        {
            string assetsPath = PathHelper.GetAbsolutePath("assets");
            var modelFilePath = Path.Combine(assetsPath, "Model", "TinyYolo2_model.onnx");
            return Score(modelFilePath, "image", "grid", images);
        }

        private IEnumerable<float[]> Score(string modelPath, string modelInput, string modelOutput, IEnumerable<byte[]> imageBytes)
        {
            IEnumerable<ImageNetData> images = ImageNetData.FromImages(imageBytes);
            var dataView = mlContext.Data.LoadFromEnumerable(images);
            var result = PredictDataUsingModel(dataView, CreateModel(modelPath, modelInput, modelOutput), modelOutput);
            ImageNetData.EraseFiles(images);
            return result;
        }

    }
}
