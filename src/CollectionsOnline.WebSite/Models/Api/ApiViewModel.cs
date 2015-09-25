using System.Collections.Generic;

namespace CollectionsOnline.WebSite.Models.Api
{
    public class ApiViewModel
    {
        public IList<dynamic> Results { get; set; }

        public ApiPageInfo ApiPageInfo;

        public ApiViewModel()
        {
            Results = new List<dynamic>();
        }
    }
}