using CollectionsOnline.Core.Models;

namespace CollectionsOnline.WebSite.Features.Specimens
{
    public class SpecimenViewModelFactory : ISpecimenViewModelFactory
    {
        public SpecimenViewModel MakeViewModel(Specimen specimen)
        {
            var specimenViewModel = new SpecimenViewModel
                {
                    Specimen = specimen
                };

            return specimenViewModel;
        }
    }
}