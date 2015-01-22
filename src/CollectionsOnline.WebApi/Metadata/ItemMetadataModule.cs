using System.Collections.Generic;
using System.Linq;
using CollectionsOnline.Core.Models;
using Nancy;
using Nancy.Metadata.Module;
using Newtonsoft.Json;

namespace CollectionsOnline.WebApi.Metadata
{
    public class ItemMetadataModule : MetadataModule<WebapiMetadata>
    {
        public ItemMetadataModule()
        {
            Describe["items-index"] = description =>
            {
                return new WebapiMetadata
                {
                    Method = description.Method,
                    Path = description.Path,
                    Description = "Returns a bunch of items.",
                    StatusCodes = new Dictionary<HttpStatusCode, string>
                    {
                        { HttpStatusCode.OK, "A bunch of items were able to be retrieved ok." }
                    },
                    SampleResponse = JsonConvert.SerializeObject(new []
                    {
                        new Item
                        {
                            Id = "items/1",
                            ObjectName = "Example object name"
                        }
                    }, Formatting.Indented),
                    ExampleUrl = description.Path,
                };
            };

            Describe["items-by-id"] = description =>
            {
                return new WebapiMetadata
                {
                    Method = description.Method,
                    Path = description.Path,
                    Description = "Returns a single item by Id.",
                    StatusCodes = new Dictionary<HttpStatusCode, string>
                    {
                        { HttpStatusCode.OK, "The item was found and retrieved ok." },
                        { HttpStatusCode.NotFound, "The item could not be found and probably does not exist." }
                    },
                    SampleResponse = JsonConvert.SerializeObject(new Item
                        {
                            Id = "items/1",
                            ObjectName = "Example object name"
                        }, Formatting.Indented),
                    ExampleUrl = "/v1/items/791773",
                };
            };
        }
    }
}