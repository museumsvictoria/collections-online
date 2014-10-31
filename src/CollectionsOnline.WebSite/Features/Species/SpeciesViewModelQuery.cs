using System.Linq;
using CollectionsOnline.Core.Indexes;
using Raven.Client;

namespace CollectionsOnline.WebSite.Features.Species
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

            if (result.Species.Taxonomy != null)
            {
                var query = _documentSession.Advanced
                    .LuceneQuery<CombinedResult, Combined>()
                    .WhereEquals("Taxon", result.Species.Taxonomy.TaxonName);

                // Dont allow a link to search page if the current species is the only result
                if (query.SelectFields<CombinedResult>("Id").Select(x => x.Id).Except(new[] {speciesId}).Any())
                {
                    result.RelatedSpecimenCount = query.QueryResult.TotalResults;
                }
            }

            return result;
        }
    }
}