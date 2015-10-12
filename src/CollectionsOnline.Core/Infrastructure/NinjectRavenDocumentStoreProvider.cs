using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Indexes;
using System.Configuration;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Core.Utilities;
using Ninject.Activation;
using NLog;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;

namespace CollectionsOnline.Core.Infrastructure
{
    public class NinjectRavenDocumentStoreProvider : Provider<IDocumentStore>
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        protected override IDocumentStore CreateInstance(IContext context)
        {
            // Connect to raven db instance
            IDocumentStore documentStore;
            using (new StopwatchTimer("Initialized raven document store", _log))
            {
                documentStore = new DocumentStore
                {
                    Url = ConfigurationManager.AppSettings["DatabaseUrl"],
                    DefaultDatabase = ConfigurationManager.AppSettings["DatabaseName"]
                }.Initialize();
            }

            // Ensure DB exists
            documentStore.DatabaseCommands.GlobalAdmin.EnsureDatabaseExists(ConfigurationManager.AppSettings["DatabaseName"]);

            // Create core indexes and store facets
            using (new StopwatchTimer("Ensured Core indexes and facets have been created", _log))
            {
                IndexCreation.CreateIndexes(typeof (CombinedIndex).Assembly, documentStore);

                using (var documentSession = documentStore.OpenSession())
                {
                    documentSession.Store(new CombinedFacets());
                    documentSession.SaveChanges();
                }
            }

            // Ensure we have a application document
            using (new StopwatchTimer("Ensured Application document has been created", _log))
            {
                using (var documentSession = documentStore.OpenSession())
                {
                    var application = documentSession.Load<Application>(Constants.ApplicationId);

                    if (application == null)
                    {
                        _log.Debug("Creating new application document store");
                        application = new Application();
                        documentSession.Store(application);
                    }

                    documentSession.SaveChanges();
                }
            }

            return documentStore;
        }
    }
}