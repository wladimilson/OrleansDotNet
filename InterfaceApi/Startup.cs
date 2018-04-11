using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orleans;
using Orleans.Runtime.Configuration;
using Orleans.Hosting;
using System.Net;
using Orleans.Configuration;
using OrleansDotNet.GraosInterfaces;

namespace InterfaceApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddSingleton<IClusterClient>(provider =>
            {
                var client = new ClientBuilder()
                            .UseLocalhostClustering()
                            .Configure<ClusterOptions>(options =>
                            {
                                options.ClusterId = "OrleansDotNet-cluster";
                                options.ServiceId = "OrleansDotNet";
                            })
                            .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(IGraoContador).Assembly).WithReferences())
                            .ConfigureLogging(logging => logging.AddConsole())
                            .Build();

                StartClientWithRetries(client).Wait();

                return client;
            });
        }

        private static async Task StartClientWithRetries(IClusterClient client)
        {
            for (var i=0; i<5; i++)
            {
                try
                {
                    await client.Connect();
                    return;
                }
                catch(Exception)
                { }
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            app.UseStaticFiles();
        }
    }
}
