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
                new Facet<CombinedResult>
                {
                    Name = x => x.HasImages
                },
                new Facet<CombinedResult>
                {
                    Name = x => x.OnDisplay
                },
                new Facet<CombinedResult>
                {
                    Name = x => x.Type
                },
                new Facet<CombinedResult>
                {
                    Name = x => x.Category
                },
                new Facet<CombinedResult>
                {
                    Name = x => x.CollectionAreas
                },
                new Facet<CombinedResult>
                {
                    Name = x => x.ItemType
                },
                new Facet<CombinedResult>
                {
                    Name = x => x.SpeciesType
                },
                new Facet<CombinedResult>
                {
                    Name = x => x.SpeciesEndemicity
                },
                new Facet<CombinedResult>
                {
                    Name = x => x.SpecimenScientificGroup
                },
                new Facet<CombinedResult>
                {
                    Name = x => x.ArticleTypes
                },
                new Facet<CombinedResult>
                {
                    Name = x => x.OnDisplayLocation
                }
            };
        }
    }
}