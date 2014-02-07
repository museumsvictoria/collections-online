using CollectionsOnline.Core.Models;
using Nancy;
using Raven.Client;

namespace CollectionsOnline.WebSite.Features.Specimens
{
    public class SpecimensModule : NancyModule
    {
        public SpecimensModule(
            ISpecimenViewModelQuery specimenViewModelQuery,
            IDocumentSession documentSession)            
        {
            Get["/specimens/{id}"] = parameters =>
            {
                var specimen = documentSession
                    .Load<Specimen>("specimens/" + parameters.id as string);

                return (specimen == null || specimen.IsHidden) ? HttpStatusCode.NotFound : View["specimens", specimenViewModelQuery.BuildSpecimen("specimens/" + parameters.id)];
            };
        }
    }
}