using System.Collections.Generic;
using AutoMapper;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Core.Utilities;
using CollectionsOnline.WebSite.Models.Api;
using Nancy;
using Nancy.Metadata.Modules;
using Newtonsoft.Json;
using Raven.Client;

namespace CollectionsOnline.WebSite.Modules.Api
{
    public class SpecimensApiMetadataModule : MetadataModule<ApiMetadata>
    {
        public SpecimensApiMetadataModule(IDocumentStore documentStore)
        {
            using (new StopwatchTimer("Creation of Specimens Api Metadata complete"))
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
                        Path = description.Path.Replace(Constants.CurrentApiVersionPathSegment, string.Empty),
                        Description = "Returns a bunch of specimens.",
                        StatusCodes = new Dictionary<HttpStatusCode, string>
                        {
                            {HttpStatusCode.OK, "A bunch of specimens were able to be retrieved ok."}
                        },
                        SampleResponse = JsonConvert.SerializeObject(new[] { Mapper.Map<Specimen, SpecimenApiViewModel>(sampleSpecimen) }, Formatting.Indented),
                        ExampleUrl = description.Path.Replace(Constants.CurrentApiVersionPathSegment, string.Empty),
                    };
                };

                Describe["specimens-api-by-id"] = description =>
                {
                    return new ApiMetadata
                    {
                        Name = description.Name,
                        Method = description.Method,
                        Path = description.Path.Replace(Constants.CurrentApiVersionPathSegment, string.Empty),
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
                        SampleResponse = JsonConvert.SerializeObject(Mapper.Map<Specimen, SpecimenApiViewModel>(sampleSpecimen), Formatting.Indented),
                        ExampleUrl = (sampleSpecimen != null) ? string.Format("/{0}/{1}", Constants.ApiPathBase, sampleSpecimen.Id) : null,
                    };
                };
            }
        }
    }
}