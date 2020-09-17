using System;
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
        private readonly IEnumerable<ITask> _tasks;

        public TaskRunner(
            IHostApplicationLifetime appLifetime,
            IEnumerable<ITask> tasks)
        {
            _appLifetime = appLifetime;
            _tasks = tasks;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log.Logger.Information("ExecuteAsync has been called.");

            try
            {
                // Run all tasks
                foreach (var task in _tasks.OrderBy(x => x.Order))
                {
                    if(stoppingToken.IsCancellationRequested)
                        break;
                    
                    await task.Run(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                if (ex is OperationCanceledException)
                {
                    Log.Logger.Information("Collections task runner has been cancelled prematurely");
                }
                else
                {
                    Log.Logger.Error(ex, "Exception occured running tasks");                    
                }

                throw;
            }
            
            if(!stoppingToken.IsCancellationRequested)
                Log.Logger.Information("All Collections tasks finished successfully");

            _appLifetime.StopApplication();
        }
    }
}