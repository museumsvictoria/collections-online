using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Import.Factories;
using IMu;
using NLog;
using Raven.Abstractions.Extensions;
using Raven.Client;

namespace CollectionsOnline.Import.Imports
{
    public class ImuImport<T> : IImport where T : EmuAggregateRoot
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly IDocumentStore _documentStore;
        private readonly Session _session;
        private readonly IImuFactory<T> _imuFactory;

        public ImuImport(
            IDocumentStore documentStore,
            Session session,
            IImuFactory<T> imuFactory)
        {
            _documentStore = documentStore;
            _session = session;
            _imuFactory = imuFactory;
        }

        public void Run(DateTime? previousDateRun)
        {            
            var module = new Module(_imuFactory.ModuleName, _session);
            var terms = _imuFactory.Terms;
            int currentOffset;
            IList<long> cachedResult;

            // Check for existing import in case we need to resume.
            using (var documentSession = _documentStore.OpenSession())
            {
                var importProgress = documentSession.Load<Application>(Constants.ApplicationId).GetImportProgress(GetType().ToString());

                // Exit current import if it had completed previous time it was run.
                if (importProgress.IsFinished)
                {
                    return;
                }

                currentOffset = importProgress.CurrentOffset;
                cachedResult = importProgress.CachedResult;
                terms.Add("AdmDateModified", importProgress.DateRun.ToString("MMM dd yyyy"), "<");

                documentSession.SaveChanges();
            }

            if (!previousDateRun.HasValue)
            {
                // Import has never run, do a fresh import
                _log.Debug("Beginning {0} import", typeof(T).Name);

                var hits = module.FindTerms(terms);
                
                _log.Debug("Finished search. {0} Hits", hits);

                if (!cachedResult.Any())
                {
                    // Cache the results from the search to ensure consistency
                    var cachedCurrentOffset = 0;
                    
                    _log.Debug("Caching results from search to ensure consistency");

                    while (true)
                    {
                        if (DateTime.Now.TimeOfDay > Constants.ImuOfflineTimeSpan)
                        {
                            _log.Warn("Imu about to go offline, canceling all imports");
                            Program.ImportCanceled = true;
                        }

                        if (Program.ImportCanceled)
                        {
                            return;
                        }

                        var results = module.Fetch("start", cachedCurrentOffset, Constants.CachedDataBatchSize,
                            new[] {"irn"});

                        if (results.Count == 0)
                            break;

                        cachedResult.AddRange(results.Rows.Select(x => long.Parse(x.GetString("irn"))));

                        cachedCurrentOffset += results.Count;

                        _log.Debug("{0} cache import progress... {1}/{2}", typeof (T).Name, cachedCurrentOffset, hits);
                    }

                    // Store cached result
                    using (var documentSession = _documentStore.OpenSession())
                    {
                        documentSession.Load<Application>(Constants.ApplicationId)
                            .GetImportProgress(GetType().ToString())
                            .CachedResult = cachedResult;
                        documentSession.SaveChanges();
                    }
                }

                while (true)
                {
                    using (var documentSession = _documentStore.OpenSession())
                    {
                        if (DateTime.Now.TimeOfDay > Constants.ImuOfflineTimeSpan)
                        {
                            _log.Warn("Imu about to go offline, canceling all imports");
                            Program.ImportCanceled = true;
                        }

                        if (Program.ImportCanceled)
                        {
                            return;
                        }

                        var cachedResultBatch = cachedResult
                            .Skip(currentOffset)
                            .Take(Constants.DataBatchSize)
                            .ToList();

                        if (cachedResultBatch.Count == 0)
                            break;

                        module.FindKeys(cachedResultBatch);

                        var results = module.Fetch("start", 0, -1, _imuFactory.Columns);

                        // Create and store documents
                        results.Rows
                            .Select(_imuFactory.MakeDocument)
                            .ToList()
                            .ForEach(documentSession.Store);

                        currentOffset += results.Count;

                        documentSession.Load<Application>(Constants.ApplicationId).UpdateImportCurrentOffset(GetType().ToString(), currentOffset);

                        _log.Debug("{0} import progress... {1}/{2}", typeof(T).Name, currentOffset, hits);
                        documentSession.SaveChanges();
                    }
                }
            }
            else
            {
                // Import has been run before, do an update import
                _log.Debug("Beginning {0} update import", typeof(T).Name);
                
                _imuFactory.RegisterAutoMapperMap();

                terms.Add("AdmDateModified", previousDateRun.Value.ToString("MMM dd yyyy"), ">=");

                var hits = module.FindTerms(terms);

                _log.Debug("Finished Search. {0} Hits", hits);

                while (true)
                {
                    using (var documentSession = _documentStore.OpenSession())
                    {
                        if (DateTime.Now.TimeOfDay > Constants.ImuOfflineTimeSpan)
                        {
                            _log.Warn("Imu about to go offline, canceling all imports");
                            Program.ImportCanceled = true;
                        }

                        if (Program.ImportCanceled)
                        {
                            return;
                        }

                        var results = module.Fetch("start", currentOffset, Constants.DataBatchSize, _imuFactory.Columns);

                        if (results.Count == 0)
                            break;

                        // Update documents
                        var newDocuments = results.Rows.Select(_imuFactory.MakeDocument).ToList();
                        var existingDocuments = documentSession.Load<T>(newDocuments.Select(x => x.Id));

                        for (var i = 0; i < newDocuments.Count; i++)
                        {
                            if (existingDocuments[i] != null)
                            {
                                // Update existing
                                Mapper.Map(newDocuments[i], existingDocuments[i]);
                            }
                            else
                            {
                                // Create new
                                documentSession.Store(newDocuments[i]);
                            }
                        }

                        currentOffset += results.Count;

                        documentSession.Load<Application>(Constants.ApplicationId).UpdateImportCurrentOffset(GetType().ToString(), currentOffset);

                        _log.Debug("{0} import progress... {1}/{2}", typeof(T).Name, currentOffset, hits);
                        documentSession.SaveChanges();
                    }
                }
            }

            using (var documentSession = _documentStore.OpenSession())
            {
                documentSession.Load<Application>(Constants.ApplicationId).ImportFinished(GetType().ToString());
                documentSession.SaveChanges();
            }
        }
    }
}