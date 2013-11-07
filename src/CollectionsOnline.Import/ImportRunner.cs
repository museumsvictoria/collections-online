using System;
using System.Collections.Generic;
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
        private readonly IEnumerable<IImport<EmuAggregateRoot>> _imports;

        public ImportRunner(
            IDocumentStore documentStore, 
            IEnumerable<IImport<EmuAggregateRoot>> imports)
        {
            _documentStore = documentStore;
            _imports = imports;
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
                    foreach (var import in _imports)
                    {
                        import.Run(application.LastDataImport);
                    }
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

                if (Program.ImportCanceled || hasFailed)
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