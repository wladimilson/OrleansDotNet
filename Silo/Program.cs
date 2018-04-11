using System;
using Orleans;
using Orleans.Runtime.Configuration;
using Orleans.Hosting;
using Orleans.Configuration;
using System.Threading.Tasks;
using OrleansDotNet.Graos;
using Microsoft.Extensions.Logging;
using System.Runtime.Loader;
using System.Threading;
using System.Net;

namespace OrleansDotNet.Silo
{
    class Program
    {
        private static ISiloHost silo;
        private static readonly ManualResetEvent siloStopped = new ManualResetEvent(false);

        static void Main(string[] args)
        {

            silo = new SiloHostBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "OrleansDotNet-cluster";
                    options.ServiceId = "OrleansDotNet";
                })
                .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback)
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(GraoContador).Assembly).WithReferences())
                .ConfigureLogging(logging => logging.AddConsole())
                .Build();

            Task.Run(StartSilo);

            AssemblyLoadContext.Default.Unloading += context =>
            {
                Task.Run(StopSilo);
                siloStopped.WaitOne();
            };

            siloStopped.WaitOne();

        }

        private static async Task StartSilo()
        {
            await silo.StartAsync();
            Console.WriteLine("Silo iniciado");
        }

        private static async Task StopSilo()
        {
            await silo.StopAsync();
            Console.WriteLine("Silo parado");
            siloStopped.Set();
        }
    }
}
