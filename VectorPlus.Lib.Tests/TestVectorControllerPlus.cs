using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace VectorPlus.Lib.Tests
{
    public class TestVectorControllerPlus
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
        public void TestControllerStartsUp()
        {
            Assert.True(controller.MainLoopRunning);
            Thread.Sleep(500);
            Assert.True(controller.MainLoopRunning);
        }

        [Test]
        public async Task TestControllerCanConnectToRobotAsync()
        {
            var config = new VectorControllerPlusConfig() { ReconnectDelay_ms = 2000 };
            var connected = await controller.ConnectAsync(config);
            Assert.True(connected);
        }

    }
}