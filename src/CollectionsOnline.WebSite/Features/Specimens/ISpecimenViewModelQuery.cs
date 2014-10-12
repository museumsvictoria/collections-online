using CollectionsOnline.WebSite.Features.Items;

namespace CollectionsOnline.WebSite.Features.Specimens
{
    public interface ISpecimenViewModelQuery
    {
        SpecimenViewTransformerResult BuildSpecimen(string specimenId);
    }
}