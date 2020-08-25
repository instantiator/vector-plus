using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using VectorPlus.Demo.Behaviour.Behaviours;
using VectorPlus.Lib;

namespace VectorPlus.Console
{
    class Program
    {
        static string LOG_FILE = "VectorPlusLog.txt";
        static CancellationTokenSource cancelSource;

        static async Task Main(string[] args)
        {
            System.Console.WriteLine("Hello World!");

            var config = new VectorControllerPlusConfig()
            {
                ReconnectDelay_ms = 2000
            };

            await using (IVectorControllerPlus controller = new VectorControllerPlus())
            {
                controller.OnConnectionChanged += async (previously, state) => System.Console.WriteLine("! Connection: " + state);

                //await controller.AddBehaviourAsync(new MonitoringEventsBehaviour(false));
                await controller.AddBehaviourAsync(new OfferTeaSometimesBehaviour(0));
                await controller.AddBehaviourAsync(new ExterminateCubeOnSightBehaviour(1));

                controller.OnBehaviourReport += async (report) =>
                {
                    System.Console.WriteLine(report.Description);
                    await File.AppendAllLinesAsync(LOG_FILE, new[] { report.Description });
                };

                await controller.ConnectAsync(config);

                cancelSource = new CancellationTokenSource();
                await controller.StartMainLoopAsync(cancelSource.Token);
            }
        }

    }
}
