using System;
using System.Globalization;
using System.Linq;
using AutoMapper;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Core.Utilities;
using CollectionsOnline.Import.Extensions;
using IMu;
using Raven.Client;
using Serilog;
using Constants = CollectionsOnline.Core.Config.Constants;

namespace CollectionsOnline.Import.Factories
{
    public class CollectionFactory : IEmuAggregateRootFactory<Collection>
    {
        private readonly IMediaFactory _mediaFactory;

        public CollectionFactory(
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
                        "NarNarrative",
                        "NarNarrativeSummary",
                        "DetNarrativeIdentifier",
                        "authors=NarAuthorsRef_tab.(NamFirst,NamLast,NamFullName,BioLabel,media=MulMultiMediaRef_tab.(irn,MulTitle,MulIdentifier,MulMimeType,MdaDataSets_tab,metadata=[MdaElement_tab,MdaQualifier_tab,MdaFreeText_tab],DetAlternateText,RigCreator_tab,RigSource_tab,RigAcknowledgementCredit,RigCopyrightStatement,RigCopyrightStatus,RigLicence,RigLicenceDetails,ChaRepository_tab,ChaMd5Sum,AdmPublishWebNoPassword,AdmDateModified,AdmTimeModified))",
                        "curators=[curator=NarContributorRef_tab.(NamFirst,NamLast,NamFullName,BioLabel,media=MulMultiMediaRef_tab.(irn,MulTitle,MulIdentifier,MulMimeType,MdaDataSets_tab,metadata=[MdaElement_tab,MdaQualifier_tab,MdaFreeText_tab],DetAlternateText,RigCreator_tab,RigSource_tab,RigAcknowledgementCredit,RigCopyrightStatement,RigCopyrightStatus,RigLicence,RigLicenceDetails,ChaRepository_tab,ChaMd5Sum,AdmPublishWebNoPassword,AdmDateModified,AdmTimeModified)),NarContributorRole_tab]",
                        "media=MulMultiMediaRef_tab.(irn,MulTitle,MulIdentifier,MulMimeType,MdaDataSets_tab,metadata=[MdaElement_tab,MdaQualifier_tab,MdaFreeText_tab],DetAlternateText,RigCreator_tab,RigSource_tab,RigAcknowledgementCredit,RigCopyrightStatement,RigCopyrightStatus,RigLicence,RigLicenceDetails,ChaRepository_tab,ChaMd5Sum,AdmPublishWebNoPassword,AdmDateModified,AdmTimeModified)",
                        "favorites=[itemspecimen=ObjObjectsRef_tab.(irn,MdaDataSets_tab),ObjObjectNotes_tab]",
                        "subcollections=[article=AssAssociatedWithRef_tab.(irn,DetPurpose_tab),AssAssociatedWithComment_tab]"
                    };
            }
        }

        public Terms Terms
        {
            get
            {
                var terms = new Terms();

                terms.Add("DetPurpose_tab", Constants.ImuCollectionQueryString);

                return terms;
            }
        }

        public Collection MakeDocument(Map map)
        {
            var collection = new Collection();

            collection.Id = "collections/" + map.GetEncodedString("irn");

            collection.IsHidden = string.Equals(map.GetEncodedString("AdmPublishWebNoPassword"), "no", StringComparison.OrdinalIgnoreCase);

            collection.DateModified = DateTime.ParseExact(
                string.Format("{0} {1}", map.GetEncodedString("AdmDateModified"), map.GetEncodedString("AdmTimeModified")),
                "dd/MM/yyyy HH:mm",
                new CultureInfo("en-AU")).ToUniversalTime();
            collection.Title = map.GetEncodedString("NarTitle");

            var sanitizedResult = HtmlConverter.HtmlSanitizer(map.GetEncodedString("NarNarrative"));

            collection.Significance = sanitizedResult.Html;

            if(sanitizedResult.HasRemovedTag || sanitizedResult.HasRemovedStyle || sanitizedResult.HasRemovedAttribute)
                Log.Logger.Warning("Poorly formatted HTML detected, consider reviewing {Id} {@Reason}", collection.Id, new { sanitizedResult.HasRemovedTag, sanitizedResult.HasRemovedStyle, sanitizedResult.HasRemovedAttribute });

            collection.CollectionSummary = map.GetEncodedString("NarNarrativeSummary");

            collection.Category = map.GetEncodedString("DetNarrativeIdentifier");

            // Authors
            collection.Authors = map.GetMaps("authors")
                .Where(x => x != null)
                .Select(x => new Author
                {
                    FirstName = x.GetEncodedString("NamFirst"),
                    LastName = x.GetEncodedString("NamLast"),
                    FullName = x.GetEncodedString("NamFullName"),
                    Biography = x.GetEncodedString("BioLabel"),
                    ProfileImage = _mediaFactory.Make(x.GetMaps("media").FirstOrDefault()) as ImageMedia
                }).ToList();

            foreach (var curators in map.GetMaps("curators").Where(x => x != null))
            {
                var curatorMap = curators.GetMap("curator");

                if (string.Equals(curators.GetEncodedString("NarContributorRole_tab"), "Contributor", StringComparison.OrdinalIgnoreCase))
                collection.Curators.Add(new Author
                {
                    FirstName = curatorMap.GetEncodedString("NamFirst"),
                    LastName = curatorMap.GetEncodedString("NamLast"),
                    FullName = curatorMap.GetEncodedString("NamFullName"),
                    Biography = curatorMap.GetEncodedString("BioLabel"),
                    ProfileImage = _mediaFactory.Make(curatorMap.GetMaps("media").FirstOrDefault()) as ImageMedia
                });
            }

            // Media           
            collection.Media = _mediaFactory.Make(map.GetMaps("media"));

            // Assign thumbnail
            var media = collection.Media.OfType<IHasThumbnail>().FirstOrDefault();
            if (media != null)
                collection.ThumbnailUri = media.Thumbnail.Uri;

            // Relationships

            // Favorites
            foreach (var favorite in map.GetMaps("favorites").Where(x => x != null))
            {
                var itemSpecimenMap = favorite.GetMap("itemspecimen");
                if (itemSpecimenMap.GetEncodedStrings("MdaDataSets_tab").Contains(Constants.ImuItemQueryString))
                    collection.FavoriteItems.Add(new EmuSummary
                    {
                        Id = string.Format("items/{0}", itemSpecimenMap.GetEncodedString("irn")),
                        Summary = favorite.GetEncodedString("ObjObjectNotes_tab")
                    });                    
                if (itemSpecimenMap.GetEncodedStrings("MdaDataSets_tab").Contains(Constants.ImuSpecimenQueryString))
                    collection.FavoriteSpecimens.Add(new EmuSummary
                    {
                        Id = string.Format("specimens/{0}", itemSpecimenMap.GetEncodedString("irn")),
                        Summary = favorite.GetEncodedString("ObjObjectNotes_tab")
                    });
            }

            // Sub-Collections
            foreach (var subCollection in map.GetMaps("subcollections").Where(x => x != null))
            {
                var articleMap = subCollection.GetMap("article");
                if (articleMap.GetEncodedStrings("DetPurpose_tab").Contains(Constants.ImuArticleQueryString))
                    collection.SubCollectionArticles.Add(new EmuSummary
                    {
                        Id = string.Format("articles/{0}", articleMap.GetEncodedString("irn")),
                        Summary = subCollection.GetEncodedString("AssAssociatedWithComment_tab")
                    });
            }

            // Collection Area
            collection.CollectingArea = collection.Title.Replace("Collection", "").Trim();

            // Build summary
            if (!string.IsNullOrWhiteSpace(collection.CollectionSummary))
                collection.Summary = collection.CollectionSummary;
            else if (!string.IsNullOrWhiteSpace(collection.Significance))
                collection.Summary = collection.Significance;

            // Display Title
            if (!string.IsNullOrWhiteSpace(collection.Title))
                collection.DisplayTitle = collection.Title;
            else
                collection.DisplayTitle = "Collection Overview";

            Log.Logger.Debug("Completed {Id} creation with {MediaCount} media", collection.Id, collection.Media.Count);

            return collection;
        }

        public void UpdateDocument(Collection newDocument, Collection existingDocument, IDocumentSession documentSession)
        {
            // Map over existing document
            Mapper.Map(newDocument, existingDocument);
        }
    }
}