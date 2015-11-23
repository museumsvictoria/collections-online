using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Import.Extensions;
using CollectionsOnline.Import.Factories;
using CollectionsOnline.Import.Infrastructure;
using IMu;
using Raven.Abstractions.Data;
using Raven.Abstractions.Extensions;
using Raven.Abstractions.Indexing;
using Raven.Client;
using Raven.Json.Linq;
using Serilog;
using Constants = CollectionsOnline.Core.Config.Constants;

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
                                        "AdmTimeModified",
                                        "catmedia=<ecatalogue:MulMultiMediaRef_tab>.(irn,MdaDataSets_tab)",
                                        "narmedia=<enarratives:MulMultiMediaRef_tab>.(irn,DetPurpose_tab)",
                                        "parmedia=<eparties:MulMultiMediaRef_tab>.(narmedia=<enarratives:NarAuthorsRef_tab>.(irn,DetPurpose_tab))"
                                    };

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
                    {
                        Log.Logger.Information("Dependant imports have never been run... skipping");
                        return;
                    }

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
                            var mediaIrn = long.Parse(row.GetEncodedString("irn"));
                            var media = _mediaFactory.Make(row);

                            UpdateItemMedia(row, media, mediaIrn);
                            UpdateArticleMedia(row, media, mediaIrn);
                            UpdateSpeciesMedia(row, media, mediaIrn);
                            UpdateSpecimenMedia(row, media, mediaIrn);
                            UpdateArticleProfileMedia(row, media, mediaIrn);
                            UpdateSpeciesProfileMedia(row, media, mediaIrn);
                            UpdateCollectionProfileMedia(row, media, mediaIrn);
                        }

                        importStatus.CurrentImportCacheOffset += results.Count;

                        Log.Logger.Information("{Name} import progress... {Offset}/{TotalResults}", GetType().Name, importStatus.CurrentImportCacheOffset, importCache.Irns.Count);
                        documentSession.SaveChanges();
                    }
                }                

                using (var documentSession = _documentStore.OpenSession())
                {
                    // Mark import status as finished
                    documentSession.Load<Application>(Constants.ApplicationId).ImportFinished(GetType().ToString(), importCache.DateCreated);
                    documentSession.SaveChanges();
                }
            }
        }

        private void UpdateArticleMedia(Map map, Media media, long mediaIrn)
        {
            var count = 0;

            var articleDocumentIds = map
                .GetMaps("narmedia")
                .Where(x => x != null && x.GetEncodedStrings("MdaDataSets_tab").Contains(Constants.ImuArticleQueryString))
                .Select(x => long.Parse(x.GetEncodedString("irn")))
                .ToList();

            while (true)
            {
                using (var documentSession = _documentStore.OpenSession())
                {
                    if (ImportCanceled())
                        return;

                    var articleDocumentIdBatch = articleDocumentIds
                        .Skip(count)
                        .Take(Constants.DataBatchSize)
                        .ToList();

                    if(articleDocumentIdBatch.Count == 0)
                        break;

                    foreach (var articleDocumentId in articleDocumentIdBatch)
                    {
                        var article = documentSession.Load<Article>(articleDocumentId);

                        if (article != null)
                        {
                            // Add/Replace
                            if (media != null)
                            {
                                var existingMedia = article.Media
                                    .Where(x => x != null)
                                    .SingleOrDefault(x => x.Irn == mediaIrn);

                                if (existingMedia != null)
                                    article.Media[article.Media.IndexOf(existingMedia)] = media;
                                else
                                    article.Media.Add(media);
                            }
                            // Remove/Media
                            else
                            {
                                var existingMedia = article.Media
                                    .Where(x => x != null)
                                    .SingleOrDefault(x => x.Irn == mediaIrn);

                                if (existingMedia != null)
                                    article.Media.Remove(existingMedia);
                            }

                            // Assign thumbnail
                            var mediaWithThumbnail = article.Media.OfType<IHasThumbnail>().FirstOrDefault();
                            article.ThumbnailUri = mediaWithThumbnail != null ? mediaWithThumbnail.Thumbnail.Uri : null;
                        }
                    }

                    count += articleDocumentIdBatch.Count;
                    documentSession.SaveChanges();
                }
            }
        }

        private void UpdateItemMedia(Map map, Media media, long mediaIrn)
        {
            var count = 0;

            var itemDocumentIds = map
                .GetMaps("catmedia")
                .Where(x => x != null && x.GetEncodedStrings("MdaDataSets_tab").Contains(Constants.ImuItemQueryString))
                .Select(x => long.Parse(x.GetEncodedString("irn")))
                .ToList();

            while (true)
            {
                using (var documentSession = _documentStore.OpenSession())
                {
                    if (ImportCanceled())
                        return;

                    var itemDocumentIdBatch = itemDocumentIds
                        .Skip(count)
                        .Take(Constants.DataBatchSize)
                        .ToList();

                    if (itemDocumentIdBatch.Count == 0)
                        break;

                    foreach (var itemDocumentId in itemDocumentIdBatch)
                    {
                        var item = documentSession.Load<Item>(itemDocumentId);

                        if (item != null)
                        {
                            // Add/Replace
                            if (media != null)
                            {
                                var existingMedia = item.Media
                                    .Where(x => x != null)
                                    .SingleOrDefault(x => x.Irn == mediaIrn);

                                if (existingMedia != null)
                                    item.Media[item.Media.IndexOf(existingMedia)] = media;
                                else
                                    item.Media.Add(media);
                            }
                            // Remove/Media
                            else
                            {
                                var existingMedia = item.Media
                                    .Where(x => x != null)
                                    .SingleOrDefault(x => x.Irn == mediaIrn);

                                if (existingMedia != null)
                                    item.Media.Remove(existingMedia);
                            }

                            // Assign thumbnail
                            var mediaWithThumbnail = item.Media.OfType<IHasThumbnail>().FirstOrDefault();
                            item.ThumbnailUri = mediaWithThumbnail != null ? mediaWithThumbnail.Thumbnail.Uri : null;
                        }
                    }

                    count += itemDocumentIdBatch.Count;
                    documentSession.SaveChanges();
                }
            }
        }

        private void UpdateSpeciesMedia(Map map, Media media, long mediaIrn)
        {
            var count = 0;

            var speciesDocumentIds = map
                .GetMaps("narmedia")
                .Where(x => x != null && x.GetEncodedStrings("MdaDataSets_tab").Contains(Constants.ImuSpeciesQueryString))
                .Select(x => long.Parse(x.GetEncodedString("irn")))
                .ToList();

            while (true)
            {
                using (var documentSession = _documentStore.OpenSession())
                {
                    if (ImportCanceled())
                        return;

                    var speciesDocumentIdBatch = speciesDocumentIds
                        .Skip(count)
                        .Take(Constants.DataBatchSize)
                        .ToList();

                    if (speciesDocumentIdBatch.Count == 0)
                        break;

                    foreach (var speciesDocumentId in speciesDocumentIdBatch)
                    {
                        var species = documentSession.Load<Species>(speciesDocumentId);

                        if (species != null)
                        {
                            // Add/Replace
                            if (media != null)
                            {
                                var existingMedia = species.Media
                                    .Where(x => x != null)
                                    .SingleOrDefault(x => x.Irn == mediaIrn);

                                if (existingMedia != null)
                                    species.Media[species.Media.IndexOf(existingMedia)] = media;
                                else
                                    species.Media.Add(media);
                            }
                            // Remove/Media
                            else
                            {
                                var existingMedia = species.Media
                                    .Where(x => x != null)
                                    .SingleOrDefault(x => x.Irn == mediaIrn);

                                if (existingMedia != null)
                                    species.Media.Remove(existingMedia);
                            }

                            // Assign thumbnail
                            var mediaWithThumbnail = species.Media.OfType<IHasThumbnail>().FirstOrDefault();
                            species.ThumbnailUri = mediaWithThumbnail != null ? mediaWithThumbnail.Thumbnail.Uri : null;
                        }
                    }

                    count += speciesDocumentIdBatch.Count;
                    documentSession.SaveChanges();
                }
            }
        }

        private void UpdateSpecimenMedia(Map map, Media media, long mediaIrn)
        {
            var count = 0;

            var specimenDocumentIds = map
                .GetMaps("catmedia")
                .Where(x => x != null && x.GetEncodedStrings("MdaDataSets_tab").Contains(Constants.ImuSpecimenQueryString))
                .Select(x => long.Parse(x.GetEncodedString("irn")))
                .ToList();

            while (true)
            {
                using (var documentSession = _documentStore.OpenSession())
                {
                    if (ImportCanceled())
                        return;

                    var specimenDocumentIdBatch = specimenDocumentIds
                        .Skip(count)
                        .Take(Constants.DataBatchSize)
                        .ToList();

                    if (specimenDocumentIdBatch.Count == 0)
                        break;

                    foreach (var specimenDocumentId in specimenDocumentIdBatch)
                    {
                        var specimen = documentSession.Load<Specimen>(specimenDocumentId);

                        if (specimen != null)
                        {
                            // Add/Replace
                            if (media != null)
                            {
                                var existingMedia = specimen.Media
                                    .Where(x => x != null)
                                    .SingleOrDefault(x => x.Irn == mediaIrn);

                                if (existingMedia != null)
                                    specimen.Media[specimen.Media.IndexOf(existingMedia)] = media;
                                else
                                    specimen.Media.Add(media);
                            }
                            // Remove/Media
                            else
                            {
                                var existingMedia = specimen.Media
                                    .Where(x => x != null)
                                    .SingleOrDefault(x => x.Irn == mediaIrn);

                                if (existingMedia != null)
                                    specimen.Media.Remove(existingMedia);
                            }

                            // Assign thumbnail
                            var mediaWithThumbnail = specimen.Media.OfType<IHasThumbnail>().FirstOrDefault();
                            specimen.ThumbnailUri = mediaWithThumbnail != null ? mediaWithThumbnail.Thumbnail.Uri : null;
                        }
                    }

                    count += specimenDocumentIdBatch.Count;
                    documentSession.SaveChanges();
                }
            }
        }

        private void UpdateArticleProfileMedia(Map map, Media media, long mediaIrn)
        {
            var count = 0;

            var articleDocumentIds = map
                .GetMaps("parmedia")
                .SelectMany(x => x.GetMaps("narmedia"))
                .Where(x => x != null && x.GetEncodedStrings("DetPurpose_tab").Contains(Constants.ImuArticleQueryString))
                .Select(x => long.Parse(x.GetEncodedString("irn")))
                .ToList();

            while (true)
            {
                using (var documentSession = _documentStore.OpenSession())
                {
                    if (ImportCanceled())
                        return;

                    var articleDocumentIdBatch = articleDocumentIds
                        .Skip(count)
                        .Take(Constants.DataBatchSize)
                        .ToList();

                    if (articleDocumentIdBatch.Count == 0)
                        break;

                    foreach (var articleDocumentId in articleDocumentIdBatch)
                    {
                        var article = documentSession.Load<Article>(articleDocumentId);

                        if (article != null)
                        {
                            // Add/Replace
                            var author = article.Authors.FirstOrDefault(x => x.ProfileImage.Irn == mediaIrn);

                            if (author != null)
                            {
                                author.ProfileImage = media != null ? media as ImageMedia : null;
                            }
                        }
                    }

                    count += articleDocumentIdBatch.Count;
                    documentSession.SaveChanges();
                }
            }
        }

        private void UpdateSpeciesProfileMedia(Map map, Media media, long mediaIrn)
        {
            var count = 0;

            var speciesDocumentIds = map
                .GetMaps("parmedia")
                .SelectMany(x => x.GetMaps("narmedia"))
                .Where(x => x != null && x.GetEncodedStrings("DetPurpose_tab").Contains(Constants.ImuSpeciesQueryString))
                .Select(x => long.Parse(x.GetEncodedString("irn")))
                .ToList();

            while (true)
            {
                using (var documentSession = _documentStore.OpenSession())
                {
                    if (ImportCanceled())
                        return;

                    var speciesDocumentIdBatch = speciesDocumentIds
                        .Skip(count)
                        .Take(Constants.DataBatchSize)
                        .ToList();

                    if (speciesDocumentIdBatch.Count == 0)
                        break;

                    foreach (var speciesDocumentId in speciesDocumentIdBatch)
                    {
                        var species = documentSession.Load<Species>(speciesDocumentId);

                        if (species != null)
                        {
                            // Add/Replace
                            var author = species.Authors.FirstOrDefault(x => x.ProfileImage.Irn == mediaIrn);

                            if (author != null)
                            {
                                author.ProfileImage = media != null ? media as ImageMedia : null;
                            }
                        }
                    }

                    count += speciesDocumentIdBatch.Count;
                    documentSession.SaveChanges();
                }
            }
        }

        private void UpdateCollectionProfileMedia(Map map, Media media, long mediaIrn)
        {
            var count = 0;

            var collectionDocumentIds = map
                .GetMaps("parmedia")
                .SelectMany(x => x.GetMaps("narmedia"))
                .Where(x => x != null && x.GetEncodedStrings("DetPurpose_tab").Contains(Constants.ImuCollectionQueryString))
                .Select(x => long.Parse(x.GetEncodedString("irn")))
                .ToList();

            while (true)
            {
                using (var documentSession = _documentStore.OpenSession())
                {
                    if (ImportCanceled())
                        return;

                    var collectionDocumentIdBatch = collectionDocumentIds
                        .Skip(count)
                        .Take(Constants.DataBatchSize)
                        .ToList();

                    if (collectionDocumentIdBatch.Count == 0)
                        break;

                    foreach (var collectionDocumentId in collectionDocumentIdBatch)
                    {
                        var collection = documentSession.Load<Species>(collectionDocumentId);

                        if (collection != null)
                        {
                            // Add/Replace
                            var author = collection.Authors.FirstOrDefault(x => x.ProfileImage.Irn == mediaIrn);

                            if (author != null)
                            {
                                author.ProfileImage = media != null ? media as ImageMedia : null;
                            }
                        }
                    }

                    count += collectionDocumentIdBatch.Count;
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