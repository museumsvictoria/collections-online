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
                    RelatedItems = from itemId in species.RelatedItemIds
                        let relatedItem = LoadDocument<Item>(itemId)
                        where relatedItem != null && !relatedItem.IsHidden
                        select new
                        {
                            relatedItem.Id,                             
                            relatedItem.DisplayTitle,
                            SubDisplayTitle = relatedItem.RegistrationNumber,
                            relatedItem.Summary,
                            relatedItem.ThumbnailUri,
                            Type = "Item"
                        },
                    RelatedSpecimens = from specimenId in species.RelatedSpecimenIds
                        let relatedSpecimen = LoadDocument<Specimen>(specimenId)
                        where relatedSpecimen != null && !relatedSpecimen.IsHidden
                        select new
                        {
                            relatedSpecimen.Id,
                            relatedSpecimen.DisplayTitle,
                            SubDisplayTitle = relatedSpecimen.RegistrationNumber,
                            relatedSpecimen.Summary,
                            relatedSpecimen.ThumbnailUri,
                            Type = "Specimen"
                        }
                };
        }
    }
}