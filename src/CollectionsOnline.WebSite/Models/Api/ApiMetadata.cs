using System.Collections.Generic;
using Nancy;

namespace CollectionsOnline.WebSite.Models.Api
{
    public class ApiMetadata
    {
        public string Name { get; set; }

        public string Method { get; set; }
        
        public string Path { get; set; }

        public string Description { get; set; }

        public IEnumerable<ApiParameter> Parameters { get; set; }

        public IDictionary<HttpStatusCode, string> StatusCodes { get; set; }

        public string SampleResponse { get; set; }

        public string ExampleUrl { get; set; }
    }

    public class ApiParameter
    {
        public string Parameter { get; set; }

        public string Type { get; set; }

        public string Description { get; set; }
    }
}