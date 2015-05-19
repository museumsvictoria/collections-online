using System.Collections.Generic;
using CollectionsOnline.WebSite.Models.Api;

namespace CollectionsOnline.WebSite.Models
{
    public class DevelopersIndexViewModel
    {
        public IEnumerable<ApiOperationMetadata> OperationMetadata { get; set; }

        public string ApiRootUrl { get; set; }

        public string ApiCurrentVersionRootUrl { get; set; }

        public string PagingPageSizeMax { get; set; }
    }
}