using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CollectionsOnline.WebSite.Infrastructure
{
    public sealed class ApiJsonSerializer : JsonSerializer
    {
        public ApiJsonSerializer()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver();
            Formatting = Formatting.Indented;
        }
    }
}