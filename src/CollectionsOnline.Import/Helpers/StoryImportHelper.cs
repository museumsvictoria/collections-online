using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CollectionsOnline.Core.Factories;
using CollectionsOnline.Core.Models;
using IMu;

namespace CollectionsOnline.Import.Helpers
{
    public class StoryImportHelper : IImportHelper<Story>
    {
        private readonly ISlugFactory _slugFactory;

        public StoryImportHelper(
            ISlugFactory slugFactory)
        {
            _slugFactory = slugFactory;
        }

        public string MakeModuleName()
        {
            return "enarratives";
        }

        public string[] MakeColumns()
        {
            return new[]
                {
                    "irn",
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
                    "media=MulMultiMediaRef_tab.(irn,MulTitle,MulMimeType,MdaDataSets_tab,MdaElement_tab,MdaQualifier_tab,MdaFreeText_tab,ChaRepository_tab,rights=<erights:MulMultiMediaRef_tab>.(RigType,RigAcknowledgement),AdmPublishWebNoPassword,AdmDateModified,AdmTimeModified)",
                    "parent=AssMasterNarrativeRef.(irn)",
                    "relatedstories=AssAssociatedWithRef_tab.(irn)",
                    "relateditems=ObjObjectsRef_tab.(irn)"
                };
        }

        public Terms MakeTerms()
        {
            var terms = new Terms();

            terms.Add("DetPurpose_tab", "Website - History & Technology Collections");
            terms.Add("AdmPublishWebNoPassword", "Yes");

            return terms;
        }

        public Story MakeDocument(Map map)
        {
            var story = new Story(map.GetString("irn"));

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
                media.Add(new Media
                {
                    DateModified =
                        DateTime.ParseExact(
                            string.Format("{0} {1}", mediaMap.GetString("AdmDateModified"),
                                          mediaMap.GetString("AdmTimeModified")), "dd/MM/yyyy HH:mm",
                            new CultureInfo("en-AU")),
                    Title = mediaMap.GetString("MulTitle"),
                    Type = mediaMap.GetString("MulMimeType")
                });
            }
            story.Media = media;

            // Relationships
            if (map.GetMap("parent") != null)
                story.ParentStoryId = "stories/" + map.GetMap("parent").GetString("irn");
            story.RelatedStoryIds = map.GetMaps("relatedstories").Where(x => x != null).Select(x => "stories/" + x.GetString("irn")).ToList();
            story.RelatedItemIds = map.GetMaps("relateditems").Where(x => x != null).Select(x => "items/" + x.GetString("irn")).ToList();
            
            return story;
        }
    }
}