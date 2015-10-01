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

        public IList<ApiParameter> Parameters { get; set; }

        public IDictionary<HttpStatusCode, string> StatusCodes { get; set; }

        public string SampleResponse { get; set; }

        public string ExampleUrl { get; set; }

        public ApiMetadata()
        {
            Parameters = new List<ApiParameter>();
            StatusCodes = new Dictionary<HttpStatusCode, string>();
        }
    }

    public class ApiParameter
    {
        public string Parameter { get; set; }

        public string Necessity { get; set; }

        public string Description { get; set; }

        public IList<ApiParameterValidValue> ValidValues { get; set; }

        public IList<string> ExampleValues { get; set; }

        public ApiParameter()
        {
            ValidValues = new List<ApiParameterValidValue>();
            ExampleValues = new List<string>();
        }
    }

    public class ApiParameterValidValue
    {
        public string Name { get; set; }

        public string Description { get; set; }
    }
}