using System.Linq;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Models;
using Raven.Client;
using Raven.Client.Linq;

namespace CollectionsOnline.WebSite.Features.Specimens
{
    public class SpecimenViewModelQuery : ISpecimenViewModelQuery
    {
        private readonly IDocumentSession _documentSession;

        public SpecimenViewModelQuery(
            IDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }

        public SpecimenViewTransformerResult BuildSpecimen(string specimenId)
        {
            var result = _documentSession.Load<SpecimenViewTransformer, SpecimenViewTransformerResult>(specimenId);

            var query = _documentSession.Advanced
                .LuceneQuery<CombinedResult, Combined>()
                .WhereEquals("TaxonomyIrn", result.Specimen.Taxonomy.Irn);

            result.RelatedSpeciesSpecimenItemCount = query.QueryResult.TotalResults;

            return result;
        }
    }
}