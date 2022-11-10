using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace CollectionsOnline.Tasks.Infrastructure;

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
        Log.Logger.Information("TaskRunner starting up");

        try
        {
            // Run all tasks
            foreach (var task in _tasks.Where(x => x.Enabled).OrderBy(x => x.Order))
            {
                stoppingToken.ThrowIfCancellationRequested();

                await task.Run(stoppingToken);
            }
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, "Exception occured running tasks");

            throw;
        }

        if (!stoppingToken.IsCancellationRequested)
            Log.Logger.Information("All Collections tasks finished successfully");

        _appLifetime.StopApplication();
    }
}