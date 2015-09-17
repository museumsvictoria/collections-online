using System;
using System.Collections.Generic;
using System.Web;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Core.Factories;
using CollectionsOnline.Core.Infrastructure;
using CollectionsOnline.WebSite.Factories;
using CollectionsOnline.WebSite.Transformers;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Bootstrappers.Ninject;
using Nancy.Conventions;
using Nancy.Json;
using Newtonsoft.Json;
using Ninject;
using Ninject.Extensions.Conventions;
using NLog;
using Raven.Client;
using Raven.Client.Indexes;
using StackExchange.Profiling;
using StackExchange.Profiling.RavenDb;

namespace CollectionsOnline.WebSite.Infrastructure
{
    public class WebSiteBootstrapper : NinjectNancyBootstrapper 
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        protected override void ConfigureApplicationContainer(IKernel kernel)
        {
            kernel.Bind<IDocumentStore>().ToProvider<NinjectRavenDocumentStoreProvider>().InSingletonScope();
            kernel.Bind<IHomeHeroUriFactory>().To<HomeHeroUriFactory>().InSingletonScope();
            kernel.Bind<JsonSerializer>().To<ApiJsonSerializer>();

            var documentStore = kernel.Get<IDocumentStore>();

            // Register view transformers
            IndexCreation.CreateIndexes(typeof(ItemViewTransformer).Assembly, documentStore);

            // Initialize raven miniprofiler
            MiniProfilerRaven.InitializeFor(documentStore);
        }

        protected override void ConfigureRequestContainer(IKernel kernel, NancyContext context)
        {
            kernel.Bind<IDocumentSession>().ToProvider<NinjectRavenDocumentSessionProvider>();
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

            pipelines.OnError += (ctx, ex) =>
            {
                _log.Error(ex);

                return null;
            };

            pipelines.BeforeRequest += ctx =>
            {
                MiniProfiler.Start();

                return null;
            };

            pipelines.AfterRequest += ctx =>
            {
                MiniProfiler.Stop();

                // Remove un-needed mini profiler header
                HttpContext.Current.Response.Headers.Remove("X-MiniProfiler-Ids");

                _log.Trace(MiniProfiler.Current.RenderPlainText().RemoveLineBreaks());
            };

            // Automapper configuration
            AutomapperConfig.Initialize();
        }

        protected override void ConfigureConventions(NancyConventions conventions)
        {
            base.ConfigureConventions(conventions);            

            conventions.StaticContentsConventions.Clear();

            conventions.StaticContentsConventions.Add(CustomStaticContentConventionBuilder.AddFile("/robots.txt", "/robots.txt"));
            conventions.StaticContentsConventions.Add(CustomStaticContentConventionBuilder.AddFile("/opensearch.xml", "/opensearch.xml"));
            conventions.StaticContentsConventions.Add(CustomStaticContentConventionBuilder.AddDirectory("/content/fonts", new Dictionary<string, string> { { "Cache-Control", string.Format("public, max-age={0}", (int)TimeSpan.FromDays(1).TotalSeconds) } }));
            conventions.StaticContentsConventions.Add(CustomStaticContentConventionBuilder.AddDirectory("/content/img", new Dictionary<string, string> { { "Cache-Control", string.Format("public, max-age={0}", (int)TimeSpan.FromDays(7).TotalSeconds) } }));
            conventions.StaticContentsConventions.Add(CustomStaticContentConventionBuilder.AddDirectory("/content/static", new Dictionary<string, string> { { "Cache-Control", string.Format("public, max-age={0}", (int)TimeSpan.FromDays(365).TotalSeconds) } }));
            conventions.StaticContentsConventions.Add(CustomStaticContentConventionBuilder.AddDirectory("/content/media", new Dictionary<string, string> { { "Cache-Control", string.Format("public, max-age={0}", (int)TimeSpan.FromDays(1).TotalSeconds) } }));            
            conventions.StaticContentsConventions.Add(CustomStaticContentConventionBuilder.AddDirectory("/", contentPath: "/sitemaps"));
        }
    }
}