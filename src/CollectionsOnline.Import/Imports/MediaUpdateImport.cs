using System;
using System.Collections.Generic;
using System.Linq;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Import.Extensions;
using CollectionsOnline.Import.Factories;
using CollectionsOnline.Import.Infrastructure;
using IMu;
using Raven.Abstractions.Data;
using Raven.Abstractions.Extensions;
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
                            var media = _mediaFactory.Make(row);

                            // Dont patch and documents if media is null
                            if (media == null) continue;

                            // Document media patch request
                            var mediaDocumentIds = new List<string>();
                            mediaDocumentIds.AddRange(row
                                .GetMaps("catmedia")
                                .Where(x => x != null && x.GetEncodedStrings("MdaDataSets_tab").Contains(Constants.ImuItemQueryString))
                                .Select(x => "items/" + x.GetEncodedString("irn")));
                            mediaDocumentIds.AddRange(row
                                .GetMaps("catmedia")
                                .Where(x => x != null && x.GetEncodedStrings("MdaDataSets_tab").Contains(Constants.ImuSpecimenQueryString))
                                .Select(x => "specimens/" + x.GetEncodedString("irn")));
                            mediaDocumentIds.AddRange(row
                                .GetMaps("narmedia")
                                .Where(x => x != null && x.GetEncodedStrings("DetPurpose_tab").Contains(Constants.ImuArticleQueryString))
                                .Select(x => "articles/" + x.GetEncodedString("irn")));
                            mediaDocumentIds.AddRange(row
                                .GetMaps("narmedia")
                                .Where(x => x != null && x.GetEncodedStrings("DetPurpose_tab").Contains(Constants.ImuSpeciesQueryString))
                                .Select(x => "species/" + x.GetEncodedString("irn")));

                            foreach (var mediaDocumentId in mediaDocumentIds)
                            {
                                Log.Logger.Debug("Patching media contained on {DocumentId}", mediaDocumentId);
                                _documentStore.DatabaseCommands.Patch(mediaDocumentId,
                                    new ScriptedPatchRequest
                                    {
                                        Script = @"
                                            var found = false;
                                            for(var i=0; i < this.Media.length; i++) {
                                                if(this.Media[i].Irn == updatedMedia.Irn) {
                                                    found = true;
                                                    this.Media[i] = updatedMedia;
                                                }
                                            }
                                            if(found === false) {
                                                this.Media.push(updatedMedia);
                                            }",
                                        Values = { { "updatedMedia", RavenJObject.FromObject(media) } }
                                    });
                            }

                            // Authors and curators media patch request
                            var profileDocumentIds = new List<string>();
                            profileDocumentIds.AddRange(row
                                .GetMaps("parmedia")
                                .SelectMany(x => x.GetMaps("narmedia"))
                                .Where(x => x != null && x.GetEncodedStrings("DetPurpose_tab").Contains(Constants.ImuArticleQueryString))
                                .Select(x => "articles/" + x.GetEncodedString("irn")));
                            profileDocumentIds.AddRange(row
                                .GetMaps("parmedia")
                                .SelectMany(x => x.GetMaps("narmedia"))
                                .Where(x => x != null && x.GetEncodedStrings("DetPurpose_tab").Contains(Constants.ImuSpeciesQueryString))
                                .Select(x => "species/" + x.GetEncodedString("irn")));
                            profileDocumentIds.AddRange(row
                                .GetMaps("parmedia")
                                .SelectMany(x => x.GetMaps("narmedia"))
                                .Where(x => x != null && x.GetEncodedStrings("DetPurpose_tab").Contains(Constants.ImuCollectionQueryString))
                                .Select(x => "collections/" + x.GetEncodedString("irn")));

                            foreach (var profileDocumentId in profileDocumentIds)
                            {
                                Log.Logger.Debug("Patching author/curator profile media on {DocumentId}", profileDocumentId);
                                _documentStore.DatabaseCommands.Patch(profileDocumentId,
                                    new ScriptedPatchRequest
                                    {
                                        Script = @"
                                            for(var i=0; i < this.Authors.length; i++) {
                                                if(this.Authors[i].ProfileImage.Irn == updatedMedia.Irn) {        
                                                    this.Authors[i].ProfileImage = updatedMedia;
                                                }
                                            }
                                            for(var i=0; i < this.Curators.length; i++) {
                                                if(this.Curators[i].ProfileImage.Irn == updatedMedia.Irn) {        
                                                    this.Curators[i].ProfileImage = updatedMedia;
                                                }
                                            }",
                                        Values = { { "updatedMedia", RavenJObject.FromObject(media) } }
                                    });
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
    }
}