using System.Collections.Generic;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.WebSite.Models;
using Raven.Abstractions.Data;

namespace CollectionsOnline.WebSite.Factories
{
    public interface ISearchViewModelFactory
    {
        SearchViewModel MakeViewModel(
            IList<EmuAggregateRootViewModel> results,
            FacetResults facets, 
            List<string> suggestions, 
            int totalResults, 
            SearchInputModel searchInputModel, 
            long queryTimeElapsed, 
            long facetTimeElapsed);
    }
}