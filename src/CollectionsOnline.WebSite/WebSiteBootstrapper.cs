using System.Configuration;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Models;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.TinyIoc;
using NLog;
using Raven.Client.Document;
using Raven.Client.Extensions;
using Raven.Client.Indexes;

namespace CollectionsOnline.WebSite
{
    public class WebSiteBootstrapper : DefaultNancyBootstrapper
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            RegisterRavenDb(container);
        }

        private void RegisterRavenDb(TinyIoCContainer container)
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

            container.Register(documentStore);

            // Register IDocumentSession
            container.Register((c,p) => documentStore.OpenSession());

            // TODO: DELETE THIS
            IndexCreation.CreateIndexes(typeof(CombinedSearch).Assembly, documentStore);
            using (var documentSession = documentStore.OpenSession())
            {
                documentSession.Store(new CombinedSearchFacets());
                documentSession.SaveChanges();
            }
        }

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            base.ConfigureConventions(nancyConventions);

            nancyConventions.ViewLocationConventions.Clear();

            // 1 Handles: features / *modulename* / views / *viewname*
            nancyConventions.ViewLocationConventions.Add((viewName, model, viewLocationContext) => string.Concat("features/", viewLocationContext.ModuleName, "/views/", viewName));

            // 2 Handles: features / *viewname*
            nancyConventions.ViewLocationConventions.Add((viewName, model, viewLocationContext) => string.Concat("features/", viewName));

            // 3 Handles: features / shared / views/ *viewname*
            nancyConventions.ViewLocationConventions.Add((viewName, model, viewLocationContext) => string.Concat("features/shared/views/", viewName));
        }
    }
}