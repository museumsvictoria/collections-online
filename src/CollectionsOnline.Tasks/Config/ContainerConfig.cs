using CollectionsOnline.Tasks.Infrastructure;
using SimpleInjector;

namespace CollectionsOnline.Tasks.Config
{
    public static class ContainerConfig
    {
        public static Container Initialize()
        {
            var container = new Container();

            // Register task runner
            container.Register<TaskRunner>();

            // Register all tasks
            container.Collection.Register<ITask>(typeof(ITask).Assembly);

            // Verify registrations
            container.Verify();

            return container;
        }
    }
}
