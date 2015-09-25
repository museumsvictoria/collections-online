using AutoMapper;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Models.Api;
using CollectionsOnline.WebSite.Queries;
using Nancy;
using Raven.Client;

namespace CollectionsOnline.WebSite.Modules.Api
{
    public class SpecimensApiModule : ApiModuleBase
    {
        public SpecimensApiModule(
            IDocumentSession documentSession,
            ISpecimenViewModelQuery specimenViewModelQuery)
            : base("/specimens")
        {
            Get["specimens-api-index", "/"] = parameters =>
                {
                    var apiViewModel = specimenViewModelQuery.BuildSpecimenApiIndex(ApiInputModel);

                    return BuildResponse(apiViewModel.Results, apiPageInfo: apiViewModel.ApiPageInfo);
                };

            Get["specimens-api-by-id", "/{id}"] = parameters =>
                {
                    var specimen = documentSession.Load<Specimen>("specimens/" + parameters.id as string);

                    return (specimen == null || specimen.IsHidden) ? HttpStatusCode.NotFound : BuildResponse(Mapper.Map<Specimen, SpecimenApiViewModel>(specimen));
                };
        }
    }
}