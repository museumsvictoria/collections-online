using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Infrastructure;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Import.Factories;
using CollectionsOnline.Import.Imports;
using CollectionsOnline.Import.Infrastructure;
using IMu;
using Ninject;
using Ninject.Extensions.Conventions;
using NLog;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Extensions;
using System;
using System.Configuration;

namespace CollectionsOnline.Import
{
    class Program
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private static IKernel _kernel;

        public static volatile bool ImportCanceled = false;

        static void Main(string[] args)
        {
            // Set up Ctrl+C handling TODO: Add ability to cancel import
            //Console.CancelKeyPress += (sender, eventArgs) =>
            //{
            //    eventArgs.Cancel = true;
            //    ImportCanceled = true;
            //};

            _kernel = CreateKernel();

            _kernel.Get<ImportRunner>().Run();
        }

        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();

            RegisterServices(kernel);

            return kernel;
        }

        private static void RegisterServices(IKernel kernel)
        {
            // Raven Db Bindings
            kernel.Bind<IDocumentStore>().ToProvider<NinjectRavenDocumentStoreProvider>().InSingletonScope();

            // Imu Bindings
            kernel.Bind<Session>().ToProvider<NinjectImuSessionProvider>().InSingletonScope();

            // Bind Imports
            kernel.Bind<IImport>().To<ImuImport<Item>>();
            kernel.Bind<IImport>().To<ImuImport<Species>>();
            kernel.Bind<IImport>().To<ImuImport<Specimen>>();
            kernel.Bind<IImport>().To<ImuImport<Story>>();

            // Bind the rest
            kernel.Bind(x => x
                .FromAssemblyContaining(typeof(Program), typeof(Constants))
                .SelectAllClasses()
                .BindAllInterfaces());
        }
    }
}