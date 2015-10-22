using System;
using System.Linq;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Import.Extensions;
using CollectionsOnline.Import.Factories;
using CollectionsOnline.Import.Infrastructure;
using IMu;
using Raven.Abstractions.Extensions;
using Raven.Client;
using Serilog;

namespace CollectionsOnline.Import.Imports
{
    public class MediaUpdateImport : ImuImportBase
    {
        private readonly IDocumentStore _documentStore;
        private readonly IImuSessionProvider _imuSessionProvider;
        private readonly IMediaFactory _mediaFactory;

        public MediaUpdateImport(
            IDocumentStore documentStore,
            IImuSessionProvider imuSessionProvider,
            IMediaFactory mediaFactory)
        {
            _documentStore = documentStore;
            _imuSessionProvider = imuSessionProvider;
            _mediaFactory = mediaFactory;
        }

        public override void Run()
        {
            using (Log.Logger.BeginTimedOperation("Media Update Import starting", "MediaUpdateImport.Run"))
            { 
                ImportCache importCache;
                var columns = new[] {
                                        "irn",
                                        "MulTitle",
                                        "MulIdentifier",
                                        "MulMimeType",
                                        "MdaDataSets_tab",
                                        "metadata=[MdaElement_tab,MdaQualifier_tab,MdaFreeText_tab]",
                                        "DetAlternateText",
                                        "RigCreator_tab",
                                        "RigSource_tab",
                                        "RigAcknowledgementCredit",
                                        "RigCopyrightStatement",
                                        "RigCopyrightStatus",
                                        "RigLicence",
                                        "RigLicenceDetails",
                                        "ChaRepository_tab",
                                        "ChaMd5Sum",
                                        "AdmPublishWebNoPassword",
                                        "AdmDateModified",
                                        "AdmTimeModified"
                                    };
                var associatedDocumentCount = 0;

                using (var documentSession = _documentStore.OpenSession())
                using (var imuSession = _imuSessionProvider.CreateInstance("emultimedia"))
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
                        Log.Logger.Information("Import finished last time... skipping");
                        return;
                    }

                    // Check for existing cached results
                    importCache = documentSession.Load<ImportCache>("importCaches/media");
                    if (importCache == null)
                    {
                        Log.Logger.Information("Caching {Name} results", GetType().Name);
                        importCache = new ImportCache { Id = "importCaches/media" };

                        var terms = new Terms();
                        terms.Add("MdaDataSets_tab", Constants.ImuMultimediaQueryString);
                        terms.Add("AdmDateModified", previousDateRun.Value.ToString("MMM dd yyyy"), ">=");

                        var hits = imuSession.FindTerms(terms);

                        Log.Logger.Information("Found {Hits} records where {@Terms}", hits, terms.List);

                        var cachedCurrentOffset = 0;
                        while (true)
                        {
                            if (ImportCanceled())
                                return;

                            var results = imuSession.Fetch("start", cachedCurrentOffset, Constants.CachedDataBatchSize, new[] { "irn" });

                            if (results.Count == 0)
                                break;

                            importCache.Irns.AddRange(results.Rows.Select(x => long.Parse(x.GetEncodedString("irn"))));

                            cachedCurrentOffset += results.Count;

                            Log.Logger.Information("{Name} cache progress... {Offset}/{TotalResults}", GetType().Name, cachedCurrentOffset, hits);
                        }

                        // Store cached result
                        documentSession.Store(importCache);
                        documentSession.SaveChanges();

                        Log.Logger.Information("Caching of {Name} results complete", GetType().Name);
                    }
                    else
                    {
                        Log.Logger.Information("Cached results found, resuming {Name} import.", GetType().Name);
                    }
                }
            
                // Perform import
                while (true)
                {
                    using (var documentSession = _documentStore.OpenSession())
                    using (var imuSession = _imuSessionProvider.CreateInstance("emultimedia"))
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

                        imuSession.FindKeys(cachedResultBatch);

                        var results = imuSession.Fetch("start", 0, -1, columns);

                        Log.Logger.Debug("Fetched {RecordCount} multimedia records from Imu", cachedResultBatch.Count);

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

                                    // Find associated documents that utilize the media we are updating in order to update any denormalized references
                                    var associatedDocumentBatch = associatedDocumentSession
                                        .Query<object, CombinedIndex>()
                                        .Where(x => ((CombinedIndexResult)x).MediaIrns.Any(y => y == mediaIrn))
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
                                            var existingMedia = item.Media
                                                .Where(x => x != null)
                                                .SingleOrDefault(x => x.Irn == mediaIrn);

                                            if (existingMedia != null)
                                                item.Media[item.Media.IndexOf(existingMedia)] = media;

                                            associatedDocumentCount++;
                                            continue;
                                        }

                                        var species = document as Species;
                                        if (species != null)
                                        {
                                            var existingMedia = species.Media
                                                .Where(x => x != null)
                                                .SingleOrDefault(x => x.Irn == mediaIrn);

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
                                            var existingMedia = specimen.Media
                                                .Where(x => x != null)
                                                .SingleOrDefault(x => x.Irn == mediaIrn);

                                            if (existingMedia != null)
                                                specimen.Media[specimen.Media.IndexOf(existingMedia)] = media;

                                            associatedDocumentCount++;
                                            continue;
                                        }

                                        var article = document as Article;
                                        if (article != null)
                                        {
                                            var existingMedia = article.Media
                                                .Where(x => x != null)
                                                .SingleOrDefault(x => x.Irn == mediaIrn);

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

                        Log.Logger.Information("{Name} import progress... {Offset}/{TotalResults}", GetType().Name, importStatus.CurrentImportCacheOffset, importCache.Irns.Count);
                        documentSession.SaveChanges();
                    }
                }

                Log.Logger.Information("Updated {AssociatedDocumentCount} associated documents", associatedDocumentCount);

                using (var documentSession = _documentStore.OpenSession())
                {
                    // Mark import status as finished
                    documentSession.Load<Application>(Constants.ApplicationId).ImportFinished(GetType().ToString(), importCache.DateCreated);
                    documentSession.SaveChanges();
                }
            }
        }

        public override int Order
        {
            get { return 10; }
        }
    }
}