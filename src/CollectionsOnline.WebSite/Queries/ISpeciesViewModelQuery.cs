using CollectionsOnline.WebSite.Models.Api;
using CollectionsOnline.WebSite.Transformers;

namespace CollectionsOnline.WebSite.Queries
{
    public interface ISpeciesViewModelQuery
    {
        SpeciesViewTransformerResult BuildSpecies(string speciesId);

        ApiViewModel BuildSpeciesApiIndex(ApiInputModel apiInputModel);
    }
}