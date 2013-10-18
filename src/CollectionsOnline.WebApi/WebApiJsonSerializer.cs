using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CollectionsOnline.WebApi
{
    public sealed class WebApiJsonSerializer : JsonSerializer
    {
        public WebApiJsonSerializer()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver();
            Formatting = Formatting.Indented;
        }
    }
}