using CollectionsOnline.WebSite.Transformers;

namespace CollectionsOnline.WebSite.Queries
{
    public interface ISpeciesViewModelQuery
    {
        SpeciesViewTransformerResult BuildSpecies(string speciesId);
    }
}