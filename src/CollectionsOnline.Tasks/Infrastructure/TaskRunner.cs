using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;

namespace CollectionsOnline.Tasks.Infrastructure
{
    public class TaskRunner
    {
        private readonly IEnumerable<ITask> _tasks;

        public TaskRunner(IEnumerable<ITask> tasks)
        {
            _tasks = tasks;
        }

        public void RunAllTasks()
        {
            var tasksHaveFailed = false;

            using (Log.Logger.BeginTimedOperation("Collections task runner starting", "TaskRunner.RunAllTasks"))
            {
                try
                {
                    // Run all tasks
                    foreach (var task in _tasks.OrderBy(x => x.Order))
                    {
                        if (Program.TasksCanceled)
                            break;

                        task.Run();
                    }
                }
                catch (Exception ex)
                {
                    tasksHaveFailed = true;
                    Log.Logger.Error(ex, "Exception occured running tasks");
                }

                if (Program.TasksCanceled || tasksHaveFailed)
                    Log.Logger.Information("Collections task runner has been stopped prematurely {@Reason}", new { Program.TasksCanceled, tasksHaveFailed });
                else
                    Log.Logger.Information("All Collections tasks finished successfully");
            }

            Log.CloseAndFlush();
        }
    }
}
