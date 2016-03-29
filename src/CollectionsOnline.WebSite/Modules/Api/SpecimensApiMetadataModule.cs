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
    public class SpecimensApiMetadataModule : MetadataModule<ApiMetadata>
    {
        public SpecimensApiMetadataModule(IDocumentStore documentStore)
        {
            Log.Logger.Debug("Creating Specimen Api Metadata");

            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            };

            using (var documentSession = documentStore.OpenSession())
            {
                var sampleSpecimen = documentSession.Advanced.DocumentQuery<Specimen, CombinedIndex>()
                    .WhereEquals("RecordType", "Specimen")
                    .FirstOrDefault();

                Describe["specimens-api-index"] = description =>
                {
                    return new ApiMetadata
                    {
                        Name = description.Name,
                        Method = description.Method,
                        Path = description.Path,
                        Description = "Returns a bunch of specimens.",
                        StatusCodes = new Dictionary<HttpStatusCode, string>
                        {
                            {HttpStatusCode.OK, "A bunch of specimens were able to be retrieved ok."}
                        },
                        SampleResponse = JsonConvert.SerializeObject(new[] { Mapper.Map<Specimen, SpecimenApiViewModel>(sampleSpecimen) }, jsonSerializerSettings),
                        ExampleUrl = description.Path,
                    };
                };

                Describe["specimens-api-by-id"] = description =>
                {
                    return new ApiMetadata
                    {
                        Name = description.Name,
                        Method = description.Method,
                        Path = description.Path,
                        Description = "Returns a single specimen by Id.",
                        Parameters = new[]
                        {
                            new ApiParameter
                            {
                                Parameter = "Id",
                                Necessity = "required",
                                Description = "Id of specimen to be retrieved"
                            }
                        },
                        StatusCodes = new Dictionary<HttpStatusCode, string>
                        {
                            {HttpStatusCode.OK, "The specimen was found and retrieved ok."},
                            {HttpStatusCode.NotFound, "The specimen could not be found and probably does not exist."}
                        },
                        SampleResponse = JsonConvert.SerializeObject(Mapper.Map<Specimen, SpecimenApiViewModel>(sampleSpecimen), jsonSerializerSettings),
                        ExampleUrl = (sampleSpecimen != null) ? string.Format("{0}/{1}", Constants.ApiPathBase, sampleSpecimen.Id) : null,
                    };
                };
            }
        }
    }
}