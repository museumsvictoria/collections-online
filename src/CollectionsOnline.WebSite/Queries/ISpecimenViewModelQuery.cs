using CollectionsOnline.WebSite.Transformers;

namespace CollectionsOnline.WebSite.Queries
{
    public interface ISpecimenViewModelQuery
    {
        SpecimenViewTransformerResult BuildSpecimen(string specimenId);
    }
}