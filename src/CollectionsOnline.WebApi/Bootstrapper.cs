using System;
using System.Configuration;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Models;
using NLog;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Json;
using Nancy.Responses.Negotiation;
using Nancy.TinyIoc;
using Raven.Client.Document;
using Raven.Client.Extensions;
using Newtonsoft.Json;

namespace CollectionsOnline.WebApi
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            RegisterRavenDb(container);
            RegisterJsonNet(container);
        }

        private void RegisterJsonNet(TinyIoCContainer container)
        {
            JsonSettings.MaxJsonLength = Int32.MaxValue;

            container.Register(typeof(JsonSerializer), typeof(WebApiJsonSerializer));
        }

        protected override NancyInternalConfiguration InternalConfiguration
        {
            get
            {
                return NancyInternalConfiguration.WithOverrides((context) =>
                    {
                        // Prevent Content negotioation.
                        context.ResponseProcessors.Remove(typeof(JsonProcessor));
                        context.ResponseProcessors.Remove(typeof(XmlProcessor));
                    });
            }
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

            container.Register((c,p) => documentStore.OpenSession());
        }
    }
}