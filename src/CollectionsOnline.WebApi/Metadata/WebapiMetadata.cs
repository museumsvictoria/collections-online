using System;
using System.Collections.Generic;
using Nancy;

namespace CollectionsOnline.WebApi.Metadata
{
    public class WebApiMetadata
    {
        public string Name { get; set; }

        public string Method { get; set; }
        
        public string Path { get; set; }

        public string Description { get; set; }

        public IEnumerable<Tuple<string, string, string>> Parameters { get; set; }

        public IDictionary<HttpStatusCode, string> StatusCodes { get; set; }

        public string SampleResponse { get; set; }

        public string ExampleUrl { get; set; }
    }    
}