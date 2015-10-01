using System.Collections.Generic;
using AutoMapper;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Models.Api;
using Nancy;
using Nancy.Metadata.Modules;
using Newtonsoft.Json;
using Raven.Client;

namespace CollectionsOnline.WebSite.Modules.Api
{
    public class SpeciesApiMetadataModule : MetadataModule<ApiMetadata>
    {
        public SpeciesApiMetadataModule(IDocumentStore documentStore)
        {
            using (var documentSession = documentStore.OpenSession())
            {
                var sampleSpecies = documentSession.Advanced.DocumentQuery<Species, CombinedIndex>()
                    .WhereEquals("RecordType", "Species")
                    .FirstOrDefault();

                Describe["species-api-index"] = description =>
                {
                    return new ApiMetadata
                    {
                        Name = description.Name,
                        Method = description.Method,
                        Path = description.Path.Replace(Constants.CurrentApiVersionPathSegment, string.Empty),
                        Description = "Returns a bunch of species.",
                        StatusCodes = new Dictionary<HttpStatusCode, string>
                        {
                            {HttpStatusCode.OK, "A bunch of species were able to be retrieved ok."}
                        },
                        SampleResponse = JsonConvert.SerializeObject(new[] { Mapper.Map<Species, SpeciesApiViewModel>(sampleSpecies) }, Formatting.Indented),
                        ExampleUrl = description.Path.Replace(Constants.CurrentApiVersionPathSegment, string.Empty)
                    };
                };

                Describe["species-api-by-id"] = description =>
                {
                    return new ApiMetadata
                    {
                        Name = description.Name,
                        Method = description.Method,
                        Path = description.Path.Replace(Constants.CurrentApiVersionPathSegment, string.Empty),
                        Description = "Returns a single species by Id.",
                        Parameters = new[]
                        {
                            new ApiParameter
                            {
                                Parameter = "Id",
                                Necessity = "required",
                                Description = "Id of species to be retrieved"                                
                            }
                        },
                        StatusCodes = new Dictionary<HttpStatusCode, string>
                        {
                            {HttpStatusCode.OK, "The species was found and retrieved ok."},
                            {HttpStatusCode.NotFound, "The species could not be found and probably does not exist."}
                        },
                        SampleResponse = JsonConvert.SerializeObject(Mapper.Map<Species, SpeciesApiViewModel>(sampleSpecies), Formatting.Indented),
                        ExampleUrl = (sampleSpecies != null) ? string.Format("/{0}/{1}", Constants.ApiPathBase, sampleSpecies.Id) : null,
                    };
                };
            }
        }
    }
}