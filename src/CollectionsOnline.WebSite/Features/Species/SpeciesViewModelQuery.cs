using Raven.Client;

namespace CollectionsOnline.WebSite.Features.Species
{
    public class SpeciesViewModelQuery : ISpeciesViewModelQuery
    {
        private readonly IDocumentSession _documentSession;
        private readonly ISpeciesViewModelFactory _speciesViewModelFactory;

        public SpeciesViewModelQuery(
            IDocumentSession documentSession,
            ISpeciesViewModelFactory speciesViewModelFactory)
        {
            _documentSession = documentSession;
            _speciesViewModelFactory = speciesViewModelFactory;
        }

        public SpeciesViewModel BuildSpecies(string speciesId)
        {
            var species = _documentSession
                .Load<Core.Models.Species>(speciesId);

            return _speciesViewModelFactory.MakeViewModel(species);
        }
    }
}