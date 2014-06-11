using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Transactions;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Import.Factories;
using CollectionsOnline.Import.Utilities;
using ImageResizer;
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
        private readonly IMediaHelper _mediaHelper;

        public MediaUpdateImport(
            IDocumentStore documentStore,
            Session session,
            IMediaHelper mediaHelper)
        {
            _documentStore = documentStore;
            _session = session;
            _mediaHelper = mediaHelper;
        }

        public void Run()
        {
            var module = new Module("emultimedia", _session);
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
                                    "AdmPublishWebNoPassword",
                                    "AdmDateModified",
                                    "AdmTimeModified",
                                    "catalogue=<ecatalogue:MulMultiMediaRef_tab>.(irn,sets=MdaDataSets_tab)",
                                    "narrative=<enarratives:MulMultiMediaRef_tab>.(irn,sets=DetPurpose_tab)",
                                    "parties=<eparties:MulMultiMediaRef_tab>.(narrative=<enarratives:NarAuthorsRef_tab>.(irn,sets=DetPurpose_tab))"
                                };
                       
            using (var documentSession = _documentStore.OpenSession())
            {
                // Check to see whether we need to run import, so grab the previous date run of any imports that utilize multimedia.
                var previousDateRun = documentSession
                    .Load<Application>(Constants.ApplicationId)
                    .ImportStatuses.Where(x => x.ImportType.Contains("imuimport", StringComparison.OrdinalIgnoreCase))
                    .Select(x => x.PreviousDateRun)
                    .OrderBy(x => x)
                    .FirstOrDefault(x => x.HasValue);

                // Exit current import if it has never run.
                if(!previousDateRun.HasValue)
                    return;

                // Check for existing import in case we need to resume.
                var importStatus = documentSession.Load<Application>(Constants.ApplicationId).GetImportStatus(GetType().ToString());

                // Exit current import if it had completed previous time it was run.
                if (importStatus.IsFinished)
                {
                    return;
                }

                _log.Debug("Starting {0} import", GetType().Name);

                // Cache the search results
                if (importStatus.CachedResult == null)
                {
                    var terms = new Terms();
                    terms.Add("AdmDateModified", previousDateRun.Value.ToString("MMM dd yyyy"), ">=");
                    importStatus.CachedResult = new List<long>();
                    importStatus.CachedResultDate = DateTime.Now;

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

                        importStatus.CachedResult.AddRange(results.Rows.Select(x => long.Parse(x.GetString("irn"))));

                        cachedCurrentOffset += results.Count;

                        _log.Debug("{0} cache progress... {1}/{2}", GetType().Name, cachedCurrentOffset, hits);
                    }

                    // Store cached result
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
                using (var tx = new TransactionScope())
                using (var documentSession = _documentStore.OpenSession())
                {
                    if (ImportCanceled())
                        return;

                    var importStatus = documentSession.Load<Application>(Constants.ApplicationId).GetImportStatus(GetType().ToString());

                    var cachedResultBatch = importStatus.CachedResult
                        .Skip(importStatus.CurrentOffset)
                        .Take(Constants.DataBatchSize)
                        .ToList();

                    if (cachedResultBatch.Count == 0)
                        break;

                    module.FindKeys(cachedResultBatch);

                    var results = module.Fetch("start", 0, -1, columns);

                    foreach (var row in results.Rows)
                    {
                        // Dont save the image multiple times in the instance image is attached to multiple aggregates.
                        var hasBeenSaved = false;

                        var mediaIrn = long.Parse(row.GetString("irn"));
                        var media = new Media
                        {
                            Irn = mediaIrn,
                            DateModified =
                                DateTime.ParseExact(
                                    string.Format("{0} {1}", row.GetString("AdmDateModified"),
                                        row.GetString("AdmTimeModified")), "dd/MM/yyyy HH:mm",
                                    new CultureInfo("en-AU")),
                            Title = row.GetString("MulTitle"),
                            Type = row.GetString("MulMimeType"),
                            Url = PathFactory.MakeUrlPath(mediaIrn, FileFormatType.Jpg, "thumb")
                        };
                        var thumbResizeSettings = new ResizeSettings
                        {
                            Format = FileFormatType.Jpg.ToString(),
                            Height = 365,
                            Width = 365,
                            Mode = FitMode.Crop,
                            PaddingColor = Color.White,
                            Quality = 65
                        };

                        // Check if we need to update media on Items and Specimens
                        var count = 0;
                        var catalogues = row.GetMaps("catalogue");
                        while (true)
                        {
                            using (var catalogueDocumentSession = _documentStore.OpenSession())
                            {
                                if (ImportCanceled())
                                    return;

                                var catalogueBatch = catalogues
                                    .Skip(count)
                                    .Take(Constants.DataBatchSize)
                                    .ToList();

                                if (catalogueBatch.Count == 0)
                                    break;

                                foreach (var catalogue in catalogueBatch)
                                {
                                    var catalogueIrn = long.Parse(catalogue.GetString("irn"));
                                    var sets = catalogue.GetStrings("sets");

                                    // Item Media
                                    if (sets.Contains(Constants.ImuItemQueryString))
                                    {
                                        var item = catalogueDocumentSession.Load<Item>(catalogueIrn);

                                        if (item != null)
                                        {
                                            if (hasBeenSaved || _mediaHelper.Save(mediaIrn, FileFormatType.Jpg, thumbResizeSettings, "thumb"))
                                            {
                                                hasBeenSaved = true;
                                                // Find and delete existing media
                                                var existingMedia = item.Media.SingleOrDefault(x => x.Irn == mediaIrn);
                                                if (existingMedia != null)
                                                    item.Media.Remove(existingMedia);

                                                item.Media.Add(media);
                                            }
                                        }
                                    }
                                    // Specimen Media
                                    if (sets.Contains(Constants.ImuSpecimenQueryString))
                                    {
                                        var specimen = catalogueDocumentSession.Load<Specimen>(catalogueIrn);

                                        if (specimen != null)
                                        {
                                            if (hasBeenSaved || _mediaHelper.Save(mediaIrn, FileFormatType.Jpg, thumbResizeSettings, "thumb"))
                                            {
                                                hasBeenSaved = true;
                                                // Find and delete existing media
                                                var existingMedia = specimen.Media.SingleOrDefault(x => x.Irn == mediaIrn);
                                                if (existingMedia != null)
                                                    specimen.Media.Remove(existingMedia);

                                                specimen.Media.Add(media);
                                            }
                                        }
                                    }
                                }

                                // Save any changes
                                catalogueDocumentSession.SaveChanges();
                                count += catalogueBatch.Count;
                            }
                        }

                        // Check if we need to update media on Stories and Species
                        count = 0;
                        var narratives = row.GetMaps("narrative");
                        while (true)
                        {
                            using (var narrativeDocumentSession = _documentStore.OpenSession())
                            {
                                if (ImportCanceled())
                                    return;

                                var narrativeBatch = narratives
                                    .Skip(count)
                                    .Take(Constants.DataBatchSize)
                                    .ToList();

                                if (narrativeBatch.Count == 0)
                                    break;

                                foreach (var narrative in narrativeBatch)
                                {
                                    var narrativeIrn = long.Parse(narrative.GetString("irn"));
                                    var sets = narrative.GetStrings("sets");

                                    // Species Media
                                    if (sets.Contains(Constants.ImuSpeciesQueryString))
                                    {
                                        var species = narrativeDocumentSession.Load<Species>(narrativeIrn);

                                        if (species != null)
                                        {
                                            if (hasBeenSaved || _mediaHelper.Save(mediaIrn, FileFormatType.Jpg, thumbResizeSettings, "thumb"))
                                            {
                                                hasBeenSaved = true;
                                                // Find and delete existing media
                                                var existingMedia = species.Media.SingleOrDefault(x => x.Irn == mediaIrn);
                                                if (existingMedia != null)
                                                    species.Media.Remove(existingMedia);

                                                species.Media.Add(media);
                                            }
                                        }
                                    }

                                    // Story media
                                    if (sets.Any(x => x == Constants.ImuStoryQueryString))
                                    {
                                        var story = narrativeDocumentSession.Load<Story>(narrativeIrn);

                                        if (story != null)
                                        {
                                            if (hasBeenSaved || _mediaHelper.Save(mediaIrn, FileFormatType.Jpg, thumbResizeSettings, "thumb"))
                                            {
                                                hasBeenSaved = true;
                                                // Find and delete existing media
                                                var existingMedia = story.Media.SingleOrDefault(x => x.Irn == mediaIrn);
                                                if (existingMedia != null)
                                                    story.Media.Remove(existingMedia);

                                                story.Media.Add(media);
                                            }
                                        }
                                    }
                                }

                                // Save any changes
                                narrativeDocumentSession.SaveChanges();
                                count += narrativeBatch.Count;
                            }
                        }

                        // Check if we need to update media on Authors attached to Stories and Species
                        count = 0;
                        var partyNarratives = row.GetMaps("parties").SelectMany(x => x.GetMaps("narrative"));
                        while (true)
                        {
                            using (var partyNarrativeDocumentSession = _documentStore.OpenSession())
                            {
                                if (ImportCanceled())
                                    return;

                                var partyNarrativeBatch = partyNarratives
                                    .Skip(count)
                                    .Take(Constants.DataBatchSize)
                                    .ToList();

                                if (partyNarrativeBatch.Count == 0)
                                    break;

                                foreach (var partyNarrative in partyNarrativeBatch)
                                {
                                    var narrativeIrn = long.Parse(partyNarrative.GetString("irn"));
                                    var sets = partyNarrative.GetStrings("sets");

                                    // Species Author media
                                    if (sets.Contains(Constants.ImuSpeciesQueryString))
                                    {
                                        var species = partyNarrativeDocumentSession.Load<Species>(narrativeIrn);

                                        if (species != null)
                                        {
                                            if (hasBeenSaved || _mediaHelper.Save(mediaIrn, FileFormatType.Jpg, thumbResizeSettings, "thumb"))
                                            {
                                                hasBeenSaved = true;
                                                // Find and replace existing media
                                                var author = species.Authors.SingleOrDefault(x => x.Media.Irn == mediaIrn);
                                                if (author != null)
                                                    author.Media = media;
                                            }
                                        }
                                    }

                                    // Story Author media
                                    if (sets.Contains(Constants.ImuStoryQueryString))
                                    {
                                        var story = partyNarrativeDocumentSession.Load<Story>(narrativeIrn);

                                        if (story != null)
                                        {
                                            if (hasBeenSaved || _mediaHelper.Save(mediaIrn, FileFormatType.Jpg, thumbResizeSettings, "thumb"))
                                            {
                                                hasBeenSaved = true;
                                                // Find and replace existing media
                                                var author = story.Authors.SingleOrDefault(x => x.Media.Irn == mediaIrn);
                                                if (author != null)
                                                    author.Media = media;
                                            }
                                        }
                                    }
                                }

                                // Save any changes
                                partyNarrativeDocumentSession.SaveChanges();
                                count += partyNarrativeBatch.Count;
                            }
                        }
                    }

                    importStatus.CurrentOffset += results.Count;

                    _log.Debug("{0} import progress... {1}/{2}", GetType().Name, importStatus.CurrentOffset, importStatus.CachedResult.Count);
                    documentSession.SaveChanges();

                    tx.Complete();
                }
            }

            using (var documentSession = _documentStore.OpenSession())
            {
                documentSession.Load<Application>(Constants.ApplicationId).ImportFinished(GetType().ToString());
                documentSession.SaveChanges();
            }
        }

        private bool ImportCanceled()
        {
            if (DateTime.Now.TimeOfDay > Constants.ImuOfflineTimeSpan)
            {
                _log.Warn("Imu about to go offline, canceling all imports");
                Program.ImportCanceled = true;
            }

            return Program.ImportCanceled;
        }
    }
}