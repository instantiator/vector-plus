using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Data;
using VectorPlus.Capabilities.Vision.ImageNetStructures;

namespace VectorPlus.Capabilities.Vision.Onnx
{
    public abstract class AbstractOnnxModelScorer
    {
        public struct ImageNetSettings
        {
            public const int imageHeight = 416;
            public const int imageWidth = 416;
        }

        private MLContext mlContext;
        private ITransformer model;

        protected abstract string ModelInputColumn { get; }
        protected abstract string ModelOutputColumn { get; }

        protected AbstractOnnxModelScorer(string modelPath)
        {
            mlContext = new MLContext();
            model = CreateModel(modelPath);
        }

        private ITransformer CreateModel(string modelPath)
        {
            var data = mlContext.Data.LoadFromEnumerable(new List<ImageNetData>());

            var pipeline = mlContext.Transforms
                .LoadImages(outputColumnName: ModelInputColumn, imageFolder: "", inputColumnName: nameof(ImageNetData.ImagePath))
                .Append(mlContext.Transforms.ResizeImages(outputColumnName: ModelInputColumn, imageWidth: ImageNetSettings.imageWidth, imageHeight: ImageNetSettings.imageHeight, inputColumnName: ModelInputColumn))
                .Append(mlContext.Transforms.ExtractPixels(outputColumnName: ModelInputColumn))
                .Append(mlContext.Transforms.ApplyOnnxModel(modelFile: modelPath, outputColumnNames: new[] { ModelOutputColumn }, inputColumnNames: new[] { ModelInputColumn }));

            var model = pipeline.Fit(data);
            return model;
        }

        private IEnumerable<float[]> PredictDataUsingModel(IDataView testData)
        {
            IDataView scoredData = model.Transform(testData);
            IEnumerable<float[]> probabilities = scoredData.GetColumn<float[]>(ModelOutputColumn);
            return probabilities;
        }

        public IEnumerable<float[]> Score(IEnumerable<byte[]> imageBytes)
        {

            IEnumerable<ImageNetData> images = ImageNetData.FromImages(imageBytes);
            var dataView = mlContext.Data.LoadFromEnumerable(images);
            var result = PredictDataUsingModel(dataView);
            ImageNetData.EraseFiles(images); // TODO: do this asynchronously
            return result;
        }

    }
}
