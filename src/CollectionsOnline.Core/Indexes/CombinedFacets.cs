using System.Collections.Generic;
using Raven.Abstractions.Data;

namespace CollectionsOnline.Core.Indexes
{
    public class CombinedFacets : FacetSetup
    {
        public CombinedFacets()
        {
            Id = "facets/combinedFacets";

            Facets = new List<Facet>
            {
                new Facet<CombinedIndexResult>
                {
                    Name = x => x.HasImages
                },
                new Facet<CombinedIndexResult>
                {
                    Name = x => x.OnDisplay
                },
                new Facet<CombinedIndexResult>
                {
                    Name = x => x.Type
                },
                new Facet<CombinedIndexResult>
                {
                    Name = x => x.Category
                },
                new Facet<CombinedIndexResult>
                {
                    Name = x => x.CollectionArea
                },
                new Facet<CombinedIndexResult>
                {
                    Name = x => x.ItemType
                },
                new Facet<CombinedIndexResult>
                {
                    Name = x => x.SpeciesType
                },
                new Facet<CombinedIndexResult>
                {
                    Name = x => x.SpeciesEndemicity
                },
                new Facet<CombinedIndexResult>
                {
                    Name = x => x.SpecimenScientificGroup
                },
                new Facet<CombinedIndexResult>
                {
                    Name = x => x.ArticleType
                },
                new Facet<CombinedIndexResult>
                {
                    Name = x => x.OnDisplayLocation
                }
            };
        }
    }
}