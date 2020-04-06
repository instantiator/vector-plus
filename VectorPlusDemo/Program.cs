using System;
using System.Threading.Tasks;
using VectorPlusDemo.behaviours;
using VectorPlusLib;

namespace VectorPlusDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var config = new VectorControllerPlusConfig()
            {
                ReconnectDelay_ms = 2000
            };

            await using (var controller = new VectorControllerPlus(config))
            {
                controller.OnBehaviourReport += async report => Console.WriteLine(report.Description);
                controller.OnConnectionChanged += async state => Console.WriteLine("! " + state.ToString());

                await controller.AddBehaviourAsync(new ExterminateCubeOnSightBehaviour());

                await controller.ConnectAsync();

                bool exit = false;
                while (!exit && !Console.KeyAvailable)
                {
                    await Task.Delay(500);
                }
            }

        }
    }
}
