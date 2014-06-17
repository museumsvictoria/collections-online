using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                    Name = x => x.Type
                },
                new Facet<CombinedResult>
                {
                    Name = x => x.Category
                },
                new Facet<CombinedResult>
                {
                    Name = x => x.HasImages
                },
                new Facet<CombinedResult>
                {
                    Name = x => x.Discipline
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
                    Name = x => x.SpeciesSubType
                },
                new Facet<CombinedResult>
                {
                    Name = x => x.SpeciesHabitats
                },
                new Facet<CombinedResult>
                {
                    Name = x => x.SpeciesDepths
                },
                new Facet<CombinedResult>
                {
                    Name = x => x.SpeciesWaterColumnLocations
                },
                new Facet<CombinedResult>
                {
                    Name = x => x.Phylum
                },
                new Facet<CombinedResult>
                {
                    Name = x => x.Class
                },
                new Facet<CombinedResult>
                {
                    Name = x => x.SpecimenScientificGroup
                },
                new Facet<CombinedResult>
                {
                    Name = x => x.SpecimenDiscipline
                },
                new Facet<CombinedResult>
                {
                    Name = x => x.StoryTypes
                },
                new Facet<CombinedResult>
                {
                    Name = x => x.Dates
                }
            };
        }
    }
}
