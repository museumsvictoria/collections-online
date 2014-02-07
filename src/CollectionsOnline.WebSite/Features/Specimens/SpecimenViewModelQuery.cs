using CollectionsOnline.Core.Models;
using Raven.Client;

namespace CollectionsOnline.WebSite.Features.Specimens
{
    public class SpecimenViewModelQuery : ISpecimenViewModelQuery
    {
        private readonly IDocumentSession _documentSession;
        private readonly ISpecimenViewModelFactory _specimenViewModelFactory;

        public SpecimenViewModelQuery(
            IDocumentSession documentSession,
            ISpecimenViewModelFactory specimenViewModelFactory)
        {
            _documentSession = documentSession;
            _specimenViewModelFactory = specimenViewModelFactory;
        }

        public SpecimenViewModel BuildSpecimen(string specimenId)
        {
            var specimen = _documentSession
                .Load<Specimen>(specimenId);

            return _specimenViewModelFactory.MakeViewModel(specimen);
        }
    }
}