﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using AutoMapper;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Import.Extensions;
using CollectionsOnline.Import.Utilities;
using IMu;
using NLog;
using Raven.Abstractions.Commands;
using Raven.Abstractions.Data;
using Raven.Abstractions.Extensions;
using Raven.Client;
using Constants = CollectionsOnline.Core.Config.Constants;

namespace CollectionsOnline.Import.Factories
{
    public class ArticleFactory : IEmuAggregateRootFactory<Article>
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly IMediaFactory _mediaFactory;

        public ArticleFactory(
            IMediaFactory mediaFactory)
        {
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
                        "authors=NarAuthorsRef_tab.(NamFirst,NamLast,NamFullName,BioLabel,media=MulMultiMediaRef_tab.(irn,MulTitle,MulIdentifier,MulMimeType,MdaDataSets_tab,metadata=[MdaElement_tab,MdaQualifier_tab,MdaFreeText_tab],DetAlternateText,RigCreator_tab,RigSource_tab,RigAcknowledgementCredit,RigCopyrightStatement,RigCopyrightStatus,RigLicence,RigLicenceDetails,ChaRepository_tab,AdmPublishWebNoPassword,AdmDateModified,AdmTimeModified))",
                        "contributors=[contributor=NarContributorRef_tab.(NamFirst,NamLast,NamFullName,BioLabel),NarContributorRole_tab]",
                        "media=MulMultiMediaRef_tab.(irn,MulTitle,MulIdentifier,MulMimeType,MdaDataSets_tab,metadata=[MdaElement_tab,MdaQualifier_tab,MdaFreeText_tab],DetAlternateText,RigCreator_tab,RigSource_tab,RigAcknowledgementCredit,RigCopyrightStatement,RigCopyrightStatus,RigLicence,RigLicenceDetails,ChaRepository_tab,AdmPublishWebNoPassword,AdmDateModified,AdmTimeModified)",
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

            var sanitizedResult = HtmlConverter.HtmlSanitizer(map.GetEncodedString("NarNarrative"));

            article.Content = sanitizedResult.Html;

            try
            {
                article.ContentText = HtmlConverter.HtmlToText(article.Content);
            }
            catch (Exception exception)
            {
                _log.Warn("Unable to convert article content html to text, irn:{0}, html:{1}, exception:{2}", map.GetEncodedString("irn"), article.Content, exception);
            }

            if(sanitizedResult.HasRemovedTag || sanitizedResult.HasRemovedStyle || sanitizedResult.HasRemovedAttribute)
                _log.Trace("Suspected obsolete HTML, consider reviewing narrative with irn {0} (removedTag:{1}, removedStyle:{2}, removedAttribute:{3})", 
                    map.GetEncodedString("irn"), 
                    sanitizedResult.HasRemovedTag, 
                    sanitizedResult.HasRemovedStyle, 
                    sanitizedResult.HasRemovedAttribute);
            
            article.ContentSummary = map.GetEncodedString("NarNarrativeSummary");

            // Article types (Remove problematic formatting that is used in facets)
            article.Types.AddRange(map.GetEncodedStrings("DesType_tab").Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.CleanForMultiFacets()));

            // Localities
            article.Localities.AddRange(map.GetEncodedStrings("DesGeographicLocation_tab"));

