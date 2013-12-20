using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Models;
using Ninject.Activation;
using NLog;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Extensions;
using Raven.Client.Indexes;

namespace CollectionsOnline.WebSite.Infrastructure
{
    public class NinjectRavenDocumentStoreProvider : Provider<IDocumentStore>
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        protected override IDocumentStore CreateInstance(IContext context)
        {
            // Connect to raven db instance
            _log.Debug("Initializing document store");
            var documentStore = new DocumentStore
            {
                Url = ConfigurationManager.AppSettings["DatabaseUrl"],
                DefaultDatabase = ConfigurationManager.AppSettings["DatabaseName"]
            }.Initialize();

            // Ensure DB exists
            documentStore.DatabaseCommands.EnsureDatabaseExists(ConfigurationManager.AppSettings["DatabaseName"]);

            // Ensure we have a application document
            using (var documentSession = documentStore.OpenSession())
            {
                var application = documentSession.Load<Application>(Constants.ApplicationId);

                if (application == null)
                {
                    application = new Application();
                    documentSession.Store(application);
                }

                documentSession.SaveChanges();
            }

            // Create indexes and store facets
            IndexCreation.CreateIndexes(typeof(CombinedSearch).Assembly, documentStore);
            using (var documentSession = documentStore.OpenSession())
            {
                documentSession.Store(new CombinedSearchFacets());
                documentSession.SaveChanges();
            }

            return documentStore;
        }
    }
}