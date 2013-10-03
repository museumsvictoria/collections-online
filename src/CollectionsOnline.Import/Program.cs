using AutoMapper;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Import.Importers;
using IMu;
using Ninject;
using Ninject.Extensions.Conventions;
using NLog;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Extensions;
using System;
using System.Configuration;
using System.Linq;

namespace CollectionsOnline.Import
{
    class Program
    {
        private static IDocumentStore _documentStore;
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private static Session _session;
        private static volatile bool _importCancelled = false;
        private static IKernel _kernal;

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

            // Set up ninject
            _kernal = new StandardKernel();
            _kernal.Bind(x => x
                .FromAssemblyContaining(typeof(StoryImporter), typeof(Story))
                .SelectAllClasses()
                .BindAllInterfaces());
        }

        private static void Import()
        {
            var dateRun = DateTime.Now;
            var hasFailed = false;

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
                    RunDocumentImport<Story>(application.LastDataImport);
                    RunDocumentImport<Item>(application.LastDataImport);
                    RunDocumentImport<Species>(application.LastDataImport);
                    RunDocumentImport<Specimen>(application.LastDataImport);
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

        private static void RunDocumentImport<T>(DateTime dateLastRun) where T : AggregateRoot, IHideable
        {
            _log.Debug("Begining {0} import", typeof(T).Name);

            var importer = _kernal.Get<IImporter<T>>();

            var module = new Module(importer.ModuleName, _session);
            var terms = importer.Terms;

            if (dateLastRun == default(DateTime))
            {
                var hits = module.FindTerms(terms);

                _log.Debug("Finished Search. {0} Hits", hits);

                var count = 0;

                while (true)
                {
                    using (var documentSession = _documentStore.OpenSession())
                    {
                        if (_importCancelled)
                        {
                            _log.Debug("Canceling Data import");
                            return;
                        }

                        var results = module.Fetch("start", count, Constants.DataBatchSize, importer.Columns);

                        // Todo: REMOVE IMPORT LIMIT
                        if (results.Count == 0 || count == 2000)
                            break;

                        // Create and store documents
                        results.Rows
                            .Select(importer.MakeDocument)
                            .ToList()
                            .ForEach(documentSession.Store);

                        // Save any changes
                        documentSession.SaveChanges();
                        count += results.Count;
                        _log.Debug("{0} import progress... {1}/{2}", typeof(T).Name, count, hits);
                    }
                }
            }
            else
            {
                Mapper.CreateMap<T, T>().ForMember(x => x.Id, y => y.Ignore());

                terms.Add("AdmDateModified", dateLastRun.ToString("MMM dd yyyy"), ">=");

                var hits = module.FindTerms(terms);

                _log.Debug("Finished Search. {0} Hits", hits);

                var count = 0;

                while (true)
                {
                    using (var documentSession = _documentStore.OpenSession())
                    {
                        if (_importCancelled)
                        {
                            _log.Debug("Canceling Data import");
                            return;
                        }

                        var results = module.Fetch("start", count, Constants.DataBatchSize, importer.Columns);

                        if (results.Count == 0)
                            break;

                        // Update documents
                        var newDocuments = results.Rows.Select(importer.MakeDocument).ToList();
                        var existingDocuments = documentSession.Load<T>(newDocuments.Select(x => x.Id));

                        foreach (var newDocument in newDocuments)
                        {
                            var existingDocument = existingDocuments.SingleOrDefault(x => x != null && x.Id == newDocument.Id);                            

                            if (existingDocument != null)
                            {
                                // Update existing story
                                Mapper.Map(newDocument, existingDocument);
                            }
                            else
                            {
                                // Create new story
                                documentSession.Store(newDocument);
                            }
                        }

                        // Save any changes
                        documentSession.SaveChanges();
                        count += results.Count;
                        _log.Debug("{0} import progress... {1}/{2}", typeof(T).Name, count, hits);
                    }
                }
            }
        }

        private static void Dispose()
        {
            _documentStore.Dispose();
        }
    }
}