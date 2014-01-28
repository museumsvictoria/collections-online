using Nancy;
using Raven.Client;

namespace CollectionsOnline.WebSite.Features.Species
{
    public class SpeciesModule : NancyModule
    {
        public SpeciesModule(
            ISpeciesViewModelQuery speciesViewModelQuery,
            IDocumentSession documentSession)            
        {
            Get["/species/{id}"] = parameters =>
            {
                var species = documentSession
                    .Load<Core.Models.Species>("species/" + parameters.id as string);

                return (species == null || species.IsHidden) ? HttpStatusCode.NotFound : View["species", speciesViewModelQuery.BuildSpecies("species/" + parameters.id)];
            };
        }
    }
}