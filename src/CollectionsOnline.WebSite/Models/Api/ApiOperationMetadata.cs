using System.Collections.Generic;

namespace CollectionsOnline.WebSite.Models.Api
{
    public class ApiOperationMetadata
    {
        public string Name { get; set; }

        public IEnumerable<ApiMetadata> Metadata { get; set; }
    }    
}