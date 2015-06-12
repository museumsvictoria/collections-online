using System.Linq;
using CollectionsOnline.Core.Models;
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
                            relatedItem.DisplayTitle,
                            SubDisplayTitle = relatedItem.RegistrationNumber,
                            relatedItem.Summary,
                            relatedItem.ThumbnailUri,
                            RecordType = "Item"
                        },
                    RelatedSpecimens = from specimenId in item.RelatedSpecimenIds
                        let relatedSpecimen = LoadDocument<Specimen>(specimenId)
                        where relatedSpecimen != null && !relatedSpecimen.IsHidden
                        select new
                        {
                            relatedSpecimen.Id,                            
                            relatedSpecimen.DisplayTitle,
                            SubDisplayTitle = relatedSpecimen.RegistrationNumber,
                            relatedSpecimen.Summary,
                            relatedSpecimen.ThumbnailUri,
                            RecordType = "Specimen"
                        },
                    RelatedArticles = from articleId in item.RelatedArticleIds
                        let relatedArticle = LoadDocument<Article>(articleId)
                        where relatedArticle != null && !relatedArticle.IsHidden
                        select new
                        {
                            relatedArticle.Id,                            
                            relatedArticle.DisplayTitle,
                            relatedArticle.Summary,
                            relatedArticle.ThumbnailUri,
                            RecordType = "Article"
                        },
                    RelatedSpecies = from speciesId in item.RelatedSpeciesIds
                        let relatedSpecies = LoadDocument<Species>(speciesId)
                        where relatedSpecies != null && !relatedSpecies.IsHidden
                        select new
                        {
                            relatedSpecies.Id,
                            relatedSpecies.DisplayTitle,
                            relatedSpecies.Summary,
                            relatedSpecies.ThumbnailUri,
                            RecordType = "Species"
                        }
                };
        }
    }
}