using System;
using System.IO;
using System.Threading.Tasks;
using CollectionsOnline.Tasks.NetCoreApp31.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace CollectionsOnline.Tasks.NetCoreApp31
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
                Log.Fatal((Exception) eventArgs.ExceptionObject, "Unhandled Exception occured in CollectionsOnline Tasks");

            try
            {
                Log.Information("CollectionsOnline Tasks starting up...");

                await CreateHostBuilder(args)
                    .Build()
                    .RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "CollectionsOnline Tasks startup failed...");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host
                .CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddHostedService<TaskRunnerHostedService>();
                })
                .UseConsoleLifetime()
                .UseSerilog();

        private static IConfiguration Configuration => new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile(
                        $"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production"}.json",
                        optional: true)
                    .AddEnvironmentVariables()
                    .Build();
    }
}
