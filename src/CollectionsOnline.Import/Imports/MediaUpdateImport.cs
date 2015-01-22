using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Import.Extensions;
using CollectionsOnline.Import.Factories;
using IMu;
using NLog;
using Raven.Abstractions.Extensions;
using Raven.Client;

namespace CollectionsOnline.Import.Imports
{
    public class MediaUpdateImport : IImport
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly IDocumentStore _documentStore;
        private readonly Session _session;
        private readonly IMediaFactory _mediaFactory;

        public MediaUpdateImport(
            IDocumentStore documentStore,
            Session session,
            IMediaFactory mediaFactory)
        {
            _documentStore = documentStore;
            _session = session;
            _mediaFactory = mediaFactory;
        }

        public void Run()
        {
            var module = new Module("emultimedia", _session);
            ImportCache importCache;
            var columns = new[]
                                {
                                    "irn",
                                    "MulTitle",
                                    "MulMimeType",
                                    "MulDescription",
                                    "MulCreator_tab",
                                    "MdaDataSets_tab",
                                    "MdaElement_tab",
                                    "MdaQualifier_tab",
                                    "MdaFreeText_tab",
                                    "ChaRepository_tab",
                                    "DetAlternateText",
                                    "AdmPublishWebNoPassword",
                                    "AdmDateModified",
                                    "AdmTimeModified"
                                };
            var associatedDocumentCount = 0;


            using (var documentSession = _documentStore.OpenSession())
            {
                // Check to see whether we need to run import, so grab the earliest previous date run of any imports that utilize multimedia.
                var previousDateRun = documentSession
                    .Load<Application>(Constants.ApplicationId)
                    .ImportStatuses.Where(x => x.ImportType.Contains(typeof(ImuImport<>).Name, StringComparison.OrdinalIgnoreCase))
                    .Select(x => x.PreviousDateRun)
                    .OrderBy(x => x)
                    .FirstOrDefault(x => x.HasValue);

                // Exit current import if it has never run.
                if (!previousDateRun.HasValue)
                    return;

                // Check for existing import in case we need to resume.
                var importStatus = documentSession.Load<Application>(Constants.ApplicationId).GetImportStatus(GetType().ToString());

                // Exit current import if it had completed previous time it was run.
                if (importStatus.IsFinished)
                {
                    return;
                }

                _log.Debug("Starting {0} import", GetType().Name);

                // Check for existing cached results
                importCache = documentSession.Load<ImportCache>("importCaches/media");
                if (importCache == null)
                {
                    importCache = new ImportCache { Id = "importCaches/media" };

                    var terms = new Terms();
                    terms.Add("MdaDataSets_tab", Constants.ImuMultimediaQueryString);
                    terms.Add("AdmDateModified", previousDateRun.Value.ToString("MMM dd yyyy"), ">=");

                    var hits = module.FindTerms(terms);

                    _log.Debug("Caching {0} search results. {1} Hits", GetType().Name, hits);

                    var cachedCurrentOffset = 0;
                    while (true)
                    {
                        if (ImportCanceled())
                            return;

                        var results = module.Fetch("start", cachedCurrentOffset, Constants.CachedDataBatchSize, new[] { "irn" });

                        if (results.Count == 0)
                            break;

                        importCache.Irns.AddRange(results.Rows.Select(x => long.Parse(x.GetEncodedString("irn"))));

                        cachedCurrentOffset += results.Count;

                        _log.Debug("{0} cache progress... {1}/{2}", GetType().Name, cachedCurrentOffset, hits);
                    }

                    // Store cached result
                    documentSession.Store(importCache);
                    documentSession.SaveChanges();

                    _log.Debug("Caching of {0} search results complete, beginning import.", GetType().Name);
                }
                else
                {
                    _log.Debug("Cached search results found, resuming {0} import.", GetType().Name);
                }
            }
            
            // Perform import
            while (true)
            {
                using (var documentSession = _documentStore.OpenSession())
                {
                    if (ImportCanceled())
                        return;

                    var importStatus = documentSession.Load<Application>(Constants.ApplicationId).GetImportStatus(GetType().ToString());

                    var cachedResultBatch = importCache.Irns
                        .Skip(importStatus.CurrentImportCacheOffset)
                        .Take(Constants.DataBatchSize)
                        .ToList();

                    if (cachedResultBatch.Count == 0)
                        break;

                    module.FindKeys(cachedResultBatch);

                    var results = module.Fetch("start", 0, -1, columns);

                    foreach (var row in results.Rows)
                    {
                        var mediaIrn = long.Parse(row.GetString("irn"));

                        var count = 0;
                        while (true)
                        {
                            using (var associatedDocumentSession = _documentStore.OpenSession())
                            {
                                if (ImportCanceled())
                                    return;

                                var associatedDocumentBatch = associatedDocumentSession
                                    .Query<object, Combined>()
                                    .Where(x => ((CombinedResult)x).MediaIrns.Any(y => y == mediaIrn))
                                    .Skip(count)
                                    .Take(Constants.DataBatchSize)
                                    .ToList();

                                if (associatedDocumentBatch.Count == 0)
                                    break;

                                foreach (var document in associatedDocumentBatch)
                                {
                                    var media = _mediaFactory.Make(row);

                                    // Determine type of document
                                    var item = document as Item;
                                    if (item != null)
                                    {
                                        var existingMedia = item.Media.SingleOrDefault(x => x.Irn == mediaIrn);
                                        if (existingMedia != null)
                                            item.Media[item.Media.IndexOf(existingMedia)] = media;

                                        associatedDocumentCount++;
                                        continue;
                                    }

                                    var species = document as Species;
                                    if (species != null)
                                    {
                                        var existingMedia = species.Media.SingleOrDefault(x => x.Irn == mediaIrn);
                                        if (existingMedia != null)
                                            species.Media[species.Media.IndexOf(existingMedia)] = media;

                                        var author = species.Authors.SingleOrDefault(x => x.ProfileImage != null && x.ProfileImage.Irn == mediaIrn);
                                        if (author != null)
                                            author.ProfileImage = media as ImageMedia;

                                        associatedDocumentCount++;
                                        continue;
                                    }

                                    var specimen = document as Specimen;
                                    if (specimen != null)
                                    {
                                        var existingMedia = specimen.Media.SingleOrDefault(x => x.Irn == mediaIrn);
                                        if (existingMedia != null)
                                            specimen.Media[specimen.Media.IndexOf(existingMedia)] = media;

                                        associatedDocumentCount++;
                                        continue;
                                    }

                                    var article = document as Article;
                                    if (article != null)
                                    {
                                        var existingMedia = article.Media.SingleOrDefault(x => x.Irn == mediaIrn);
                                        if (existingMedia != null)
                                            article.Media[article.Media.IndexOf(existingMedia)] = media;

                                        var author = article.Authors.SingleOrDefault(x => x.ProfileImage != null && x.ProfileImage.Irn == mediaIrn);
                                        if (author != null)
                                            author.ProfileImage = media as ImageMedia;

                                        associatedDocumentCount++;
                                    }
                                }

                                // Save any changes
                                associatedDocumentSession.SaveChanges();
                                count += associatedDocumentBatch.Count;
                            }
                        }
                    }

                    importStatus.CurrentImportCacheOffset += results.Count;

                    _log.Debug("{0} import progress... {1}/{2}", GetType().Name, importStatus.CurrentImportCacheOffset, importCache.Irns.Count);
                    documentSession.SaveChanges();
                }
            }

            _log.Debug("{0} import complete, updated {1} associated documents", GetType().Name, associatedDocumentCount);

            using (var documentSession = _documentStore.OpenSession())
            {
                documentSession.Load<Application>(Constants.ApplicationId).ImportFinished(GetType().ToString(), importCache.DateCreated);
                documentSession.SaveChanges();
            }
        }

        public int Order
        {
            get { return 10; }
        }

        private bool ImportCanceled()
        {
            if (DateTime.Now.TimeOfDay > Constants.ImuOfflineTimeSpanStart && DateTime.Now.TimeOfDay < Constants.ImuOfflineTimeSpanEnd)
            {
                _log.Warn("Imu about to go offline, canceling all imports");
                Program.ImportCanceled = true;
            }

            return Program.ImportCanceled;
        }
    }
}