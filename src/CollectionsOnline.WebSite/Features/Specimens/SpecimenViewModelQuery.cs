using System.Linq;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Models;
using Raven.Client;

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

            if (result.Specimen.Taxonomy != null)
            {
                var query = _documentSession.Advanced
                    .DocumentQuery<CombinedResult, Combined>()
                    .WhereEquals("Taxon", result.Specimen.Taxonomy.TaxonName);

                // Dont allow a link to search page if the current specimen is the only result
                if (query.SelectFields<CombinedResult>("Id").Select(x => x.Id).Except(new[] { specimenId }).Any())
                {
                    result.RelatedSpeciesSpecimenItemCount = query.QueryResult.TotalResults;
                }
            }

            // Set Media
            result.SpecimenHeroImage = result.Specimen.Media.FirstOrDefault(x => x is ImageMedia) as ImageMedia;
            result.SpecimenImages = result.Specimen.Media.Where(x => x is ImageMedia).Cast<ImageMedia>().ToList();

            return result;
        }
    }
}