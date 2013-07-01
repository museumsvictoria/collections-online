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
using Ninject;
using Ninject.Extensions.Conventions;

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

            // Set up niject
            _kernal = new StandardKernel();
            _kernal.Bind(x => x
                .FromAssemblyContaining(typeof(StoryDocumentFactory), typeof(Story))
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

            var storyDocumentFactory = _kernal.Get<StoryDocumentFactory>();
            var module = new Module(storyDocumentFactory.MakeModuleName(), _session);
            var terms = storyDocumentFactory.MakeTerms();

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

                        var results = module.Fetch("start", count, Constants.DataBatchSize, storyDocumentFactory.MakeColumns());

                        if (results.Count == 0)
                            break;

                        // Create and store documents
                        results.Rows
                            .Select(storyDocumentFactory.MakeDocument)
                            .ToList()
                            .ForEach(documentSession.Store);

                        // Save any changes
                        documentSession.SaveChanges();
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

                        var results = module.Fetch("start", count, Constants.DataBatchSize, storyDocumentFactory.MakeColumns());

                        if (results.Count == 0)
                            break;

                        // Update documents
                        var newStories = results.Rows.Select(storyDocumentFactory.MakeDocument).ToList();
                        var existingStories = documentSession.Load<Story>(newStories.Select(x => x.Id));

                        foreach (var newStory in newStories)
                        {
                            var existingStory = existingStories.SingleOrDefault(x => x != null && x.Id == newStory.Id);

                            if (existingStory != null)
                            {
                                // Update existing story
                                existingStory.Update(newStory.Title);
                            }
                            else
                            {
                                // Create new story
                                documentSession.Store(newStory);
                            }
                        }

                        // Save any changes
                        documentSession.SaveChanges();
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