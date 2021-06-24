using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Steeltoe.Management.Endpoint;
using Steeltoe.Extensions.Configuration.CloudFoundry;
using Microsoft.AspNetCore;
using Steeltoe.Common.Hosting;

namespace SpringBootAdmin
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = WebHost.CreateDefaultBuilder(args)
                                .AddCloudFoundryConfiguration()
                                .AddAllActuators()
                                .UseCloudHosting()
                                .UseStartup<Startup>()
                                .Build();

            host.Run();
            
            // BuildWebHost(args).Run();
        }

        public static IHost BuildWebHost(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .AddCloudFoundryConfiguration()
                .ConfigureWebHost(configure =>
                {
                    // configure.UseStartup<Startup>().UseKestrel();
                    configure.UseUrls("https://sbo-integration.apps.pcfone.io");
                })
               .AddAllActuators(endpoints => endpoints.RequireAuthorization("actuators.read"))
               .Build();
    }
}
