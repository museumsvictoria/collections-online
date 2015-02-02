using System;
using System.Collections.Generic;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Models;
using FizzWare.NBuilder;
using Nancy;
using Nancy.Metadata.Module;
using Newtonsoft.Json;
using Raven.Client;

namespace CollectionsOnline.WebApi.Metadata
{
    public class ItemMetadataModule : MetadataModule<WebApiMetadata>
    {
        public ItemMetadataModule(IDocumentStore documentStore)
        {
            Describe["items-index"] = description =>
            {
                return new WebApiMetadata
                {
                    Method = description.Method,
                    Path = description.Path.Replace(Constants.CurrentWebApiVersionPathSegment, string.Empty),
                    Description = "Returns a bunch of items.",
                    StatusCodes = new Dictionary<HttpStatusCode, string>
                    {
                        { HttpStatusCode.OK, "A bunch of items were able to be retrieved ok." }
                    },
                    SampleResponse = JsonConvert.SerializeObject(new []
                    {
                        Builder<Item>
                            .CreateNew()
                            .With(x => x.Id = "items/1")
                            .With(x => x.DateModified = new DateTime(2015, 1, 1))
                            .With(x => x.Associations = Builder<Association>
                                .CreateListOfSize(1)
                                .Build())
                            .Build()
                    }, Formatting.Indented),
                    ExampleUrl = description.Path.Replace(Constants.CurrentWebApiVersionPathSegment, string.Empty),
                };
            };

            Describe["items-by-id"] = description =>
            {
                using (var documentSession = documentStore.OpenSession())
                {
                    return new WebApiMetadata
                    {
                        Method = description.Method,
                        Path = description.Path.Replace(Constants.CurrentWebApiVersionPathSegment, string.Empty),
                        Description = "Returns a single item by Id.",
                        StatusCodes = new Dictionary<HttpStatusCode, string>
                        {
                            {HttpStatusCode.OK, "The item was found and retrieved ok."},
                            {HttpStatusCode.NotFound, "The item could not be found and probably does not exist."}
                        },
                        SampleResponse = JsonConvert.SerializeObject(Builder<Item>
                            .CreateNew()
                            .With(x => x.Id = "items/1")
                            .With(x => x.DateModified = new DateTime(2015, 1, 1))
                            .With(x => x.Associations = Builder<Association>
                                .CreateListOfSize(1)
                                .Build())
                            .Build(), Formatting.Indented),
                        ExampleUrl = documentSession.Advanced
                            .DocumentQuery<Item, Combined>()
                            .WhereEquals("Type", "Item")
                            .First().Id,
                    };
                }
            };
        }
    }
}