using System.Collections.Generic;
using System.Linq;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Features.Shared;
using Raven.Client.Indexes;

namespace CollectionsOnline.WebSite.Features.Species
{
    public class SpeciesViewTransformer : AbstractTransformerCreationTask<Core.Models.Species>
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
                            Title = relatedItem.ObjectName
                        },
                    RelatedSpecimens = from specimenId in species.RelatedSpecimenIds
                        let relatedSpecimen = LoadDocument<Specimen>(specimenId)
                        where relatedSpecimen != null && !relatedSpecimen.IsHidden
                        select new
                        {
                            relatedSpecimen.Id,
                            relatedSpecimen.ThumbnailUri,
                            Title = relatedSpecimen.ObjectName ?? relatedSpecimen.ScientificName
                        }
                };
        }
    }

    public class SpeciesViewTransformerResult
    {
        public Core.Models.Species Species { get; set; }

        public IList<RelatedDocumentViewModel> RelatedItems { get; set; }

        public IList<RelatedDocumentViewModel> RelatedSpecimens { get; set; }

        public int RelatedSpecimenCount { get; set; }
        
        public ImageMedia SpeciesHeroImage { get; set; }
        
        public IList<ImageMedia> SpeciesImages { get; set; }

        public SpeciesViewTransformerResult()
        {
            RelatedItems = new List<RelatedDocumentViewModel>();
            RelatedSpecimens = new List<RelatedDocumentViewModel>();
            SpeciesImages = new List<ImageMedia>();
        }
    }
}