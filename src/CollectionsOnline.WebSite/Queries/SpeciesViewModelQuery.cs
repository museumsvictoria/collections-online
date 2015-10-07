using System;
using System.Linq;
using AutoMapper;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.WebSite.Models.Api;
using CollectionsOnline.WebSite.Transformers;
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
            using (_documentSession.Advanced.DocumentStore.AggressivelyCacheFor(Constants.AggressiveCacheTimeSpan))
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

                // Add UriMedia link to ALA (except for fossils)
                if (result.Species.Taxonomy != null &&
                    !(result.Species.AnimalType.Contains("fossil", StringComparison.OrdinalIgnoreCase) || result.Species.AnimalSubType.Contains("fossil", StringComparison.OrdinalIgnoreCase)))
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

        public ApiViewModel BuildSpeciesApiIndex(ApiInputModel apiInputModel)
        {
            using (_documentSession.Advanced.DocumentStore.AggressivelyCacheFor(Constants.AggressiveCacheTimeSpan))
            {
                RavenQueryStatistics statistics;
                var results = _documentSession.Advanced
                    .DocumentQuery<dynamic, CombinedIndex>()
                    .WhereEquals("RecordType", "Species")
                    .Statistics(out statistics)
                    .Skip((apiInputModel.Page - 1) * apiInputModel.PerPage)
                    .Take(apiInputModel.PerPage);

                return new ApiViewModel
                {
                    Results = results.Cast<Species>().Select<Species, dynamic>(Mapper.Map<Species, SpeciesApiViewModel>).ToList(),
                    ApiPageInfo = new ApiPageInfo(statistics.TotalResults, apiInputModel.PerPage)
                };
            }

        }
    }
}