using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using VectorPlus.Capabilities.Vision.Yolo;
using VectorPlus.Lib;

namespace VectorPlus.Capabilities.Tests
{
    public class TestVectorControllerPlusWithVision
    {
        VectorControllerPlus controller;
        Task controllerMainLoop;
        CancellationTokenSource cancelSource;

        [SetUp]
        public async Task SetupAsync()
        {
            cancelSource = new CancellationTokenSource();
            controller = new VectorControllerPlus();
            controllerMainLoop = controller.StartMainLoopAsync(cancelSource.Token);
        }

        [TearDown]
        public async Task TearDownAsync()
        {
            await controller.DisposeAsync();
        }

        [Test]
        public void TestFrameProcessorsNotNull()
        {
            Assert.NotNull(controller.FrameProcessors);
        }

        /// <summary>
        /// Tests that the processor unpacks its ONNX model during construction.
        /// </summary>
        [Test]
        public void TestYoloCameraFrameProcessorUnpacksModel()
        {
            using (var processor = new YoloCameraFrameProcessor())
            {
                Assert.NotNull(processor.ModelPath);
                Assert.True(processor.Ready);
                var path = processor.ModelPath;
                Assert.IsNotEmpty(path);
                Assert.True(File.Exists(path));
            }
        }

        /// <summary>
        /// Tests that a controller connected to a robot can pass camera frames to the processor.
        /// </summary>
        [Test]
        public async Task TestStubVisionBehaviourPlusReceivesFrames()
        {
            await controller.AddBehaviourAsync(new StubVisionBehaviour(0));
            var config = new VectorControllerPlusConfig() { ReconnectDelay_ms = 2000 };
            var connected = await controller.ConnectAsync(config);
            Assert.True(connected);

            var processor = controller.FrameProcessors.First();
            Assert.True(processor is YoloCameraFrameProcessor);
            Thread.Sleep(500);
            Assert.True(processor.FramesProcessed >= 2);
        }

        [Test]
        public async Task TestControllerAcceptsYoloCameraFrameProcessorAsync()
        {
            await controller.AddBehaviourAsync(new StubVisionBehaviour(0));
            Assert.True(controller.FrameProcessors.Any(p => p is YoloCameraFrameProcessor));
        }


    }
}
