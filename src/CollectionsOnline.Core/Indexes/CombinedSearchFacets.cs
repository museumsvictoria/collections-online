using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Abstractions.Data;

namespace CollectionsOnline.Core.Indexes
{
    public class CombinedSearchFacets : FacetSetup
    {
        public CombinedSearchFacets()
        {
            Id = "facets/combinedFacets";

            Facets = new List<Facet>
            {
                new Facet<CombinedSearchResult>
                {
                    Name = x => x.Type
                },
                new Facet<CombinedSearchResult>
                {
                    Name = x => x.Category
                },
                new Facet<CombinedSearchResult>
                {
                    Name = x => x.ItemType
                },
                new Facet<CombinedSearchResult>
                {
                    Name = x => x.SpeciesType
                },
                new Facet<CombinedSearchResult>
                {
                    Name = x => x.SpeciesSubType
                },
                new Facet<CombinedSearchResult>
                {
                    Name = x => x.SpeciesHabitats
                },
                new Facet<CombinedSearchResult>
                {
                    Name = x => x.SpeciesDepths
                },
                new Facet<CombinedSearchResult>
                {
                    Name = x => x.SpeciesWaterColumnLocations
                },
                new Facet<CombinedSearchResult>
                {
                    Name = x => x.SpeciesPhylum
                },
                new Facet<CombinedSearchResult>
                {
                    Name = x => x.SpeciesClass
                },
                new Facet<CombinedSearchResult>
                {
                    Name = x => x.SpeciesOrder
                },
                new Facet<CombinedSearchResult>
                {
                    Name = x => x.SpeciesFamily
                },
                new Facet<CombinedSearchResult>
                {
                    Name = x => x.SpecimenScientificGroup
                },
                new Facet<CombinedSearchResult>
                {
                    Name = x => x.SpecimenDiscipline
                },
                new Facet<CombinedSearchResult>
                {
                    Name = x => x.StoryTypes
                }
            };
        }
    }
}
