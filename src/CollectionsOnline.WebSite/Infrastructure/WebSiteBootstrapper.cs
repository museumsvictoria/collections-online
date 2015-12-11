using System;
using System.Collections.Generic;
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
using Raven.Client;
using Raven.Client.Indexes;
using Serilog;
using StackExchange.Profiling;
using StackExchange.Profiling.RavenDb;

namespace CollectionsOnline.WebSite.Infrastructure
{
    public class WebSiteBootstrapper : NinjectNancyBootstrapper 
    {
        protected override void ConfigureApplicationContainer(IKernel kernel)
        {
            SerilogConfig.Initialize();

            using (Log.Logger.BeginTimedOperation("Configure application scope dependencies", "WebSiteBootstrapper.ConfigureApplicationContainer"))
            {
                kernel.Bind<IDocumentStore>().ToProvider<NinjectRavenDocumentStoreProvider>().InSingletonScope();
                kernel.Bind<IHomeHeroUriFactory>().To<HomeHeroUriFactory>().InSingletonScope();
                kernel.Bind<JsonSerializer>().To<ApiJsonSerializer>();

                var documentStore = kernel.Get<IDocumentStore>();

                // Register view transformers from website 
                Log.Logger.Debug("Ensure Website indexes and transformers are created");
                IndexCreation.CreateIndexes(typeof (ItemViewTransformer).Assembly, documentStore);

                // Initialize raven miniprofiler
                MiniProfilerRaven.InitializeFor(documentStore);
            }
        }

        protected override void ConfigureRequestContainer(IKernel kernel, NancyContext context)
        {
            kernel.Bind<IDocumentSession>().ToProvider<NinjectRavenDocumentSessionProvider>();
            kernel.Bind(x => x
                .FromAssemblyContaining(typeof (WebSiteBootstrapper), typeof (SlugFactory))
                .SelectAllClasses()
                .InNamespaces(new[] {"CollectionsOnline"})
                .Excluding<HomeHeroUriFactory>()
                .BindAllInterfaces());
        }

        protected override void ApplicationStartup(IKernel container, IPipelines pipelines)
        {
            using (Log.Logger.BeginTimedOperation("Application starting up", "WebSiteBootstrapper.ApplicationStartup"))
            {
                JsonSettings.MaxJsonLength = Int32.MaxValue;

                pipelines.OnError += (ctx, ex) =>
                {
                    Log.Logger.Fatal(ex, "Unhandled Exception occured in Nancy pipeline {Url}", ctx.Request.Url);

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

                    if (MiniProfiler.Current != null)
                        Log.Logger.Debug(MiniProfiler.Current.RenderPlainText().RemoveLineBreaks());
                };

                // Automapper configuration
                AutomapperConfig.Initialize();

                // See https://github.com/NancyFx/Nancy/issues/2052
                StaticConfiguration.DisableErrorTraces = true;
            }
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