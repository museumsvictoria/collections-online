using System.Collections.Generic;
using System.Linq;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Models;
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
                            relatedItem.ThumbnailUri,
                            relatedItem.DisplayTitle,
                            SubDisplayTitle = relatedItem.RegistrationNumber
                        },
                    RelatedSpecimens = from specimenId in species.RelatedSpecimenIds
                        let relatedSpecimen = LoadDocument<Specimen>(specimenId)
                        where relatedSpecimen != null && !relatedSpecimen.IsHidden
                        select new
                        {
                            relatedSpecimen.Id,
                            relatedSpecimen.ThumbnailUri,
                            relatedSpecimen.DisplayTitle,
                            SubDisplayTitle = relatedSpecimen.RegistrationNumber
                        }
                };
        }
    }
}