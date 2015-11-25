using System;
using System.Collections.Generic;
using System.Linq;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Import.Extensions;
using CollectionsOnline.Import.Factories;
using CollectionsOnline.Import.Infrastructure;
using IMu;
using Raven.Abstractions.Extensions;
using Raven.Client;
using Serilog;
using Constants = CollectionsOnline.Core.Config.Constants;

namespace CollectionsOnline.Import.Imports
{
    public class MediaUpdateImport : ImuImportBase
    {
        private readonly IDocumentStore _documentStore;
        private readonly IImuSessionProvider _imuSessionProvider;
        private readonly IMediaFactory _mediaFactory;
        private readonly IList<DocumentMediaUpdateJob> _documentMediaUpdateJobs;

        public MediaUpdateImport(
            IDocumentStore documentStore,
            IImuSessionProvider imuSessionProvider,
            IMediaFactory mediaFactory)
        {
            _documentStore = documentStore;
            _imuSessionProvider = imuSessionProvider;
            _mediaFactory = mediaFactory;
            _documentMediaUpdateJobs = new List<DocumentMediaUpdateJob>
            {
                new DocumentMediaUpdateJob
                {
                    JobType = JobType.Media,
                    GetDocumentIds = map => map
                        .GetMaps("narmedia")
                        .Where(x => x != null && x.GetEncodedStrings("DetPurpose_tab").Contains(Constants.ImuArticleQueryString))
                        .Select(x => string.Format("articles/{0}", x.GetEncodedString("irn")))
                        .Concat(map
                        .GetMaps("catmedia")
                        .Where(x => x != null && x.GetEncodedStrings("MdaDataSets_tab").Contains(Constants.ImuItemQueryString))
                        .Select(x => "items/" + x.GetEncodedString("irn")))
                        .Concat(map
                        .GetMaps("narmedia")
                        .Where(x => x != null && x.GetEncodedStrings("DetPurpose_tab").Contains(Constants.ImuSpeciesQueryString))
                        .Select(x => "species/" + x.GetEncodedString("irn")))
                        .Concat(map
                        .GetMaps("catmedia")
                        .Where(x => x != null && x.GetEncodedStrings("MdaDataSets_tab").Contains(Constants.ImuSpecimenQueryString))
                        .Select(x => "specimens/" + x.GetEncodedString("irn")))
                },
                new DocumentMediaUpdateJob
                {
                    JobType = JobType.ProfileImage,
                    GetDocumentIds = map => map
                        .GetMaps("parmedia")
                        .SelectMany(x => x.GetMaps("narmedia"))
                        .Where(x => x != null && x.GetEncodedStrings("DetPurpose_tab").Contains(Constants.ImuArticleQueryString))
                        .Select(x => "articles/" + x.GetEncodedString("irn"))
                        .Concat(map
                        .GetMaps("parmedia")
                        .SelectMany(x => x.GetMaps("narmedia"))
                        .Where(x => x != null && x.GetEncodedStrings("DetPurpose_tab").Contains(Constants.ImuSpeciesQueryString))
                        .Select(x => "species/" + x.GetEncodedString("irn")))
                        .Concat(map
                        .GetMaps("parmedia")
                        .SelectMany(x => x.GetMaps("narmedia"))
                        .Where(x => x != null && x.GetEncodedStrings("DetPurpose_tab").Contains(Constants.ImuCollectionQueryString))
                        .Select(x => "collections/" + x.GetEncodedString("irn")))
                }
            };
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
                            if (ImportCanceled())
                                return;

                            var mediaIrn = long.Parse(row.GetEncodedString("irn"));
                            var media = _mediaFactory.Make(row);

                            foreach (var documentMediaUpdateJob in _documentMediaUpdateJobs)
                            {
                                var count = 0;
                                var documentIds = documentMediaUpdateJob.GetDocumentIds(row);

                                while (true)
                                {
                                    using (var documentMediaUpdateSession = _documentStore.OpenSession())
                                    {
                                        if (ImportCanceled())
                                            return;

                                        var documentIdsBatch = documentIds
                                            .Skip(count)
                                            .Take(Constants.DataBatchSize)
                                            .ToList();

                                        if (documentIdsBatch.Count == 0)
                                            break;

                                        foreach (var documentId in documentIdsBatch)
                                        {
                                            var document = documentMediaUpdateSession.Load<dynamic>(documentId);

                                            if (document == null) continue;

                                            // Update Media
                                            if (documentMediaUpdateJob.JobType == JobType.Media)
                                            {
                                                IList<Media> documentMedia = document.Media;

                                                // Add/Replace
                                                if (media != null)
                                                {
                                                    var existingMedia = documentMedia
                                                        .Where(x => x != null)
                                                        .SingleOrDefault(x => x.Irn == mediaIrn);

                                                    if (existingMedia != null)
                                                    {
                                                        Log.Logger.Debug("Updating media on {Id}", documentId);
                                                        document.Media[document.Media.IndexOf(existingMedia)] = media;
                                                    }
                                                    else
                                                    {
                                                        Log.Logger.Debug("Adding media on {Id}", documentId);
                                                        document.Media.Add(media);
                                                    }
                                                }
                                                // Remove/Media
                                                else
                                                {
                                                    var existingMedia = documentMedia
                                                        .Where(x => x != null)
                                                        .SingleOrDefault(x => x.Irn == mediaIrn);

                                                    if (existingMedia != null)
                                                    {
                                                        Log.Logger.Debug("Removing media on {Id}", documentId);
                                                        document.Media.Remove(existingMedia);
                                                    }
                                                }

                                                // Assign thumbnail
                                                var mediaWithThumbnail = documentMedia.OfType<IHasThumbnail>().FirstOrDefault();
                                                document.ThumbnailUri = mediaWithThumbnail != null ? mediaWithThumbnail.Thumbnail.Uri : null;
                                            }
                                            // Update ProfileImages
                                            else if(documentMediaUpdateJob.JobType == JobType.ProfileImage)
                                            {
                                                IList<Author> documentAuthors = document.Authors;

                                                // Add/Replace
                                                var author = documentAuthors.FirstOrDefault(x => x.ProfileImage.Irn == mediaIrn);

                                                if (author != null)
                                                {
                                                    Log.Logger.Debug("Updating {FullName} profileImage on {Id}", author.FullName, documentId);
                                                    author.ProfileImage = media != null ? media as ImageMedia : null;
                                                }
                                            }
                                        }

                                        count += documentIdsBatch.Count;
                                        documentMediaUpdateSession.SaveChanges();
                                    }
                                }
                            }
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

        public override int Order
        {
            get { return 10; }
        }

        private class DocumentMediaUpdateJob
        {
            public Func<Map, IEnumerable<string>> GetDocumentIds { get; set; }

            public JobType JobType { get; set; }
        }

        private enum JobType
        {
            Media,
            ProfileImage
        }
    }
}