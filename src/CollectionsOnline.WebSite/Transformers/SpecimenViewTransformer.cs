using System.Linq;
using CollectionsOnline.Core.Models;
using Raven.Client.Indexes;

namespace CollectionsOnline.WebSite.Transformers
{
    public class SpecimenViewTransformer : AbstractTransformerCreationTask<Specimen>
    {
        public SpecimenViewTransformer()
        {
            TransformResults = specimens => from specimen in specimens
                select new
                {
                    Specimen = specimen,
                    RelatedItems = from itemId in specimen.RelatedItemIds
                        let relatedItem = LoadDocument<Item>(itemId)
                        where relatedItem != null && !relatedItem.IsHidden
                        select new
                        {
                            relatedItem.Id, 
                            relatedItem.ThumbnailUri,
                            relatedItem.DisplayTitle,
                            SubDisplayTitle = relatedItem.RegistrationNumber
                        },
                    RelatedSpecimens = from specimenId in specimen.RelatedSpecimenIds
                        let relatedSpecimen = LoadDocument<Specimen>(specimenId)
                        where relatedSpecimen != null && !relatedSpecimen.IsHidden
                        select new
                        {
                            relatedSpecimen.Id,
                            relatedSpecimen.ThumbnailUri,
                            relatedSpecimen.DisplayTitle,
                            SubDisplayTitle = relatedSpecimen.RegistrationNumber
                        },
                    RelatedSpecies = from speciesId in specimen.RelatedSpeciesIds
                        let relatedSpecies = LoadDocument<Species>(speciesId)
                        where relatedSpecies != null && !relatedSpecies.IsHidden
                        select new
                        {
                            relatedSpecies.Id,
                            relatedSpecies.ThumbnailUri,
                            relatedSpecies.DisplayTitle
                        },
                    RelatedArticles = from articleId in specimen.RelatedArticleIds
                        let relatedArticle = LoadDocument<Article>(articleId)
                        where relatedArticle != null && !relatedArticle.IsHidden
                        select new
                        {
                            relatedArticle.Id,
                            relatedArticle.ThumbnailUri,
                            relatedArticle.DisplayTitle
                        }
                };
        }
    }
}