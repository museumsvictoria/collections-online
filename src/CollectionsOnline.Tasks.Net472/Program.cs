using CollectionsOnline.Tasks.Net472.Config;
using CollectionsOnline.Tasks.Net472.Infrastructure;

namespace CollectionsOnline.Tasks.Net472
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
