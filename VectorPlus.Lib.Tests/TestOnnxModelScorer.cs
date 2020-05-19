using System.IO;
using System.Linq;
using NUnit.Framework;
using VectorPlus.Lib.ML.Onnx;
using VectorPlus.Lib.ML.YoloParsing;

namespace VectorPlus.Lib.Tests
{
    public class TestOnnxModelScorer
    {
        private OnnxModelScorer scorer;

        [SetUp]
        public void Setup()
        {
            scorer = scorer ?? new OnnxModelScorer();
        }

        [Test]
        public void TestOnnxModelScorerLoadModel()
        {
            // TODO: add to notes, you need: brew install mono-libgdiplus

            var cat_jpg = File.ReadAllBytes("assets/001-cat.jpg");
            var scorer = new OnnxModelScorer();

            var result = scorer.ScoreYolo(new[] { cat_jpg });
            Assert.NotNull(result);

            YoloOutputParser parser = new YoloOutputParser();
            var boundingBoxes =
                result
                .Select(probability => parser.ParseOutputs(probability))
                .Select(boxes => parser.FilterBoundingBoxes(boxes, 5, .5F));

            Assert.NotNull(boundingBoxes);

            var first_setOfBoxes = boundingBoxes.First();

            Assert.NotNull(first_setOfBoxes);
            Assert.Equals(first_setOfBoxes[0].Label, "cat");

        }
    }
}