using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace CollectionsOnline.Tasks.NetCoreApp31.Infrastructure
{
    public class TaskRunner : BackgroundService
    {
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly IHostEnvironment _env;
        private readonly IConfiguration _configuration;
        private readonly IEnumerable<ITask> _tasks;

        public TaskRunner(
            IHostApplicationLifetime appLifetime,
            IHostEnvironment env,
            IConfiguration configuration,
            IEnumerable<ITask> tasks)
        {
            _appLifetime = appLifetime;
            _configuration = configuration;
            _env = env;
            _tasks = tasks;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log.Logger.Information("ExecuteAsync has been called.");
            
            // Run all tasks
            foreach (var task in _tasks.OrderBy(x => x.Order))
            {
                task.Run();
            }
            
            _appLifetime.StopApplication();
            
            return Task.CompletedTask;
        }
    }
}