            // Authors
            article.Authors = map.GetMaps("authors")
                .Where(x => x != null)
                .Select(x => new Author
                {
                    FirstName = x.GetEncodedString("NamFirst"),
                    LastName = x.GetEncodedString("NamLast"),
                    FullName = x.GetEncodedString("NamFullName"),
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
                       FirstName = x.GetEncodedString("NamFirst"),
                       LastName = x.GetEncodedString("NamLast"),
                       FullName = x.GetEncodedString("NamFullName"),
                       Biography = x.GetEncodedString("BioLabel")
                   }));

            // Media           
            article.Media = _mediaFactory.Make(map.GetMaps("media"));

            // Assign thumbnail
            var media = article.Media.FirstOrDefault(x => x is ImageMedia || x is VideoMedia);

            var image = media as VideoMedia;
            if (image != null)
                article.ThumbnailUri = image.Thumbnail.Uri;
            
            var video = media as VideoMedia;
            if (video != null)
                article.ThumbnailUri = video.Thumbnail.Uri;

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
                article.Summary = article.ContentText;

            // Display Title
            // TODO: Move to display title factory and encapsulate entire process
            if (!string.IsNullOrWhiteSpace(article.Title))
                article.DisplayTitle = article.Title;

            if (string.IsNullOrWhiteSpace(article.DisplayTitle))
                article.DisplayTitle = "Article";

            stopwatch.Stop();
            _log.Trace("Completed article creation for narrative record with irn {0}, elapsed time {1} ms", map.GetEncodedString("irn"), stopwatch.ElapsedMilliseconds);

            return article;
        }

        public void UpdateDocument(Article newDocument, Article existingDocument, IDocumentSession documentSession)
        {
            //TODO: because related id's can be from different relationships in emu, it is possible to remove a legitimate relationship when updating. consider splitting related id's into different relationships (related party articles, related sites articles)

            // Perform any denormalized updates
            var patchCommands = new List<ICommandData>();

            // Related Items update
            foreach (var itemIdtoRemove in existingDocument.RelatedItemIds.Except(newDocument.RelatedItemIds))
            {
                patchCommands.Add(new PatchCommandData
                {
                    Key = itemIdtoRemove,
                    Patches = new[]
                    {
                        new PatchRequest
                        {
                            Type = PatchCommandType.Remove,
                            AllPositions = true,
                            Name = "RelatedArticleIds",
                            Value = newDocument.Id
                        }
                    }
                });
            }
            foreach (var itemIdToAdd in newDocument.RelatedItemIds.Except(existingDocument.RelatedItemIds))
            {
                patchCommands.Add(new PatchCommandData
                {
                    Key = itemIdToAdd,
                    Patches = new[]
                    {
                        new PatchRequest
                        {
                            Type = PatchCommandType.Add,
                            Name = "RelatedArticleIds",
                            Value = newDocument.Id
                        }
                    }
                });
            }

            // Related Specimen update
            foreach (var specimenIdtoRemove in existingDocument.RelatedSpecimenIds.Except(newDocument.RelatedSpecimenIds))
            {
                patchCommands.Add(new PatchCommandData
                {
                    Key = specimenIdtoRemove,
                    Patches = new[]
                    {
                        new PatchRequest
                        {
                            Type = PatchCommandType.Remove,
                            AllPositions = true,
                            Name = "RelatedArticleIds",
                            Value = newDocument.Id
                        }
                    }
                });
            }
            foreach (var specimenIdtoAdd in newDocument.RelatedSpecimenIds.Except(existingDocument.RelatedSpecimenIds))
            {
                patchCommands.Add(new PatchCommandData
                {
                    Key = specimenIdtoAdd,
                    Patches = new[]
                    {
                        new PatchRequest
                        {
                            Type = PatchCommandType.Add,
                            Name = "RelatedArticleIds",
                            Value = newDocument.Id
                        }
                    }
                });
            }

            // Parent Article update
            if (!string.Equals(newDocument.ParentArticleId, existingDocument.ParentArticleId, StringComparison.OrdinalIgnoreCase))
            {
                patchCommands.Add(new PatchCommandData
                {
                    Key = existingDocument.ParentArticleId,
                    Patches = new[]
                    {
                        new PatchRequest
                        {
                            Type = PatchCommandType.Remove,
                            AllPositions = true,
                            Name = "ChildArticleIds",
                            Value = newDocument.Id
                        }
                    }
                });
                patchCommands.Add(new PatchCommandData
                {
                    Key = newDocument.ParentArticleId,
                    Patches = new[]
                    {
                        new PatchRequest
                        {
                            Type = PatchCommandType.Add,
                            Name = "ChildArticleIds",
                            Value = newDocument.Id
                        }
                    }
                });
            }

            documentSession.Advanced.Defer(patchCommands.ToArray());

            // Map over existing document
            Mapper.Map(newDocument, existingDocument);
        }
    }
}