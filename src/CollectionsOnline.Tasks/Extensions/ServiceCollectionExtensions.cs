using System.Linq;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Raven.Client.Document;

namespace CollectionsOnline.Tasks.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTasks(this IServiceCollection services)
        {
            // Add all implementations of ITask
            services.TryAddEnumerable(System.AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(ITask).IsAssignableFrom(type) && type.IsClass)
                .Select(type => ServiceDescriptor.Transient(typeof(ITask), type)));

            return services;
        }

        public static IServiceCollection AddRavenDb(this IServiceCollection services, AppSettings appSettings)
        {
            // Add Raven DocumentStore
            services.TryAddSingleton(sp =>
            {
                // Connect to raven db instance
                Log.Logger.Debug("Initialize Raven document store");
                var documentStore = new DocumentStore
                {
                    Url = appSettings.DatabaseUrl,
                    DefaultDatabase = appSettings.DatabaseName,
                    Credentials = new NetworkCredential(appSettings.DatabaseUserName,
                        appSettings.DatabasePassword,
                        appSettings.DatabaseDomain)
                }.Initialize();
                
                return documentStore;
            });

            return services;
        }
    }
}