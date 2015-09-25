using AutoMapper;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Models.Api;
using CollectionsOnline.WebSite.Queries;
using Nancy;
using Raven.Client;

namespace CollectionsOnline.WebSite.Modules.Api
{
    public class SpeciesApiModule : ApiModuleBase
    {
        public SpeciesApiModule(
            IDocumentSession documentSession,
            ISpeciesViewModelQuery speciesViewModelQuery)
            : base("/species")
        {
            Get["species-api-index", "/"] = parameters =>
                {
                    var apiViewModel = speciesViewModelQuery.BuildSpeciesApiIndex(ApiInputModel);

                    return BuildResponse(apiViewModel.Results, apiPageInfo: apiViewModel.ApiPageInfo);
                };

            Get["species-api-by-id", "/{id}"] = parameters =>
                {
                    var species = documentSession.Load<Species>("species/" + parameters.id as string);

                    return (species == null || species.IsHidden) ? HttpStatusCode.NotFound : BuildResponse(Mapper.Map<Species, SpeciesApiViewModel>(species));
                };
        }
    }
}