using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using AutoMapper;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Core.Factories;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Import.Utilities;
using ImageResizer;
using IMu;
using NLog;

namespace CollectionsOnline.Import.Factories
{
    public class StoryImuFactory : IImuFactory<Story>
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        private readonly ISlugFactory _slugFactory;
        private readonly IMediaHelper _mediaHelper;

        public StoryImuFactory(
            ISlugFactory slugFactory,
            IMediaHelper mediaHelper)
        {
            _slugFactory = slugFactory;
            _mediaHelper = mediaHelper;
        }

        public string ModuleName
        {
            get { return "enarratives"; }
        }

        public string[] Columns
        {
            get
            {
                return new[]
                    {
                        "irn",
                        "AdmPublishWebNoPassword",
                        "AdmDateModified",
                        "AdmTimeModified",
                        "NarTitle",
                        "DesSubjects_tab",
                        "NarNarrative",
                        "NarNarrativeSummary",
                        "DesType_tab",
                        "DesGeographicLocation_tab",
                        "authors=NarAuthorsRef_tab.(NamFullName,BioLabel,media=MulMultiMediaRef_tab.(irn,MulTitle,MulMimeType,MulDescription,MulCreator_tab,MdaDataSets_tab,MdaElement_tab,MdaQualifier_tab,MdaFreeText_tab,ChaRepository_tab,AdmPublishWebNoPassword,AdmDateModified,AdmTimeModified))",
                        "contributors=[contributor=NarContributorRef_tab.(NamFullName,BioLabel),NarContributorRole_tab]",
                        "media=MulMultiMediaRef_tab.(irn,MulTitle,MulMimeType,MulDescription,MulCreator_tab,MdaDataSets_tab,MdaElement_tab,MdaQualifier_tab,MdaFreeText_tab,ChaRepository_tab,AdmPublishWebNoPassword,AdmDateModified,AdmTimeModified)",
                        "parent=AssMasterNarrativeRef.(irn,DetPurpose_tab)",
                        "children=<enarratives:AssMasterNarrativeRef>.(irn,DetPurpose_tab)",
                        "relatedstories=AssAssociatedWithRef_tab.(irn,DetPurpose_tab)",
                        "relateditems=ObjObjectsRef_tab.(irn,MdaDataSets_tab)"
                    };
            }
        }

        public Terms Terms
        {
            get
            {
                var terms = new Terms();

                terms.Add("DetPurpose_tab", Constants.ImuStoryQueryString);

                return terms;
            }
        }

