using System.Collections.Generic;
using Nancy.Cookies;

namespace CollectionsOnline.WebSite.Models
{
    public class SearchInputModel
    {
        public int Page { get; set; }

        public int PerPage { get; set; }

        public string Sort { get; set; }

        public string View { get; set; }

        public IList<string> Queries { get; set; }

        public string CurrentUrl { get; set; }

        public string CurrentQueryString { get; set; }

        public IDictionary<string, string> Facets { get; set; }

        public IDictionary<string, string[]> MultiFacets { get; set; }

        public IDictionary<string, string> Terms { get; set; }

        public IList<NancyCookie> Cookies { get; set; }

        public SearchInputModel()
        {
            Queries = new List<string>();
            Facets = new Dictionary<string, string>();
            MultiFacets = new Dictionary<string, string[]>();
            Terms = new Dictionary<string, string>();
            Cookies = new List<NancyCookie>();
        }
    }
}