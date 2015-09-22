using System.Collections.Generic;
using Raven.Client;

namespace CollectionsOnline.WebSite.Models
{
    public class SearchApiViewModel
    {
        public IList<dynamic> Results { get; set; }

        public RavenQueryStatistics Statistics;

        public SearchApiViewModel()
        {
            Results = new List<dynamic>();
        }
    }
}