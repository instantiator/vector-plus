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
            //controllerMainLoop.Start();
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
        }

        
    }
}
