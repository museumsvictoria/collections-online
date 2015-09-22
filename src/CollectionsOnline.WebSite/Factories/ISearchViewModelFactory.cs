using System.Collections.Generic;
using CollectionsOnline.WebSite.Models;
using CollectionsOnline.WebSite.Models.Api;
using Raven.Abstractions.Data;

namespace CollectionsOnline.WebSite.Factories
{
    public interface ISearchViewModelFactory
    {
        SearchIndexViewModel MakeSearchIndex(
            IList<EmuAggregateRootViewModel> results,
            FacetResults facets, 
            int totalResults,
            SearchInputModel searchInputModel, 
            long queryTimeElapsed, 
            long facetTimeElapsed);
    }
}