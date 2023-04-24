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
                    Name = x => x.HasMedia
                },
                new Facet<CombinedIndexResult>
                {
                    Name = x => x.DisplayStatus
                },
                new Facet<CombinedIndexResult>
                {
                    Name = x => x.DisplayLocation
                },
                new Facet<CombinedIndexResult>
                {
                    Name = x => x.RecordType
                },
                new Facet<CombinedIndexResult>
                {
                    Name = x => x.Category
                },
                new Facet<CombinedIndexResult>
                {
                    Name = x => x.CollectingArea
                },
                new Facet<CombinedIndexResult>
                {
                    Name = x => x.SpecimenScientificGroup
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
                    Name = x => x.ArticleType
                },
                new Facet<CombinedIndexResult>
                {
                    Name = x => x.ImageLicence
                },
                // Deprecated Facets
                new Facet<CombinedIndexResult>
                {
                    Name = x => x.HasImages
                },
                new Facet<CombinedIndexResult>
                {
                    Name = x => x.OnDisplay
                },
            };
        }
    }
}