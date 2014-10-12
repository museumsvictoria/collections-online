using System.Linq;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Models;
using Raven.Client;
using Raven.Client.Linq;

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

            var query = _documentSession.Advanced
                .LuceneQuery<CombinedResult, Combined>()
                .WhereEquals("Species", speciesId);

            result.RelatedSpecimenCount = query.QueryResult.TotalResults;

            return result;
        }
    }
}