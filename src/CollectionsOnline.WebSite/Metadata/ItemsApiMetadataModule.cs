using System;
using System.Collections.Generic;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Models;
using FizzWare.NBuilder;
using Nancy;
using Nancy.Metadata.Modules;
using Newtonsoft.Json;
using Raven.Client;

namespace CollectionsOnline.WebSite.Metadata
{
    public class ItemsApiMetadataModule : MetadataModule<ApiMetadata>
    {
        public ItemsApiMetadataModule(IDocumentStore documentStore)
        {
            Describe["items-api-index"] = description =>
            {
                return new ApiMetadata
                {
                    Name = description.Name,
                    Method = description.Method,
                    Path = description.Path.Replace(Constants.CurrentApiVersionPathSegment, string.Empty),
                    Description = "Returns a bunch of items.",
                    StatusCodes = new Dictionary<HttpStatusCode, string>
                    {
                        { HttpStatusCode.OK, "A bunch of items were able to be retrieved ok." }
                    },
                    SampleResponse = JsonConvert.SerializeObject(SampleItems.CreateList(), Formatting.Indented),
                    ExampleUrl = description.Path.Replace(Constants.CurrentApiVersionPathSegment, string.Empty),
                };
            };

            Describe["items-api-by-id"] = description =>
            {
                using (var documentSession = documentStore.OpenSession())
                {
                    return new ApiMetadata
                    {
                        Name = description.Name,
                        Method = description.Method,
                        Path = description.Path.Replace(Constants.CurrentApiVersionPathSegment, string.Empty),
                        Description = "Returns a single item by Id.",
                        StatusCodes = new Dictionary<HttpStatusCode, string>
                        {
                            {HttpStatusCode.OK, "The item was found and retrieved ok."},
                            {HttpStatusCode.NotFound, "The item could not be found and probably does not exist."}
                        },
                        SampleResponse = JsonConvert.SerializeObject(SampleItems.Create(), Formatting.Indented),
                        ExampleUrl = string.Format("{0}/{1}", Constants.ApiBasePath, documentSession.Advanced.DocumentQuery<Item, Combined>().WhereEquals("Type", "Item").First().Id),
                    };
                }
            };
        }
    }
}