using System.Linq;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Models;
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

            if (result.Species.Taxonomy != null)
            {
                var query = _documentSession.Advanced
                    .DocumentQuery<CombinedIndexResult, CombinedIndex>()
                    .WhereEquals("Taxon", result.Species.Taxonomy.TaxonName)
                    .Take(1);

                // Dont allow a link to search page if the current species is the only result
                if (query.SelectFields<CombinedIndexResult>("Id").Select(x => x.Id).Except(new[] {speciesId}).Any())
                {
                    result.RelatedSpecimenCount = query.QueryResult.TotalResults;
                }
            }

            // Set Media            
            result.SpeciesImages = result.Species.Media.Where(x => x is ImageMedia).Cast<ImageMedia>().ToList();
            result.SpeciesFiles = result.Species.Media.Where(x => x is FileMedia).Cast<FileMedia>().ToList();
            result.SpeciesVideos = result.Species.Media.Where(x => x is VideoMedia).Cast<VideoMedia>().ToList();
            result.JsonSpeciesImages = JsonConvert.SerializeObject(result.SpeciesImages);

            return result;
        }
    }
}