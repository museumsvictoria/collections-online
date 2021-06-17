using System;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Indexes;
using System.Configuration;
using CollectionsOnline.Core.Models;
using Ninject.Activation;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;
using Serilog;
using Serilog.Events;

namespace CollectionsOnline.Core.Infrastructure
{
    public class NinjectRavenDocumentStoreProvider : Provider<IDocumentStore>
    {
        protected override IDocumentStore CreateInstance(IContext context)
        {
            using (Log.Logger.BeginTimedOperation("Create new instance of DocumentStore", "NinjectRavenDocumentStoreProvider.CreateInstance", LogEventLevel.Debug))
            {
                // Connect to raven db instance
                Log.Logger.Debug("Initialize Raven document store");
                var documentStore = new DocumentStore
                {
                    Url = ConfigurationManager.AppSettings["DatabaseUrl"],
                    DefaultDatabase = ConfigurationManager.AppSettings["DatabaseName"]
                }.Initialize();

                // Ensure DB exists
                Log.Logger.Debug("Ensure the Database {DatabaseName} Exists", ConfigurationManager.AppSettings["DatabaseName"]);
                documentStore.DatabaseCommands.GlobalAdmin.EnsureDatabaseExists(ConfigurationManager.AppSettings["DatabaseName"]);

                // Create core indexes and store facets
                Log.Logger.Debug("Ensure Core indexes and facets are created");
                IndexCreation.CreateIndexes(typeof(CombinedIndex).Assembly, documentStore);
                using (var documentSession = documentStore.OpenSession())
                {
                    documentSession.Store(new CombinedFacets());
                    documentSession.SaveChanges();
                }

                // Ensure we have a application document
                Log.Logger.Debug("Ensure Application document has been created");
                using (var documentSession = documentStore.OpenSession())
                {
                    var application = documentSession.Load<Application>(Constants.ApplicationId);

                    if (application == null)
                    {
                        Log.Logger.Information("Creating new application document store");
                        application = new Application();
                        documentSession.Store(application);
                    }

                    documentSession.SaveChanges();
                }

                documentStore.JsonRequestFactory.RequestTimeout = TimeSpan.FromSeconds(10);

                return documentStore; 
            }
        }
    }
}