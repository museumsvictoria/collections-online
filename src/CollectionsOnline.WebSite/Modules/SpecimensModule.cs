using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Factories;
using CollectionsOnline.WebSite.Queries;
using Nancy;
using Raven.Client;

namespace CollectionsOnline.WebSite.Modules
{
    public class SpecimensModule : NancyModule
    {
        public SpecimensModule(
            ISpecimenViewModelQuery specimenViewModelQuery,
            IDocumentSession documentSession,
            IMetadataViewModelFactory metadataViewModelFactory)            
        {
            Get["/specimens/{id}"] = parameters =>
            {
                var specimen = documentSession.Load<Specimen>("specimens/" + parameters.id as string);

                if (specimen == null || specimen.IsHidden) 
                    return HttpStatusCode.NotFound;

                ViewBag.metadata = metadataViewModelFactory.Make(specimen);
                
                return View["Specimens", specimenViewModelQuery.BuildSpecimen("specimens/" + parameters.id)];
            };
        }
    }
}