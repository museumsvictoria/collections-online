using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace CollectionsOnline.Tasks.NetCoreApp31.Infrastructure
{
    public class TaskRunnerHostedService : IHostedService
    {
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly IHostEnvironment _env;
        private readonly IConfiguration _configuration;

        public TaskRunnerHostedService(
            IHostApplicationLifetime appLifetime,
            IHostEnvironment env,
            IConfiguration configuration)
        {
            _appLifetime = appLifetime;
            _configuration = configuration;
            _env = env;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _appLifetime.ApplicationStarted.Register(OnStarted);
            _appLifetime.ApplicationStopping.Register(OnStopping);
            _appLifetime.ApplicationStopped.Register(OnStopped);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void OnStarted()
        {
            Log.Logger.Information("OnStarted has been called.");
            // Perform post-startup activities here
        }

        private void OnStopping()
        {
            Log.Logger.Information("OnStopping has been called.");

            // Perform on-stopping activities here
        }

        private void OnStopped()
        {
            Log.Logger.Information("OnStopped has been called.");

            // Perform post-stopped activities here
        }
    }
}