using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using TinyMessenger;
using VectorPlus.Web.Service;

namespace VectorPlus.Web
{
    public class Program
    {
        public static TinyMessengerHub Hub { get; private set; }

        public static void Main(string[] args)
        {
            Hub = new TinyMessengerHub();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<VectorPlusWebStartup>();
                    webBuilder.UseUrls("http://*:5000"); // port 8080 on every interface
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<VectorPlusBackgroundService>();
                    services.AddSingleton<IHostedService, VectorPlusBackgroundService>(serviceProvider => serviceProvider.GetService<VectorPlusBackgroundService>());
                });
    }
}
