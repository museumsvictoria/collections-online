using System.Collections.Generic;
using System.Linq;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Factories;
using CollectionsOnline.WebSite.Models;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Linq;
using Constants = CollectionsOnline.Core.Config.Constants;

namespace CollectionsOnline.WebSite.Queries
{
    public class HomeViewModelQuery : IHomeViewModelQuery
    {
        private readonly IDocumentSession _documentSession;
        private readonly IHomeHeroUriFactory _homeHeroUriFactory;

        public HomeViewModelQuery(
            IDocumentSession documentSession,
            IHomeHeroUriFactory homeHeroUriFactory)
        {
            _documentSession = documentSession;
            _homeHeroUriFactory = homeHeroUriFactory;
        }

        public HomeIndexViewModel BuildHomeIndex()
        {
            using (_documentSession.Advanced.DocumentStore.AggressivelyCacheFor(Constants.AggressiveCacheTimeSpan))            
            {
                var homeViewModel = new HomeIndexViewModel();

                var facetResult = _documentSession.Advanced
                    .DocumentQuery<CombinedIndexResult, CombinedIndex>()
                    .ToFacets("facets/combinedFacets");

                var recordTypeFacet = facetResult.Results["RecordType"];

                if (recordTypeFacet != null)
                {
                    var articleFacetItem = recordTypeFacet.Values.FirstOrDefault(x => x.Range == "article");
                    if (articleFacetItem != null)
                        homeViewModel.ArticleCount = articleFacetItem.Hits;

                    var itemFacetItem = recordTypeFacet.Values.FirstOrDefault(x => x.Range == "item");
                    if (itemFacetItem != null)
                        homeViewModel.ItemCount = itemFacetItem.Hits;

                    var speciesFacetItem = recordTypeFacet.Values.FirstOrDefault(x => x.Range == "species");
                    if (speciesFacetItem != null)
                        homeViewModel.SpeciesCount = speciesFacetItem.Hits;

                    var specimenFacetItem = recordTypeFacet.Values.FirstOrDefault(x => x.Range == "specimen");
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

                homeViewModel.HomeHeroUri = _homeHeroUriFactory.GetCurrentUri();

                var feature = _documentSession.Load<Feature>("features/home");
                if (feature != null)
                {
                    homeViewModel.Feature = feature;
                    
                    var featuredRecords = _documentSession.Advanced
                        .DocumentQuery<CombinedIndexResult, CombinedIndex>()
                        .WhereIn("Id", feature.FeaturedIds)
                        .SelectFields<EmuAggregateRootViewModel>()
                        .ToList();
                    
                    homeViewModel.FeaturedRecords = featuredRecords.OrderBy(x => feature.FeaturedIds.IndexOf(x.Id)).ToList();
                }

                return homeViewModel;
            }
        }
    }
}