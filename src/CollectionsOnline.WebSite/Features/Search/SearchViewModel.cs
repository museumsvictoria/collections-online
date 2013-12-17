using System.Collections.Generic;
using CollectionsOnline.Core.Indexes;

namespace CollectionsOnline.WebSite.Features.Search
{
    public class SearchViewModel
    {
        public int Offset { get; set; }

        public int Limit { get; set; }

        public int TotalResults { get; set; }

        public string Query { get; set; }

        public IList<FacetViewModel> Facets { get; set; }

        public IList<SearchResultViewModel> Results { get; set; }

        public SearchViewModel()
        {
            Facets = new List<FacetViewModel>();
            Results = new List<SearchResultViewModel>();
        }
    }

    public class FacetViewModel
    {
        public string Name { get; set; }

        public IList<KeyValuePair<string, string>> Values { get; set; }

        public FacetViewModel()
        {
            Values = new List<KeyValuePair<string, string>>();
        }
    }

    public class SearchResultViewModel
    {
        public CombinedSearchResult Result { get; set; }

        public IList<KeyValuePair<string, string>> Terms { get; set; }

        public SearchResultViewModel()
        {
            Terms = new List<KeyValuePair<string, string>>();
        }
    }
}