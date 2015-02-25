using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Models.Api;
using Nancy;
using Raven.Client;

namespace CollectionsOnline.WebSite.Modules.Api
{
    public class SpecimensApiModule : BaseApiModule
    {
        public SpecimensApiModule(IDocumentSession documentSession)
            : base("/specimens")
        {
            Get["specimens-api-index", "/"] = parameters =>
                {
                    var specimens = documentSession.Advanced
                        .DocumentQuery<Specimen, Combined>()
                        .WhereEquals("Type", "Specimen")
                        .Statistics(out Statistics)
                        .Skip(Offset)
                        .Take(Limit)
                        .ToList();

                    return BuildResponse(Mapper.Map<IEnumerable<Specimen>, IEnumerable<SpecimenApiViewModel>>(specimens));
                };

            Get["specimens-api-by-id", "/{id}"] = parameters =>
                {
                    var specimen = documentSession.Load<Specimen>("specimens/" + parameters.id as string);

                    return (specimen == null || specimen.IsHidden) ? BuildErrorResponse(HttpStatusCode.NotFound, "Specimen {0} not found", parameters.id) : BuildResponse(Mapper.Map<Specimen, SpecimenApiViewModel>(specimen));
                };
        }
    }
}