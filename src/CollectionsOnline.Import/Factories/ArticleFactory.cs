using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using AutoMapper;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Core.Factories;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Import.Extensions;
using CollectionsOnline.Import.Utilities;
using IMu;
using NLog;
using Raven.Abstractions.Extensions;

namespace CollectionsOnline.Import.Factories
{
    public class ArticleFactory : IEmuAggregateRootFactory<Article>
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly ISlugFactory _slugFactory;
        private readonly IMediaFactory _mediaFactory;

        public ArticleFactory(
            ISlugFactory slugFactory,
            IMediaFactory mediaFactory)
        {
            _slugFactory = slugFactory;
            _mediaFactory = mediaFactory;
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
                        "authors=NarAuthorsRef_tab.(NamFullName,BioLabel,media=MulMultiMediaRef_tab.(irn,MulTitle,MulMimeType,MdaDataSets_tab,metadata=[MdaElement_tab,MdaQualifier_tab,MdaFreeText_tab],DetAlternateText,RigCreator_tab,RigSource_tab,RigAcknowledgementCredit,RigCopyrightStatement,RigCopyrightStatus,RigLicence,RigLicenceDetails,AdmPublishWebNoPassword,AdmDateModified,AdmTimeModified))",
                        "contributors=[contributor=NarContributorRef_tab.(NamFullName,BioLabel),NarContributorRole_tab]",
                        "media=MulMultiMediaRef_tab.(irn,MulTitle,MulMimeType,MdaDataSets_tab,metadata=[MdaElement_tab,MdaQualifier_tab,MdaFreeText_tab],DetAlternateText,RigCreator_tab,RigSource_tab,RigAcknowledgementCredit,RigCopyrightStatement,RigCopyrightStatus,RigLicence,RigLicenceDetails,AdmPublishWebNoPassword,AdmDateModified,AdmTimeModified)",
                        "parent=AssMasterNarrativeRef.(irn,DetPurpose_tab)",
                        "children=<enarratives:AssMasterNarrativeRef>.(irn,DetPurpose_tab)",
                        "relatedarticles=AssAssociatedWithRef_tab.(irn,DetPurpose_tab)",
                        "relateditemspecimens=ObjObjectsRef_tab.(irn,MdaDataSets_tab)"
                    };
            }
        }

        public Terms Terms
        {
            get
            {
                var terms = new Terms();

                terms.Add("DetPurpose_tab", Constants.ImuArticleQueryString);

                return terms;
            }
        }

        public Article MakeDocument(Map map)
        {
            var stopwatch = Stopwatch.StartNew();

            var article = new Article();

            article.Id = "articles/" + map.GetEncodedString("irn");

            article.IsHidden = string.Equals(map.GetEncodedString("AdmPublishWebNoPassword"), "no", StringComparison.OrdinalIgnoreCase);

            article.DateModified = DateTime.ParseExact(
                string.Format("{0} {1}", map.GetEncodedString("AdmDateModified"), map.GetEncodedString("AdmTimeModified")),
                "dd/MM/yyyy HH:mm",
                new CultureInfo("en-AU"));
            article.Title = map.GetEncodedString("NarTitle");
            article.Keywords.AddRange(map.GetEncodedStrings("DesSubjects_tab"));
            article.Content = map.GetEncodedString("NarNarrative");
            article.ContentSummary = map.GetEncodedString("NarNarrativeSummary");
            article.Types.AddRange(map.GetEncodedStrings("DesType_tab").Where(x => !string.IsNullOrWhiteSpace(x)));
            article.Keywords.AddRange(map.GetEncodedStrings("DesGeographicLocation_tab"));

            // Authors
            article.Authors = map.GetMaps("authors")
                .Where(x => x != null)
                .Select(x => new Author
                {
                    Name = x.GetEncodedString("NamFullName"),
                    Biography = x.GetEncodedString("BioLabel"),
                    ProfileImage = _mediaFactory.Make(x.GetMaps("media").FirstOrDefault()) as ImageMedia
                }).ToList();

            // Contributors
            article.Contributors.AddRange(
                map.GetMaps("contributors")
                   .Where(
                       x =>
                       x.GetEncodedString("NarContributorRole_tab").Contains("contributor of content", StringComparison.OrdinalIgnoreCase) ||
                       x.GetEncodedString("NarContributorRole_tab").Contains("author of quoted text", StringComparison.OrdinalIgnoreCase) ||
                       x.GetEncodedString("NarContributorRole_tab").Contains("researcher", StringComparison.OrdinalIgnoreCase))
                   .Select(x => x.GetMap("contributor"))
                   .Select(x => new Author
                   {
                       Name = x.GetEncodedString("NamFullName"),
                       Biography = x.GetEncodedString("BioLabel")
                   }));

            // Media           
            article.Media = _mediaFactory.Make(map.GetMaps("media"));

            var thumbnail = article.Media.FirstOrDefault(x => x is ImageMedia) as ImageMedia;
            if (thumbnail != null)
                article.ThumbnailUri = thumbnail.Thumbnail.Uri;

            // Relationships

            // parent article
            if (map.GetMap("parent") != null && map.GetMap("parent").GetEncodedStrings("DetPurpose_tab").Contains(Constants.ImuArticleQueryString))
                article.ParentArticleId = "articles/" + map.GetMap("parent").GetEncodedString("irn");

            // child article
            article.ChildArticleIds = map
                .GetMaps("children")
                .Where(x => x != null && x.GetEncodedStrings("DetPurpose_tab").Contains(Constants.ImuArticleQueryString))
                .Select(x => "articles/" + x.GetEncodedString("irn"))
                .ToList();

            // sibling article
            article.RelatedArticleIds = map
                .GetMaps("relatedarticles")
                .Where(x => x != null && x.GetEncodedStrings("DetPurpose_tab").Contains(Constants.ImuArticleQueryString))
                .Select(x => "articles/" + x.GetEncodedString("irn"))
                .ToList();

            // Related items/specimens (directly related)
            foreach (var relatedItemSpecimen in map.GetMaps("relateditemspecimens").Where(x => x != null && !string.IsNullOrWhiteSpace(x.GetEncodedString("irn"))))
            {
                if (relatedItemSpecimen.GetEncodedStrings("MdaDataSets_tab").Contains(Constants.ImuItemQueryString))
                    article.RelatedItemIds.Add(string.Format("items/{0}", relatedItemSpecimen.GetEncodedString("irn")));
                if (relatedItemSpecimen.GetEncodedStrings("MdaDataSets_tab").Contains(Constants.ImuSpecimenQueryString))
                    article.RelatedSpecimenIds.Add(string.Format("specimens/{0}", relatedItemSpecimen.GetEncodedString("irn")));
            }
            // Build summary
            if (!string.IsNullOrWhiteSpace(article.ContentSummary))
                article.Summary = article.ContentSummary;
            else if (!string.IsNullOrWhiteSpace(article.Content))
            {
                try
                {
                    article.Summary = HtmlConverter.HtmlToText(article.Content);
                }
                catch (Exception e)
                {
                    _log.Warn("Unable to convert article content html to text, irn:{0}, html:{0}, exception:{1}", map.GetEncodedString("irn"), article.Content, e);
                }
            }

            stopwatch.Stop();
            _log.Trace("Completed article creation for narrative record with irn {0}, elapsed time {1} ms", map.GetEncodedString("irn"), stopwatch.ElapsedMilliseconds);

            return article;
        }

        public void RegisterAutoMapperMap()
        {
            Mapper.CreateMap<Article, Article>()
                .ForMember(x => x.Id, options => options.Ignore());
        }
    }
}