using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using AutoMapper;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Core.Factories;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Import.Utilities;
using ImageResizer;
using IMu;

namespace CollectionsOnline.Import.Factories
{
    public class StoryImuFactory : IImuFactory<Story>
    {
        private readonly ISlugFactory _slugFactory;

        public StoryImuFactory(
            ISlugFactory slugFactory)
        {
            _slugFactory = slugFactory;            
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
                        "authors=NarAuthorsRef_tab.(NamFullName,BioLabel,media=MulMultiMediaRef_tab.(irn,AdmPublishWebNoPassword))",
                        "contributors=[contributor=NarContributorRef_tab.(NamFullName,BioLabel,media=MulMultiMediaRef_tab.(irn,AdmPublishWebNoPassword)),NarContributorRole_tab]",
                        "media=MulMultiMediaRef_tab.(irn,resource,MulTitle,MulMimeType,MdaDataSets_tab,MdaElement_tab,MdaQualifier_tab,MdaFreeText_tab,ChaRepository_tab,rights=<erights:MulMultiMediaRef_tab>.(RigType,RigAcknowledgement),AdmPublishWebNoPassword,AdmDateModified,AdmTimeModified)",
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

                terms.Add("DetPurpose_tab", "Website - History & Technology Collections");

                return terms;
            }
        }

        public Story MakeDocument(Map map)
        {
            var story = new Story();

            story.Id = "stories/" + map.GetString("irn");

            story.IsHidden = map.GetString("AdmPublishWebNoPassword") == "No";

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

            var authors = new List<Author>();
            authors.AddRange(map.GetMaps("authors").Select(x => new Author
            {
                Name = x.GetString("NamFullName"),
                Biography = x.GetString("BioLabel")
            }));
            authors.AddRange(
                map.GetMaps("contributors")
                   .Where(
                       x =>
                       x.GetString("NarContributorRole_tab") == "Contributor of content" ||
                       x.GetString("NarContributorRole_tab") == "Author of quoted text")
                   .Select(x => x.GetMap("contributor"))
                   .Select(x => new Author
                   {
                       Name = x.GetString("NamFullName"),
                       Biography = x.GetString("BioLabel")
                   }));
            story.Authors = authors;

            // Media
            var media = new List<Media>();
            foreach (var mediaMap in map.GetMaps("media").Where(x => x.GetString("AdmPublishWebNoPassword") == "Yes"))
            {
                var irn = long.Parse(mediaMap.GetString("irn"));
                var fileStream = mediaMap.GetMap("resource")["file"] as FileStream;

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

                if (MediaHelper.Save(fileStream, irn, FileFormatType.Jpg, thumbResizeSettings, "thumb"))
                {
                    media.Add(new Media
                    {
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
            story.Media = media;

            // Relationships
            if (map.GetMap("parent") != null && map.GetMap("parent").GetStrings("DetPurpose_tab").Contains("Website - History & Technology Collections"))
                story.ParentStoryId = "stories/" + map.GetMap("parent").GetString("irn");

            story.ChildStoryIds = map
                .GetMaps("children")
                .Where(x => x != null && x.GetStrings("DetPurpose_tab").Contains("Website - History & Technology Collections"))
                .Select(x => "stories/" + x.GetString("irn"))
                .ToList();
            
            story.RelatedStoryIds = map
                .GetMaps("relatedstories")
                .Where(x => x != null && x.GetStrings("DetPurpose_tab").Contains("Website - History & Technology Collections"))
                .Select(x => "stories/" + x.GetString("irn"))
                .ToList();
             
            story.RelatedItemIds = map
                .GetMaps("relateditems")
                .Where(x => x != null && x.GetStrings("MdaDataSets_tab").Contains("History & Technology Collections Online"))
                .Select(x => "items/" + x.GetString("irn"))
                .ToList();

            // Build summary
            if (!string.IsNullOrWhiteSpace(story.ContentSummary))
                story.Summary = story.ContentSummary.Truncate(Constants.SummaryMaxChars);
            else if (!string.IsNullOrWhiteSpace(story.Content))
                story.Summary = HtmlConverter.HtmlToText(story.Content);
            
            return story;
        }

        public void RegisterAutoMapperMap()
        {
            Mapper.CreateMap<Story, Story>()
                .ForMember(x => x.Id, options => options.Ignore());
        }
    }
}