        public Story MakeDocument(Map map)
        {
            var story = new Story();

            story.Id = "stories/" + map.GetString("irn");

            story.IsHidden = string.Equals(map.GetString("AdmPublishWebNoPassword"), "no", StringComparison.OrdinalIgnoreCase);

            story.DateModified = DateTime.ParseExact(
                string.Format("{0} {1}", map.GetString("AdmDateModified"), map.GetString("AdmTimeModified")),
                "dd/MM/yyyy HH:mm",
                new CultureInfo("en-AU"));
            story.Title = map.GetString("NarTitle");
            story.Tags = map.GetStrings("DesSubjects_tab").Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => _slugFactory.MakeSlug(x)).ToArray();
            story.Content = map.GetString("NarNarrative");
            story.ContentSummary = map.GetString("NarNarrativeSummary");
            story.Types = map.GetStrings("DesType_tab").Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            story.GeographicTags = map.GetStrings("DesGeographicLocation_tab").Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => _slugFactory.MakeSlug(x)).ToArray();

            // Authors
            var authors = new List<Author>();
            foreach (var authorMap in map.GetMaps("authors"))
            {
                var author = new Author
                {
                    Name = authorMap.GetString("NamFullName"),
                    Biography = authorMap.GetString("BioLabel")
                };

                var mediaMap = authorMap.GetMaps("media").FirstOrDefault(x => x != null &&
                    string.Equals(x.GetString("AdmPublishWebNoPassword"), "yes", StringComparison.OrdinalIgnoreCase) &&
                    x.GetString("MulMimeType") == "image");
                if (mediaMap != null)
                {
                    var irn = long.Parse(mediaMap.GetString("irn"));

                    var url = PathFactory.GetUrlPath(irn, FileFormatType.Jpg, "thumb");
                    var thumbResizeSettings = new ResizeSettings
                    {
                        Format = FileFormatType.Jpg.ToString(),
                        Height = 365,
                        Width = 365,
                        Mode = FitMode.Crop,
                        PaddingColor = Color.White,
                        Quality = 65
                    };

                    if (_mediaHelper.Save(irn, FileFormatType.Jpg, thumbResizeSettings, "thumb"))
                    {
                        author.Media = new Media
                        {
                            Irn = irn,
                            DateModified =
                                DateTime.ParseExact(
                                    string.Format("{0} {1}", mediaMap.GetString("AdmDateModified"),
                                                  mediaMap.GetString("AdmTimeModified")), "dd/MM/yyyy HH:mm",
                                    new CultureInfo("en-AU")),
                            Title = mediaMap.GetString("MulTitle"),
                            Type = mediaMap.GetString("MulMimeType"),
                            Url = url
                        };

                        authors.Add(author);
                    }
                }
            }
            story.Authors = authors;

            // Contributors
            authors.AddRange(
                map.GetMaps("contributors")
                   .Where(
                       x =>
                       x.GetString("NarContributorRole_tab").Contains("contributor of content", StringComparison.OrdinalIgnoreCase) ||
                       x.GetString("NarContributorRole_tab").Contains("author of quoted text", StringComparison.OrdinalIgnoreCase) ||
                       x.GetString("NarContributorRole_tab").Contains("researcher", StringComparison.OrdinalIgnoreCase))
                   .Select(x => x.GetMap("contributor"))
                   .Select(x => new Author
                   {
                       Name = x.GetString("NamFullName"),
                       Biography = x.GetString("BioLabel")
                   }));
            story.Authors = authors;

            // Media
            // TODO: Be more selective in what media we assign to item and how
            story.Media = new List<Media>();
            foreach (var mediaMap in map.GetMaps("media").Where(x =>
                x != null &&
                string.Equals(x.GetString("AdmPublishWebNoPassword"), "yes", StringComparison.OrdinalIgnoreCase) && 
                x.GetString("MulMimeType") == "image"))
            {
                var irn = long.Parse(mediaMap.GetString("irn"));

                var url = PathFactory.GetUrlPath(irn, FileFormatType.Jpg, "thumb");
                var thumbResizeSettings = new ResizeSettings
                {
                    Format = FileFormatType.Jpg.ToString(),
                    Height = 365,
                    Width = 365,
                    Mode = FitMode.Crop,
                    PaddingColor = Color.White,
                    Quality = 65
                };

                if (_mediaHelper.Save(irn, FileFormatType.Jpg, thumbResizeSettings, "thumb"))
                {
                    story.Media.Add(new Media
                    {
                        Irn = irn,
                        DateModified =
                            DateTime.ParseExact(
                                string.Format("{0} {1}", mediaMap.GetString("AdmDateModified"),
                                              mediaMap.GetString("AdmTimeModified")), "dd/MM/yyyy HH:mm",
                                new CultureInfo("en-AU")),
                        Title = mediaMap.GetString("MulTitle"),
                        Type = mediaMap.GetString("MulMimeType"),
                        Url = url
                    });
                }
            }

            // Relationships
            if (map.GetMap("parent") != null && map.GetMap("parent").GetStrings("DetPurpose_tab").Contains(Constants.ImuStoryQueryString))
                story.ParentStoryId = "stories/" + map.GetMap("parent").GetString("irn");

            story.ChildStoryIds = map
                .GetMaps("children")
                .Where(x => x != null && x.GetStrings("DetPurpose_tab").Contains(Constants.ImuStoryQueryString))
                .Select(x => "stories/" + x.GetString("irn"))
                .ToList();
            
            story.RelatedStoryIds = map
                .GetMaps("relatedstories")
                .Where(x => x != null && x.GetStrings("DetPurpose_tab").Contains(Constants.ImuStoryQueryString))
                .Select(x => "stories/" + x.GetString("irn"))
                .ToList();
             
            story.RelatedItemIds = map
                .GetMaps("relateditems")
                .Where(x => x != null && x.GetStrings("MdaDataSets_tab").Contains(Constants.ImuItemQueryString))
                .Select(x => "items/" + x.GetString("irn"))
                .ToList();

            // Build summary
            if (!string.IsNullOrWhiteSpace(story.ContentSummary))
                story.Summary = story.ContentSummary;
            else if (!string.IsNullOrWhiteSpace(story.Content))
            {
                try
                {
                    story.Summary = HtmlConverter.HtmlToText(story.Content);
                }
                catch (Exception e)
                {
                    _log.Warn("Unable to convert story content html to text, irn:{0}, html:{0}, exception:{1}", map.GetString("irn"), story.Content, e);
                }
            }

            return story;
        }

        public void RegisterAutoMapperMap()
        {
            Mapper.CreateMap<Story, Story>()
                .ForMember(x => x.Id, options => options.Ignore());
        }
    }
}