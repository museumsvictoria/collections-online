using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using Nancy;

namespace CollectionsOnline.WebSite.Features.Search
{
    public interface ISearchViewModelQuery
    {
        SearchViewModel BuildSearch(SearchInputModel searchInputModel, Request request);
    }
}