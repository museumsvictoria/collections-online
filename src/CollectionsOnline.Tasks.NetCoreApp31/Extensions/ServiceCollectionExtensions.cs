using System.Linq;
using CollectionsOnline.Tasks.NetCoreApp31.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Raven.Client.Document;
using Serilog;

namespace CollectionsOnline.Tasks.NetCoreApp31.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTasks(this IServiceCollection services)
        {
            // Add all implementations of ITask
            services.TryAddEnumerable(System.AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(ITask).IsAssignableFrom(type) && type.IsClass)
                .Select(type => ServiceDescriptor.Singleton(typeof(ITask), type)));

            return services;
        }

        public static IServiceCollection AddRavenDb(this IServiceCollection services, Settings settings)
        {
            // Add Raven DocumentStore
            services.TryAddSingleton(sp =>
            {
                // Connect to raven db instance
                Log.Logger.Debug("Initialize Raven document store");
                var documentStore = new DocumentStore
                {
                    Url = settings.DatabaseUrl,
                    DefaultDatabase = settings.DatabaseName
                }.Initialize();
                
                return documentStore;
            });

            return services;
        }
    }
}