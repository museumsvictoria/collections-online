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
    public class SpeciesApiModule : BaseApiModule
    {
        public SpeciesApiModule(IDocumentSession documentSession)
            : base("/species")
        {
            Get["species-api-index", "/"] = parameters =>
                {
                    var species = documentSession.Advanced
                        .DocumentQuery<Species, CombinedIndex>()
                        .WhereEquals("Type", "Species")
                        .Statistics(out Statistics)
                        .Skip(Offset)
                        .Take(Limit)
                        .ToList();

                    return BuildResponse(Mapper.Map<IEnumerable<Species>, IEnumerable<SpeciesApiViewModel>>(species));
                };

            Get["species-api-by-id", "/{id}"] = parameters =>
                {
                    var species = documentSession.Load<Species>("species/" + parameters.id as string);

                    return (species == null || species.IsHidden) ? BuildErrorResponse(HttpStatusCode.NotFound, "Species {0} not found", parameters.id) : BuildResponse(Mapper.Map<Species, SpeciesApiViewModel>(species));
                };
        }
    }
}