using System.Linq;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Extensions;
using CollectionsOnline.WebSite.Transformers;
using Newtonsoft.Json;
using Raven.Client;

namespace CollectionsOnline.WebSite.Queries
{
    public class SpeciesViewModelQuery : ISpeciesViewModelQuery
    {
        private readonly IDocumentSession _documentSession;

        public SpeciesViewModelQuery(
            IDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }

        public SpeciesViewTransformerResult BuildSpecies(string speciesId)
        {
            var result = _documentSession.Load<SpeciesViewTransformer, SpeciesViewTransformerResult>(speciesId);

            // Check to see whether there are related specimens
            if (result.Species.Taxonomy != null)
            {
                var query = _documentSession.Advanced
                    .DocumentQuery<CombinedIndexResult, CombinedIndex>()
                    .WhereEquals("Taxon", result.Species.Taxonomy.TaxonName)
                    .Take(1);

                // Dont allow a link to search page if the current species is the only result
                if (query
                    .SelectFields<CombinedIndexResult>("Id")
                    .Select(x => x.Id)
                    .Except(new[] {speciesId})
                    .Any())
                {
                    result.RelatedSpecimenCount = query.QueryResult.TotalResults;
                }
            }

            // Uris
            if (result.Species.Taxonomy != null)
            {
                result.Species.Media.Add(new UriMedia
                {
                    Caption =
                        string.Format("See {0} in the Atlas of Living Australia", result.Species.Taxonomy.TaxonName),
                    Uri =
                        string.Format("http://bie.ala.org.au/search?q={0}&fq=idxtype:TAXON",
                            result.Species.Taxonomy.TaxonName)
                });
            }

            return result;
        }
    }
}