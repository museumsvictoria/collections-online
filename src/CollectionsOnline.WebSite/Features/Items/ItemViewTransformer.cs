using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Features.Shared;
using Raven.Client.Indexes;

namespace CollectionsOnline.WebSite.Features.Items
{
    public class ItemViewTransformer : AbstractTransformerCreationTask<Item>
    {
        public ItemViewTransformer()
        {
            TransformResults = items => from item in items
                select new
                {
                    Item = item,
                    RelatedItems = from itemId in item.RelatedItemIds
                        let relatedItem = LoadDocument<Item>(itemId)
                        where relatedItem != null && !relatedItem.IsHidden
                        select new
                        {
                            relatedItem.Id, 
                            relatedItem.ThumbnailUri,
                            relatedItem.DisplayTitle
                        },
                    RelatedSpecimens = from specimenId in item.RelatedSpecimenIds
                        let relatedSpecimen = LoadDocument<Specimen>(specimenId)
                        where relatedSpecimen != null && !relatedSpecimen.IsHidden
                        select new
                        {
                            relatedSpecimen.Id,
                            relatedSpecimen.ThumbnailUri,
                            relatedSpecimen.DisplayTitle
                        },
                    RelatedArticles = from articleId in item.RelatedArticleIds
                        let relatedArticle = LoadDocument<Article>(articleId)
                        where relatedArticle != null && !relatedArticle.IsHidden
                        select new
                        {
                            relatedArticle.Id,
                            relatedArticle.ThumbnailUri,
                            relatedArticle.DisplayTitle
                        },
                    RelatedSpecies = from speciesId in item.RelatedSpeciesIds
                        let relatedSpecies = LoadDocument<Core.Models.Species>(speciesId)
                        where relatedSpecies != null && !relatedSpecies.IsHidden
                        select new
                        {
                            relatedSpecies.Id,
                            relatedSpecies.ThumbnailUri,
                            relatedSpecies.DisplayTitle
                        }
                };
        }
    }

    public class ItemViewTransformerResult
    {
        public Item Item { get; set; }

        public IList<RelatedDocumentViewModel> RelatedItems { get; set; }

        public IList<RelatedDocumentViewModel> RelatedSpecimens { get; set; }

        public IList<RelatedDocumentViewModel> RelatedArticles { get; set; }

        public IList<RelatedDocumentViewModel> RelatedSpecies { get; set; }

        public int RelatedSpeciesSpecimenItemCount { get; set; }

        public ImageMedia ItemHeroImage { get; set; }

        public IList<ImageMedia> ItemImages { get; set; }

        public ItemViewTransformerResult()
        {
            RelatedItems = new List<RelatedDocumentViewModel>();
            RelatedSpecimens = new List<RelatedDocumentViewModel>();
            RelatedArticles = new List<RelatedDocumentViewModel>();
            RelatedSpecies = new List<RelatedDocumentViewModel>();
            ItemImages = new List<ImageMedia>();
        }
    }
}