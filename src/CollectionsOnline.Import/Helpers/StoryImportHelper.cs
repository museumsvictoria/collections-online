using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CollectionsOnline.Core.DomainModels;
using CollectionsOnline.Core.Factories;
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
                    "contributors=[contributor=NarContributorRef_tab.(NamFullName,BioLabel,media=MulMultiMediaRef_tab.(irn,AdmPublishWebNoPassword)),NarContributorRole_tab]"
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
            
            return story;
        }
    }
}