using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Import.Factories;
using CollectionsOnline.Import.Utilities;
using ImageResizer;
using IMu;
using NLog;
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

        public void Run(DateTime? previousDateRun)
        {
            // Media update only happens if import has been run before
            if (previousDateRun.HasValue)
            {
                _log.Debug("Beginning Media update import");

                var module = new Module("emultimedia", _session);
                var terms = new Terms();
                terms.Add("AdmDateModified", previousDateRun.Value.ToString("MMM dd yyyy"), ">=");
                int currentOffset;
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
                                    "narrative=<enarratives:MulMultiMediaRef_tab>.(irn,sets=DetPurpose_tab)"
                                };

                // Check for existing import in case we need to resume.
                using (var documentSession = _documentStore.OpenSession())
                {
                    var importProgress = documentSession.Load<Application>(Constants.ApplicationId).GetImportProgress(GetType().ToString());

                    // Exit current import if it had completed last time it was run.
                    if (importProgress.IsFinished)
                    {
                        return;
                    }

                    currentOffset = importProgress.CurrentOffset;
                    terms.Add("AdmDateModified", importProgress.DateRun.ToString("MMM dd yyyy"), "<");

                    documentSession.SaveChanges();
                }

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

                        var results = module.Fetch("start", currentOffset, Constants.DataBatchSize, columns);

                        if (results.Count == 0)
                            break;

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
                                Url = PathFactory.GetUrlPath(mediaIrn, FileFormatType.Jpg, "thumb")
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

                            var catalogues = row.GetMaps("catalogue");
                            foreach (var catalogue in catalogues)
                            {
                                var catalogueIrn = long.Parse(catalogue.GetString("irn"));
                                var sets = catalogue.GetStrings("sets");

                                // TODO: update once we have consistent flag values for items in emu
                                if (sets.Any(x => x == Constants.ImuItemQueryString || x == "Collections Online -  Indigenous Cultures" || x == "Collections Online - Natural Sciences"))
                                {
                                    // Item Media
                                    var item = documentSession.Load<Item>(catalogueIrn);

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
                                if (sets.Any(x => x == Constants.ImuSpecimenQueryString))
                                {
                                    // Specimen media
                                    var specimen = documentSession.Load<Specimen>(catalogueIrn);

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

                            var narratives = row.GetMaps("narrative");
                            foreach (var narrative in narratives)
                            {
                                var narrativeIrn = long.Parse(narrative.GetString("irn"));
                                var sets = narrative.GetStrings("sets");
                                if (sets.Any(x => x == Constants.ImuSpeciesQueryString))
                                {
                                    // Species Media
                                    var species = documentSession.Load<Species>(narrativeIrn);

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
                                if (sets.Any(x => x == Constants.ImuStoryQueryString))
                                {
                                    // Story media
                                    var story = documentSession.Load<Story>(narrativeIrn);

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
                        }

                        currentOffset += results.Count;

                        documentSession.Load<Application>(Constants.ApplicationId).UpdateImportCurrentOffset(GetType().ToString(), currentOffset);

                        _log.Debug("import progress... {0}/{1}", currentOffset, hits);
                        documentSession.SaveChanges();
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
}