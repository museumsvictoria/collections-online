using CollectionsOnline.WebSite.Models.Api;
using CollectionsOnline.WebSite.Transformers;

namespace CollectionsOnline.WebSite.Queries
{
    public interface ISpecimenViewModelQuery
    {
        SpecimenViewTransformerResult BuildSpecimen(string specimenId);

        ApiViewModel BuildSpecimenApiIndex(ApiInputModel apiInputModel);
    }
}