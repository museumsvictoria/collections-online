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

        public void Run(DateTime? dateLastRun)
        {
            // Media update only happens if import has been run before
            if (dateLastRun.HasValue)
            {
                _log.Debug("Beginning Media update import");

                var module = new Module("emultimedia", _session);
                
                //var terms = new Terms();
                //terms.Add("AdmDateModified", dateLastRun.Value.ToString("MMM dd yyyy"), ">=");
                
                var terms = new Terms(TermsKind.OR);

                terms.Add("irn", "14797"); // item irn 734059
                terms.Add("irn", "341327"); // specimen irn 1005339
                terms.Add("irn", "389615"); // species irn 8037
                terms.Add("irn", "78937"); // stories irn 1842
            
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
                                    "rights=<erights:MulMultiMediaRef_tab>.(RigType,RigAcknowledgement)",
                                    "AdmPublishWebNoPassword",
                                    "AdmDateModified",
                                    "AdmTimeModified",
                                    "catalogue=<ecatalogue:MulMultiMediaRef_tab>.(irn,sets=MdaDataSets_tab)",
                                    "narrative=<enarratives:MulMultiMediaRef_tab>.(irn,sets=DetPurpose_tab)"
                                };

                var hits = module.FindTerms(terms);
                _log.Debug("Finished Search. {0} Hits", hits);

                var count = 0;

                while (true)
                {
                    using (var documentSession = _documentStore.OpenSession())
                    {
                        if (Program.ImportCanceled)
                        {
                            _log.Debug("Canceling Data import");
                            return;
                        }

                        var results = module.Fetch("start", count, Constants.DataBatchSize, columns);

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
                                if (sets.Any(x => x == "History & Technology Collections Online" || x == "Collections Online -  Indigenous Cultures" || x == "Collections Online - Natural Sciences"))
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
                                if (sets.Any(x => x == "Website - Atlas of Living Australia"))
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
                                if (sets.Any(x => x == "Website - Species profile"))
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
                                if (sets.Any(x => x == "Website - History & Technology Collections"))
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

                        // Save any changes
                        documentSession.SaveChanges();
                        count += results.Count;
                        _log.Debug("import progress... {0}/{1}", count, hits);
                    }
                }
            }
        }
    }
}