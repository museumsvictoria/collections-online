using System.Collections.Generic;
using CollectionsOnline.WebSite.Metadata;

namespace CollectionsOnline.WebSite.Models
{
    public class DevelopersViewModel
    {
        public IEnumerable<ApiOperationMetadata> OperationMetadata { get; set; }

        public string ApiRootUrl { get; set; }

        public string ApiCurrentVersionRootUrl { get; set; }

        public string PagingPageSizeMax { get; set; }
    }
}