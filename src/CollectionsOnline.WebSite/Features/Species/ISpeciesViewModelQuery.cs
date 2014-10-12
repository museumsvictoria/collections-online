namespace CollectionsOnline.WebSite.Features.Species
{
    public interface ISpeciesViewModelQuery
    {
        SpeciesViewTransformerResult BuildSpecies(string speciesId);
    }
}