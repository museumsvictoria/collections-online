using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CollectionsOnline.Core.Indexes;
using Nancy;
using Raven.Abstractions.Data;
using Raven.Client;

namespace CollectionsOnline.WebSite.Features.Search
{
    public interface ISearchViewModelFactory
    {
        SearchViewModel MakeViewModel(IList<CombinedSearchResult> results, FacetResults facets, Request request, int totalResults, SearchInputModel searchInputModel, long elapsedMilliseconds);
    }
}