using System.Linq;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Models;
using Nancy;
using Raven.Client;

namespace CollectionsOnline.WebApi.Modules
{
    public class SpeciesModule : BaseModule
    {
        public SpeciesModule(IDocumentSession documentSession)
            : base("/species")
        {
            Get["/"] = parameters =>
                {
                    var species = documentSession.Advanced
                        .DocumentQuery<Species, Combined>()
                        .WhereEquals("Type", "Species")
                        .Statistics(out Statistics)
                        .Skip(Offset)
                        .Take(Limit)
                        .ToList();                    

                    return BuildResponse(species);
                };

            Get["/{speciesId}"] = parameters =>
                {
                    string speciesId = parameters.speciesId;
                    var species = documentSession
                        .Load<Item>("species/" + speciesId);

                    return species == null ? BuildErrorResponse(HttpStatusCode.NotFound, "Species {0} not found", speciesId) : BuildResponse(species);
                };
        }
    }
}