using CollectionsOnline.Core.Models;

namespace CollectionsOnline.WebSite.Features.Specimens
{
    public interface ISpecimenViewModelFactory
    {
        SpecimenViewModel MakeViewModel(Specimen specimen);
    }
}