global using CollectionsOnline.Tasks.Infrastructure;
global using CollectionsOnline.Tasks;
global using Serilog;
global using System;
using CollectionsOnline.Tasks.Extensions;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog.Sinks.SystemConsole.Themes;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", false, true)
    .AddJsonFile(
        $"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production"}.json",
        true)
    .AddEnvironmentVariables()
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .WriteTo.Console(theme: AnsiConsoleTheme.Code)
    .WriteTo.File(Path.Combine(AppContext.BaseDirectory, "logs/log.txt"), rollingInterval: RollingInterval.Day)
    .Enrich.WithEnvironmentName()
    .Enrich.WithMachineName()
    .CreateLogger();

try
{
    Log.Information("CollectionsOnline Tasks starting up...");

    // Configure host
    var host = Host.CreateDefaultBuilder(args)
        .ConfigureServices((context, services) =>
        {
            var configSection = context.Configuration.GetSection(AppSettings.SECTION_NAME);

            services
                .Configure<AppSettings>(configSection)
                .Configure<HostOptions>(opts => opts.ShutdownTimeout = TimeSpan.FromSeconds(20))
                .AddHostedService<TaskRunner>()
                .AddRavenDb(configSection.Get<AppSettings>())
                .AddTasks();
        })
        .UseConsoleLifetime()
        .UseSerilog()
        .Build();

    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "CollectionsOnline Tasks startup failed...");
}
finally
{
    Log.CloseAndFlush();
}