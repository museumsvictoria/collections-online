using System;
using Ninject.Activation;
using Raven.Client;

namespace CollectionsOnline.Core.Infrastructure
{
    public class NinjectRavenDocumentSessionProvider : Provider<IDocumentSession>
    {
        private readonly IDocumentStore _documentStore;

        public NinjectRavenDocumentSessionProvider(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        protected override IDocumentSession CreateInstance(IContext context)
        {
            var documentSession = _documentStore.OpenSession();

            documentSession.Advanced.DocumentStore.AggressivelyCacheFor(TimeSpan.FromMinutes(10));

            return documentSession;
        }
    }
}