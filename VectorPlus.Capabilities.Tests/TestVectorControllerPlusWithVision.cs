using System;
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

        [Test]
        public void TestControllerAcceptsYoloCameraFrameProcessor()
        {
            // TODO: add a YoloCameraFrameProcessor
            // TODO: mock it though, so you can prove it gets called
            Assert.True(controller.FrameProcessors.Any(p => p is YoloCameraFrameProcessor));
        }
    }
}
