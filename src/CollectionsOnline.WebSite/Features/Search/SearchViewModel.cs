using System.Collections.Generic;
using System.Linq;
using CollectionsOnline.Core.Indexes;
using Raven.Client.Linq.Indexing;

namespace CollectionsOnline.WebSite.Features.Search
{
    public class SearchViewModel
    {
        public int Offset { get; set; }

        public int Limit { get; set; }

        public int TotalResults { get; set; }

        public long QueryTimeElapsed { get; set; }

        public long FacetTimeElapsed { get; set; }

        public string Query { get; set; }

        public string NextPageUrl { get; set; }

        public string PrevPageUrl { get; set; }        

        public IList<FacetViewModel> Facets { get; set; }

        public long TotalFacetValues
        {
            get
            {
                return Facets.SelectMany(x => x.Values).Count();
            }
        }

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