using System.Configuration;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Infrastructure;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Tasks.Infrastructure;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;
using Serilog;
using Serilog.Events;
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

            // Register DocumentStore
            container.Register(() =>
            {
                // Connect to raven db instance
                Log.Logger.Debug("Initialize Raven document store");
                var documentStore = new DocumentStore
                {
                    Url = ConfigurationManager.AppSettings["DatabaseUrl"],
                    DefaultDatabase = ConfigurationManager.AppSettings["DatabaseName"]
                }.Initialize();

                return documentStore;
            }, Lifestyle.Singleton);

            // Verify registrations
            container.Verify();

            return container;
        }
    }
}
