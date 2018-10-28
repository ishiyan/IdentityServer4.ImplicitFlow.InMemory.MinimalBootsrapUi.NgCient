using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace IdentityServer.Host.Ng
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();

            Log.Information("Stopping...");
            Log.CloseAndFlush();
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            IConfigurationRoot configurationRoot = BuildConfiguration();
            return WebHost
                .CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;
                    config
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", false, true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
                        .AddEnvironmentVariables();
                })
                .ConfigureLogging((hostingContext, loggingBuilder) =>
                {
                    // Remove console and debugger loggers provided by CreateDefaultBuilder().
                    loggingBuilder.ClearProviders();
                })
                .ConfigureServices((hostingContext, services) =>
                {
                    Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(hostingContext.Configuration)
                        .CreateLogger();
                    Log.Information("Getting the motors running...");

                    services.AddApplicationInsightsTelemetry(hostingContext.Configuration);
                })
                .UseSerilog()
                .UseConfiguration(configurationRoot)
                .UseStartup<Startup>();
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
