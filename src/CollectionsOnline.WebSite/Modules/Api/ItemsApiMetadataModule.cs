using System.Collections.Generic;
using AutoMapper;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Models.Api;
using Nancy;
using Nancy.Metadata.Modules;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Raven.Client;
using Serilog;

namespace CollectionsOnline.WebSite.Modules.Api
{
    public class ItemsApiMetadataModule : MetadataModule<ApiMetadata>
    {
        public ItemsApiMetadataModule(IDocumentStore documentStore)
        {
            Log.Logger.Debug("Creating Item Api Metadata");

            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            };

            using (var documentSession = documentStore.OpenSession())
            {
                var sampleItem = documentSession.Advanced.DocumentQuery<Item, CombinedIndex>()
                    .WhereEquals("RecordType", "Item")
                    .FirstOrDefault();

                Describe["items-api-index"] = description =>
                {
                    return new ApiMetadata
                    {
                        Name = description.Name,
                        Method = description.Method,
                        Path = description.Path,
                        Description = "Returns a bunch of items.",
                        StatusCodes = new Dictionary<HttpStatusCode, string>
                        {
                            {HttpStatusCode.OK, "A bunch of items were able to be retrieved ok."}
                        },
                        SampleResponse = JsonConvert.SerializeObject(new[] { Mapper.Map<Item, ItemApiViewModel>(sampleItem) }, jsonSerializerSettings),
                        ExampleUrl = description.Path,
                    };
                };

                Describe["items-api-by-id"] = description =>
                {
                    return new ApiMetadata
                    {
                        Name = description.Name,
                        Method = description.Method,
                        Path = description.Path,
                        Description = "Returns a single item by Id.",
                        Parameters = new[]
                        {
                            new ApiParameter
                            {
                                Parameter = "Id",
                                Necessity = "required",
                                Description = "Id of item to be retrieved"
                            }
                        },
                        StatusCodes = new Dictionary<HttpStatusCode, string>
                        {
                            {HttpStatusCode.OK, "The item was found and retrieved ok."},
                            {HttpStatusCode.NotFound, "The item could not be found and probably does not exist."}
                        },
                        SampleResponse = JsonConvert.SerializeObject(Mapper.Map<Item, ItemApiViewModel>(sampleItem), jsonSerializerSettings),
                        ExampleUrl = (sampleItem != null) ? string.Format("{0}/{1}", Constants.ApiPathBase, sampleItem.Id) : null,
                    };
                };
            }
        }
    }
}