using System.Collections.Generic;
using AutoMapper;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Models;
using CollectionsOnline.WebSite.Models.Api;
using CollectionsOnline.WebSite.Queries;
using Nancy;
using Nancy.Metadata.Modules;
using Newtonsoft.Json;
using Raven.Client;

namespace CollectionsOnline.WebSite.Modules.Api
{
    public class SearchApiMetadataModule : MetadataModule<ApiMetadata>
    {
        public SearchApiMetadataModule()
        {
            Describe["search-api"] = description =>
            {
                return new ApiMetadata
                {
                    Name = description.Name,
                    Method = description.Method,
                    Path = description.Path.Replace(Constants.CurrentApiVersionPathSegment, string.Empty),
                    Description = "Search the collection.",
                    StatusCodes = new Dictionary<HttpStatusCode, string>
                    {
                        { HttpStatusCode.OK, "A bunch of things were able to be searched ok." }
                    },                    
                    ExampleUrl = description.Path.Replace(Constants.CurrentApiVersionPathSegment, string.Empty),
                };
            };
        }
    }
}