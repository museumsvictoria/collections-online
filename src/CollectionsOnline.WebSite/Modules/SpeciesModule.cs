using CollectionsOnline.WebSite.Factories;
using CollectionsOnline.WebSite.Queries;
using Nancy;
using Raven.Client;

namespace CollectionsOnline.WebSite.Modules
{
    public class SpeciesModule : NancyModule
    {
        public SpeciesModule(
            ISpeciesViewModelQuery speciesViewModelQuery,
            IDocumentSession documentSession,
            IMetadataViewModelFactory metadataViewModelFactory)            
        {
            Get["/species/{id}"] = parameters =>
            {
                var species = documentSession.Load<Core.Models.Species>("species/" + parameters.id as string);

                if (species == null || species.IsHidden) 
                    return HttpStatusCode.NotFound;

                ViewBag.metadata = metadataViewModelFactory.Make(species);
                
                return View["Species", speciesViewModelQuery.BuildSpecies("species/" + parameters.id)];
            };
        }
    }
}