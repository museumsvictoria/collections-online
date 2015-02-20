using System;
using AutoMapper;
using CollectionsOnline.Core.Factories;
using CollectionsOnline.Core.Infrastructure;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebApi.Models;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Bootstrappers.Ninject;
using Nancy.Json;
using Newtonsoft.Json;
using Ninject;
using Ninject.Extensions.Conventions;
using NLog;
using Raven.Client;

namespace CollectionsOnline.WebApi
{
    public class WebApiBootstrapper : NinjectNancyBootstrapper
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        protected override void ConfigureApplicationContainer(IKernel kernel)
        {
            kernel.Bind<IDocumentStore>().ToProvider<NinjectRavenDocumentStoreProvider>().InSingletonScope();
            kernel.Bind<JsonSerializer>().To<WebApiJsonSerializer>();
        }

        protected override void ConfigureRequestContainer(IKernel kernel, NancyContext context)
        {
            kernel.Bind<IDocumentSession>().ToProvider<NinjectRavenDocumentSessionProvider>();
            kernel.Bind(x => x
                .FromAssemblyContaining(typeof(WebApiBootstrapper), typeof(SlugFactory))
                .SelectAllClasses()
                .InNamespaces(new[] { "CollectionsOnline" })
                .BindAllInterfaces());
        }

        protected override void ApplicationStartup(IKernel kernal, IPipelines pipelines)
        {            
            JsonSettings.MaxJsonLength = Int32.MaxValue;

            pipelines.OnError += (ctx, ex) =>
            {
                _log.Error(ex);

                return null;
            };

            // Automapper configuration
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Article, ArticleViewModel>();
                cfg.CreateMap<Item, ItemViewModel>();
            });
        }
    }
}