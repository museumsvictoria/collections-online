using System;
using System.Linq;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.WebSite.Models;
using Raven.Client;

namespace CollectionsOnline.WebSite.Queries
{
    public class HomeViewModelQuery : IHomeViewModelQuery
    {
        private readonly IDocumentSession _documentSession;

        public HomeViewModelQuery(
            IDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }

        public HomeIndexViewModel BuildHomeIndex()
        {
            using (_documentSession.Advanced.DocumentStore.AggressivelyCacheFor(Constants.AggressiveCacheTimeSpan))
            {
                var homeViewModel = new HomeIndexViewModel();

                var facetResult = _documentSession.Advanced
                    .DocumentQuery<CombinedIndexResult, CombinedIndex>()
                    .ToFacets("facets/combinedFacets");

                var typeFacet = facetResult.Results["Type"];

                if (typeFacet != null)
                {
                    var articleFacetItem = typeFacet.Values.FirstOrDefault(x => x.Range == "article");
                    if (articleFacetItem != null)
                        homeViewModel.ArticleCount = articleFacetItem.Hits;

                    var itemFacetItem = typeFacet.Values.FirstOrDefault(x => x.Range == "item");
                    if (itemFacetItem != null)
                        homeViewModel.ItemCount = itemFacetItem.Hits;

                    var speciesFacetItem = typeFacet.Values.FirstOrDefault(x => x.Range == "species");
                    if (speciesFacetItem != null)
                        homeViewModel.SpeciesCount = speciesFacetItem.Hits;

                    var specimenFacetItem = typeFacet.Values.FirstOrDefault(x => x.Range == "specimen");
                    if (specimenFacetItem != null)
                        homeViewModel.SpecimenCount = specimenFacetItem.Hits;
                }

                homeViewModel.RecentResults = _documentSession.Advanced
                    .DocumentQuery<CombinedIndexResult, CombinedIndex>()
                    .OrderByDescending(x => x.DateModified)
                    .OrderByDescending(x => x.Quality)
                    .WhereEquals("HasImages", "Yes")
                    .Take(Constants.HomeMaxRecentResults)
                    .SelectFields<EmuAggregateRootViewModel>()
                    .ToList();

                return homeViewModel;
            }
        }
    }
}