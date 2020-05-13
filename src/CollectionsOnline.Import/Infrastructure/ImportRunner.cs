using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using CollectionsOnline.Core.Infrastructure;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Import.Imports;
using Raven.Abstractions.Data;
using Raven.Client;
using Serilog;
using Constants = CollectionsOnline.Core.Config.Constants;

namespace CollectionsOnline.Import.Infrastructure
{
    public class ImportRunner
    {
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
            var importHasFailed = false;

            using (Log.Logger.BeginTimedOperation("Emu data Import starting", "ImportRunner.Run"))
            {
                var documentSession = _documentStore.OpenSession();
                var application = documentSession.Load<Application>(Constants.ApplicationId);

                if (!application.ImportsRunning)
                {
                    application.RunAllImports();
                    documentSession.SaveChanges();
                    documentSession.Dispose();

                    NetworkShareAccesser networkShareAccesser = null;
                    if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["WebSiteDomain"]))
                    {
                        networkShareAccesser =
                            NetworkShareAccesser.Access(ConfigurationManager.AppSettings["WebSiteComputer"],
                                ConfigurationManager.AppSettings["WebSiteDomain"],
                                ConfigurationManager.AppSettings["WebSiteUser"],
                                ConfigurationManager.AppSettings["WebSitePassword"]);
                    }
                    try
                    {
                        // Run all imports
                        foreach (var import in _imports.OrderBy(x => x.Order))
                        {
                            if (Program.ImportCanceled)
                                break;

                            import.Run();
                        }
                    }
                    catch (Exception ex)
                    {
                        importHasFailed = true;
                        Log.Logger.Error(ex, "Exception occured running import");
                    }
                    finally
                    {
                        if (networkShareAccesser != null)
                            networkShareAccesser.Dispose();
                    }

                    // Imports have run, finish up, need a fresh session as we may have been waiting a while for imports to complete.
                    documentSession = _documentStore.OpenSession();
                    application = documentSession.Load<Application>(Constants.ApplicationId);

                    if (Program.ImportCanceled || importHasFailed)
                    {
                        Log.Logger.Information("Import has been stopped prematurely {@Reason}", new { Program.ImportCanceled, importHasFailed });
                        application.FinishedAllImports();
                    }
                    else
                    {
                        Log.Logger.Information("All imports finished successfully");
                        application.FinishedAllImportsSuccessfully();

                        // Delete all import caches
                        _documentStore.DatabaseCommands.DeleteByIndex("Raven/DocumentsByEntityName", new IndexQuery {Query = "Tag:ImportCaches"}, new BulkOperationOptions {AllowStale = true});
                    }

                    // Force aggressive cache check
                    _documentStore.Conventions.ShouldAggressiveCacheTrackChanges = true;

                    documentSession.SaveChanges();
                    documentSession.Dispose();
                }
                else
                {
                    Log.Logger.Information("Another import is currently running... cancelling import");
                }
            }
        }
    }
}