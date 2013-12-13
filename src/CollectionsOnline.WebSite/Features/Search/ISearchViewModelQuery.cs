using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;

namespace CollectionsOnline.WebSite.Features.Search
{
    public interface ISearchViewModelQuery
    {
        ExpandoObject BuildSearch(SearchInputModel searchInputModel);
    }
}