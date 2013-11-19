using AutoMapper;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Import.Importers;
using IMu;
using NLog;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Extensions;
using System;
using System.Configuration;
using System.Linq;
using TinyIoC;

namespace CollectionsOnline.Import
{
    class Program
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private static TinyIoCContainer _container;
        public static volatile bool ImportCanceled = false;

        static void Main(string[] args)
        {
            // Set up Ctrl+C handling
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                ImportCanceled = true;
            };

            _container = TinyIoCContainer.Current;
            _container.AutoRegister();

            RegisterRavenDb();
            RegisterImuSession();
            RegisterImports();

            _container.Resolve<ImportRunner>().Run();
        }

        private static void RegisterRavenDb()
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

            _container.Register(documentStore);
        }

        private static void RegisterImuSession()
        {
            // Connect to Imu
            _log.Debug("Connecting to Imu server: {0}:{1}", ConfigurationManager.AppSettings["EmuServerHost"], ConfigurationManager.AppSettings["EmuServerPort"]);
            var session = new Session(ConfigurationManager.AppSettings["EmuServerHost"], int.Parse(ConfigurationManager.AppSettings["EmuServerPort"]));
            session.Connect();

            _container.Register(session);
        }

        private static void RegisterImports()
        {
            _container.RegisterMultiple<IImport<EmuAggregateRoot>>(new[]
                {
                    typeof(ItemImport), 
                    //typeof(SpeciesImport), 
                    //typeof(SpecimenImport), 
                    //typeof(StoryImport)
                });
        }
    }
}