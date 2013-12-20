using System.Collections.Generic;
using CollectionsOnline.Core.Indexes;

namespace CollectionsOnline.WebSite.Features.Search
{
    public class SearchViewModel
    {
        public int Offset { get; set; }

        public int Limit { get; set; }

        public int TotalResults { get; set; }

        public long ElapsedMilliseconds { get; set; }

        public string Query { get; set; }

        public IList<FacetViewModel> Facets { get; set; }

        public IList<SearchResultViewModel> Results { get; set; }

        public IList<FacetValueViewModel> ActiveFacets { get; set; }

        public IList<TermViewModel> ActiveTerms { get; set; }

        public SearchViewModel()
        {
            Facets = new List<FacetViewModel>();
            Results = new List<SearchResultViewModel>();
            ActiveFacets = new List<FacetValueViewModel>();
            ActiveTerms = new List<TermViewModel>();
        }
    }

    public class FacetViewModel
    {
        public string Name { get; set; }

        public IList<FacetValueViewModel> Values { get; set; }

        public FacetViewModel()
        {
            Values = new List<FacetValueViewModel>();
        }
    }

    public class FacetValueViewModel
    {
        public string Facet { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public bool Active { get; set; }

        public int Hits { get; set; }
    }

    public class TermViewModel
    {
        public string Term { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }
    }

    public class SearchResultViewModel
    {
        public CombinedSearchResult Result { get; set; }

        public IList<TermViewModel> Terms { get; set; }

        public SearchResultViewModel()
        {
            Terms = new List<TermViewModel>();
        }
    }
}