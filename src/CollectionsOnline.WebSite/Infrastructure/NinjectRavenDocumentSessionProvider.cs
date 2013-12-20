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
    public class NinjectRavenDocumentSessionProvider : Provider<IDocumentSession>
    {
        private readonly IDocumentStore _documentStore;

        public NinjectRavenDocumentSessionProvider(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        protected override IDocumentSession CreateInstance(IContext context)
        {
            return _documentStore.OpenSession();
        }
    }
}