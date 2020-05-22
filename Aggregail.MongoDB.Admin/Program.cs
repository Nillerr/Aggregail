using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Hosting;

namespace Aggregail.MongoDB.Admin
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .ConfigureAppConfiguration(config =>
                        {
                            var environmentVariables = config.Sources
                                .OfType<EnvironmentVariablesConfigurationSource>()
                                .First();

                            environmentVariables.Prefix = "AGGREGAIL_";
                        })
                        .UseStartup<Startup>();
                });
    }
}