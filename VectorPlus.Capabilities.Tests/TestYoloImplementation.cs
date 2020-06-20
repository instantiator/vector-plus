using System.IO;
using System.Linq;
using NUnit.Framework;
using VectorPlus.Capabilities.Vision.Yolo;
using VectorPlus.Lib.Helpers;

namespace VectorPlus.Capabilities.Tests
{
    public class TestYoloImplementation
    {
        private YoloModelScorer scorer;

        [SetUp]
        public void Setup()
        {
            //Assembly.GetCallingAssembly()
            var assembly = typeof(YoloCameraFrameProcessor).Assembly;
            var modelPath = PathHelper.CopyResourceToFile(assembly, "VectorPlus.Capabilities.Vision.assets.Model.TinyYolo2_model.onnx");
            scorer = scorer ?? new YoloModelScorer(modelPath);
        }

        [Test]
        public void TestYoloModelScorerMultipleUses()
        {
            // TODO: add to notes, you need: brew install mono-libgdiplus
            var cat_jpg = File.ReadAllBytes("assets/001-cat.jpg");

            var result = scorer.Score(new[] { cat_jpg });
            Assert.NotNull(result);

            var result2 = scorer.Score(new[] { cat_jpg });
            Assert.NotNull(result2);
            Assert.AreEqual(result, result2);
        }

        [Test]
        public void TestYoloOutputParserCorrectlyNamesCat()
        {
            var cat_jpg = File.ReadAllBytes("assets/001-cat.jpg");

            var result = scorer.Score(new[] { cat_jpg });
            Assert.NotNull(result);

            YoloOutputParser parser = new YoloOutputParser();
            var boundingBoxes =
                result
                .Select(probability => parser.ParseOutputs(probability))
                .Select(boxes => parser.FilterBoundingBoxes(boxes, 5, .5F));

            Assert.NotNull(boundingBoxes);
            var first_setOfBoxes = boundingBoxes.First();
            Assert.NotNull(first_setOfBoxes);
            Assert.AreEqual(first_setOfBoxes[0].Label, "cat");
        }

        [Test]
        public void TestCameraFrameProcessorParsesImage()
        {
            var processor = new YoloCameraFrameProcessor();
            var cat_jpg = File.ReadAllBytes("assets/001-cat.jpg");
            var result = processor.Process(cat_jpg);
            Assert.NotNull(result);
            Assert.AreEqual(cat_jpg, result.Image);
            Assert.NotNull(result.Boxes);
            var firstBox = result.Boxes.First();
            Assert.NotNull(firstBox);
            Assert.AreEqual(firstBox.Label, "cat");
        }
    }
}