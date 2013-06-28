using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using CollectionsOnline.Core.DomainModels;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Import.Factories;
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
                    RunStoryImport(application.LastDataImport);
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

        private static void RunStoryImport(DateTime dateLastRun)
        {            
            _log.Debug("Begining import");

            var storyFactory = new StoryFactory();
            var storyDocuments = new List<Story>();
            var module = new Module(storyFactory.MakeModuleName(), _session);
            var terms = storyFactory.MakeTerms();

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

                        var results = module.Fetch("start", count, Constants.DataBatchSize, storyFactory.MakeColumns());

                        if (results.Count == 0)
                            break;

                        // Create documents
                        storyDocuments.AddRange(results.Rows.Select(storyFactory.MakeStory));

                        count += results.Count;
                        _log.Debug("Story import progress... {0}/{1}", count, hits);
                    }
                }
            }
            else
            {
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

                        var results = module.Fetch("start", count, Constants.DataBatchSize, storyFactory.MakeColumns());

                        if (results.Count == 0)
                            break;

                        // Update documents
                        var existingStoryDocuments = documentSession.Load<Story>(results.Rows.Select(x => @"Story/" + x.GetString("irn")));

                        foreach (var map in results.Rows)
                        {
                            var existingStory = existingStoryDocuments.SingleOrDefault(x => x != null && x.Id == @"Story/" + map.GetString("irn"));

                            if (existingStory != null)
                            {
                                // Update existing story
                            }
                            else
                            {
                                // Create new story
                                storyDocuments.Add(storyFactory.MakeStory(map));
                            }
                        }

                        count += results.Count;
                        _log.Debug("Story import progress... {0}/{1}", count, hits);
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