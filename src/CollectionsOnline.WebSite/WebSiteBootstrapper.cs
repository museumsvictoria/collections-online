using CollectionsOnline.Core.Factories;
using CollectionsOnline.Core.Infrastructure;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Bootstrappers.Ninject;
using Nancy.Conventions;
using Ninject.Extensions.Conventions;
using Ninject;
using NLog;
using Raven.Client;

namespace CollectionsOnline.WebSite
{
    public class WebSiteBootstrapper : NinjectNancyBootstrapper 
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        protected override void ConfigureApplicationContainer(IKernel kernel)
        {
            kernel.Bind<IDocumentStore>().ToProvider<NinjectRavenDocumentStoreProvider>().InSingletonScope();
        }

        protected override void ConfigureRequestContainer(IKernel kernel, NancyContext context)
        {
            kernel.Bind<IDocumentSession>().ToProvider<NinjectRavenDocumentSessionProvider>();            
            kernel.Bind(x => x
                .FromAssemblyContaining(typeof(WebSiteBootstrapper), typeof(SlugFactory))
                .SelectAllClasses()
                .InNamespaces(new[] { "CollectionsOnline" })
                .BindAllInterfaces());
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

        protected override void ApplicationStartup(IKernel container, IPipelines pipelines)
        {
            pipelines.OnError += (ctx, ex) =>
            {
                _log.Error(ex);

                return null;
            };
        }
    }
}