using System.Collections.Generic;
using System.Linq;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Models;
using Raven.Client.Indexes;

namespace CollectionsOnline.WebSite.Transformers
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
                            relatedItem.DisplayTitle,
                            SubDisplayTitle = relatedItem.RegistrationNumber
                        },
                    RelatedSpecimens = from specimenId in item.RelatedSpecimenIds
                        let relatedSpecimen = LoadDocument<Specimen>(specimenId)
                        where relatedSpecimen != null && !relatedSpecimen.IsHidden
                        select new
                        {
                            relatedSpecimen.Id,
                            relatedSpecimen.ThumbnailUri,
                            relatedSpecimen.DisplayTitle,
                            SubDisplayTitle = relatedSpecimen.RegistrationNumber
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
                        let relatedSpecies = LoadDocument<Species>(speciesId)
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
}