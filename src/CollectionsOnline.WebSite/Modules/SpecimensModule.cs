using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Queries;
using Nancy;
using Raven.Client;

namespace CollectionsOnline.WebSite.Modules
{
    public class SpecimensModule : NancyModule
    {
        public SpecimensModule(
            ISpecimenViewModelQuery specimenViewModelQuery,
            IDocumentSession documentSession)            
        {
            Get["/specimens/{id}"] = parameters =>
            {
                var specimen = documentSession.Load<Specimen>("specimens/" + parameters.id as string);

                return (specimen == null || specimen.IsHidden) ? HttpStatusCode.NotFound : View["Specimens", specimenViewModelQuery.BuildSpecimen("specimens/" + parameters.id)];
            };
        }
    }
}