using System;
using System.Configuration;
using CollectionsOnline.Core.DomainModels;
using CollectionsOnline.Core.Config;
using IMu;
using NLog;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Extensions;

namespace CollectionsOnline.Import
{
    class Program
    {
        private static IDocumentStore _documentStore;
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private static Session _session;
        private static volatile bool _importCancelled = false;

        static void Main(string[] args)
        {
            Initialize();

            Import();

            Dispose();
        }

        private static void Initialize()
        {
            // Connect to raven db instance
            _log.Debug("Initializing document store");
            _documentStore = new DocumentStore
            {
                Url = ConfigurationManager.AppSettings["DatabaseUrl"],
                DefaultDatabase = ConfigurationManager.AppSettings["DatabaseName"]
            }.Initialize();

            // Ensure DB exists
            _documentStore.DatabaseCommands.EnsureDatabaseExists(ConfigurationManager.AppSettings["DatabaseName"]);

            // Connect to Imu
            _log.Debug("Connecting to Imu server: {0}:{1}", ConfigurationManager.AppSettings["EmuServerHost"], ConfigurationManager.AppSettings["EmuServerPort"]);
            _session = new Session(ConfigurationManager.AppSettings["EmuServerHost"], int.Parse(ConfigurationManager.AppSettings["EmuServerPort"]));
            _session.Connect();

            // Ensure we have a application document
            using (var documentSession = _documentStore.OpenSession())
            {
                var application = documentSession.Load<Application>(Constants.ApplicationId);

                if (application == null)
                {
                    application = new Application();
                    documentSession.Store(application);
                }

                documentSession.SaveChanges();
            }

            // Set up Ctrl+C handling
            Console.CancelKeyPress += (sender, eventArgs) =>
                {
                    eventArgs.Cancel = true;
                    _importCancelled = true;
                };
        }

        private static void Import()
        {
            var dateRun = DateTime.Now;
            bool hasFailed = false;

            try
            {
                _log.Debug("Data Import begining");

                var documentSession = _documentStore.OpenSession();
                var application = documentSession.Load<Application>(Constants.ApplicationId);

                if (application == null)
                    throw new Exception("Application not found");

                if (!application.DataImportRunning)
                {
                    application.RunDataImport();
                    documentSession.SaveChanges();
                    documentSession.Dispose();

                    // Run Imports
                }
            }
            catch (Exception exception)
            {
                hasFailed = true;
                _log.Debug(exception.ToString);
            }

            using (var documentSession = _documentStore.OpenSession())
            {
                var application = documentSession.Load<Application>(Constants.ApplicationId);

                if (_importCancelled || hasFailed)
                {
                    _log.Debug("Data import finished (cancelled or failed)");
                    application.DataImportFinished();
                }
                else
                {
                    _log.Debug("Data import finished succesfully");
                    application.DataImportSuccess(dateRun);
                }

                documentSession.SaveChanges();
            }
        }

        private static void Dispose()
        {
            _documentStore.Dispose();
        }
    }
}