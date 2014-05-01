using System;
using System.Collections.Generic;
using System.Linq;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Import.Imports;
using NLog;
using Raven.Client;

namespace CollectionsOnline.Import.Infrastructure
{
    public class ImportRunner
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly IDocumentStore _documentStore;
        private readonly IEnumerable<IImport> _imports;

        public ImportRunner(
            IDocumentStore documentStore, 
            IEnumerable<IImport> imports)
        {
            _documentStore = documentStore;
            _imports = imports;
        }

        public void Run()
        {
            var hasFailed = false;

            _log.Debug("Data Import begining");

            var documentSession = _documentStore.OpenSession();
            var application = documentSession.Load<Application>(Constants.ApplicationId);

            if (!application.ImportsRunning)
            {
                application.RunAllImports();
                documentSession.SaveChanges();
                documentSession.Dispose();

                try
                {
                    // Run all imports
                    foreach (var import in _imports)
                    {
                        if(Program.ImportCanceled)
                            break;

                        import.Run();
                    }
                }
                catch (Exception exception)
                {
                    hasFailed = true;
                    _log.Error(exception.ToString);
                }

                // Imports have run, finish up, need a fresh session as we may have been waiting a while for imports to complete.
                documentSession = _documentStore.OpenSession();
                application = documentSession.Load<Application>(Constants.ApplicationId);

                if (Program.ImportCanceled || hasFailed)
                {
                    _log.Debug("All imports finished (cancelled or failed)");
                    application.FinishedAllImports();
                }
                else
                {
                    _log.Debug("All imports finished succesfully");
                    application.FinishedAllImportsSuccessfully();
                }
                
                // Force aggressive cache check
                _documentStore.Conventions.ShouldAggressiveCacheTrackChanges = true;

                documentSession.SaveChanges();
                documentSession.Dispose();
            }
        }
    }
}