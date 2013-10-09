using System;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Import.Importers;
using NLog;
using Raven.Client;

namespace CollectionsOnline.Import
{
    public class ImportRunner
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly IDocumentStore _documentStore;
        private readonly IImport<Item> _itemImport;
        private readonly IImport<Species> _speciesImport;
        private readonly IImport<Specimen> _specimenImport;
        private readonly IImport<Story> _storyImport;

        public ImportRunner(
            IDocumentStore documentStore, 
            IImport<Item> itemImport,
            IImport<Species> speciesImport,
            IImport<Specimen> specimenImport,
            IImport<Story> storyImport)
        {
            _documentStore = documentStore;
            _itemImport = itemImport;
            _speciesImport = speciesImport;
            _specimenImport = specimenImport;
            _storyImport = storyImport;
        }

        public void Run()
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
                    _itemImport.Run(application.LastDataImport);
                    _speciesImport.Run(application.LastDataImport);
                    _specimenImport.Run(application.LastDataImport);
                    _storyImport.Run(application.LastDataImport);
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

                if (Program.ImportCancelled || hasFailed)
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
    }
}