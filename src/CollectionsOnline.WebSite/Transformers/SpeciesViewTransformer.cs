using System.Linq;
using CollectionsOnline.Core.Models;
using Raven.Client.Indexes;

namespace CollectionsOnline.WebSite.Transformers
{
    public class SpeciesViewTransformer : AbstractTransformerCreationTask<Species>
    {
        public SpeciesViewTransformer()
        {
            TransformResults = speciesDocs => from species in speciesDocs
                select new
                {
                    Species = species,
                    RelatedItems = species.RelatedItemIds
                        .Select(LoadDocument<Item>)
                        .Where(item => item != null && !item.IsHidden)
                        .Select(item => new
                        {
                            item.Id,
                            item.DisplayTitle,
                            SubDisplayTitle = item.RegistrationNumber,
                            item.Summary,
                            item.ThumbnailUri,
                            RecordType = "Item"
                        }),
                    RelatedSpecimens = species.RelatedSpecimenIds
                        .Select(LoadDocument<Specimen>)
                        .Where(specimen => specimen != null && !specimen.IsHidden)
                        .Select(specimen => new
                        {
                            specimen.Id,
                            specimen.DisplayTitle,
                            SubDisplayTitle = specimen.RegistrationNumber,
                            specimen.Summary,
                            specimen.ThumbnailUri,
                            RecordType = "Specimen"
                        }),
                    RelatedArticles = species.RelatedArticleIds
                        .Select(LoadDocument<Article>)
                        .Where(article => article != null && !article.IsHidden)
                        .Select(article => new
                        {
                            article.Id,
                            article.DisplayTitle,
                            article.Summary,
                            article.ThumbnailUri,
                            RecordType = "Article"
                        }),
                    RelatedSpecies = species.RelatedSpeciesIds
                        .Select(LoadDocument<Species>)
                        .Where(relatedSpecies => relatedSpecies != null && !relatedSpecies.IsHidden)
                        .Select(relatedSpecies => new
                        {
                            relatedSpecies.Id,
                            relatedSpecies.DisplayTitle,
                            relatedSpecies.Summary,
                            relatedSpecies.ThumbnailUri,
                            RecordType = "Species"
                        })
                };
        }
    }
}