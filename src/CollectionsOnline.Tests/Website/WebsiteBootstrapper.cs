using System;
using CollectionsOnline.Core.Factories;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.WebSite.Factories;
using CollectionsOnline.WebSite.Infrastructure;
using CollectionsOnline.WebSite.Transformers;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Bootstrappers.Ninject;
using Nancy.Json;
using Newtonsoft.Json;
using Ninject;
using Ninject.Extensions.Conventions;
using Raven.Client;
using Raven.Client.Indexes;

namespace CollectionsOnline.Tests.Website
{
    public class WebsiteBootstrapper : NinjectNancyBootstrapper
    {
        private readonly IDocumentStore _documentStore;

        public WebsiteBootstrapper(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        protected override void ConfigureApplicationContainer(IKernel kernel)
        {
            kernel.Bind<IDocumentStore>().ToConstant(_documentStore);
            kernel.Bind<IHomeHeroUriFactory>().To<HomeHeroUriFactory>().InSingletonScope();
            kernel.Bind<JsonSerializer>().To<ApiJsonSerializer>();

            // Create indexes from website and core
            IndexCreation.CreateIndexes(typeof(CombinedIndex).Assembly, _documentStore);
            IndexCreation.CreateIndexes(typeof(ItemViewTransformer).Assembly, _documentStore);
        }

        protected override void ConfigureRequestContainer(IKernel kernel, NancyContext context)
        {
            kernel.Bind<IDocumentSession>().ToMethod(x => _documentStore.OpenSession());
            kernel.Bind(x => x
                .FromAssemblyContaining(typeof(WebSiteBootstrapper), typeof(SlugFactory))
                .SelectAllClasses()                
                .InNamespaces(new[] { "CollectionsOnline" })
                .Excluding<HomeHeroUriFactory>()
                .BindAllInterfaces());
        }

        protected override void ApplicationStartup(IKernel container, IPipelines pipelines)
        {
            JsonSettings.MaxJsonLength = Int32.MaxValue;
            AutomapperConfig.Initialize();
        }
    }
}