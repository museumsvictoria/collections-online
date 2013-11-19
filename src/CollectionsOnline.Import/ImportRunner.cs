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
        private readonly IItemMigration _itemMigration;

        public ImportRunner(
            IDocumentStore documentStore, 
            IEnumerable<IImport<EmuAggregateRoot>> imports,
            IItemMigration itemMigration)
        {
            _documentStore = documentStore;
            _imports = imports;
            _itemMigration = itemMigration;
        }

        public void Run()
        {
            var dateRun = DateTime.Now;
            var hasFailed = false;

            _log.Debug("Data Import begining");

            var documentSession = _documentStore.OpenSession();
            var application = documentSession.Load<Application>(Constants.ApplicationId);

            if (!application.DataImportRunning)
            {
                application.RunDataImport();
                documentSession.SaveChanges();
                documentSession.Dispose();

                try
                {
                    // Run Imports
                    foreach (var import in _imports)
                    {
                        import.Run(application.LastDataImport);
                    }

                    // Run Item Migration
                    _itemMigration.Run(application.LastDataImport);
                }
                catch (Exception exception)
                {
                    hasFailed = true;
                    _log.Debug(exception.ToString);
                }

                // Imports have run, finish up, need a fresh session as we may have been waiting a while for imports to complete.
                documentSession = _documentStore.OpenSession();
                application = documentSession.Load<Application>(Constants.ApplicationId);

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
                documentSession.Dispose();
            }
        }
    }
}