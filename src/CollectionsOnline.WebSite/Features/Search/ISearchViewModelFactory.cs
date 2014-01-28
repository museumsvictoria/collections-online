using System.Collections.Generic;
using CollectionsOnline.Core.Indexes;
using Nancy;
using Raven.Abstractions.Data;

namespace CollectionsOnline.WebSite.Features.Search
{
    public interface ISearchViewModelFactory
    {
        SearchViewModel MakeViewModel(IList<CombinedSearchResult> results, FacetResults facets, List<string> suggestions, Request request, int totalResults, SearchInputModel searchInputModel, long queryTimeElapsed, long facetTimeElapsed);
    }
}