using CollectionsOnline.Tasks.Config;
using CollectionsOnline.Tasks.Infrastructure;

namespace CollectionsOnline.Tasks
{
    class Program
    {
        public static volatile bool TasksCanceled = false;

        static void Main(string[] args)
        {
            // Configure serilog
            SerilogConfig.Initialize();

            // Configure Program
            ProgramConfig.Initialize();

            // Configure ioc
            var container = ContainerConfig.Initialize();

            // Begin task runner
            container.GetInstance<TaskRunner>().RunAllTasks();
        }
    }
}
