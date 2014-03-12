using System.Linq;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Models;
using Nancy;
using Raven.Client;

namespace CollectionsOnline.WebApi.Modules
{
    public class SpecimenModule : BaseModule
    {
        public SpecimenModule(IDocumentSession documentSession)
            : base("/specimens")
        {
            Get["/"] = parameters =>
                {
                    var specimens = documentSession
                        .Query<Specimen, SpecimensNotHidden>()
                        .Statistics(out Statistics)
                        .Skip(Offset)
                        .Take(Limit)
                        .ToList();

                    return BuildResponse(specimens);
                };

            Get["/{specimenId}"] = parameters =>
                {
                    string specimenId = parameters.specimenId;
                    var specimen = documentSession
                        .Load<Item>("specimens/" + specimenId);

                    return specimen == null ? BuildErrorResponse(HttpStatusCode.NotFound, "Specimen {0} not found", specimenId) : BuildResponse(specimen);
                };
        }
    }
}