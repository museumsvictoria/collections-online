namespace CollectionsOnline.WebSite.Features.Species
{
    public interface ISpeciesViewModelQuery
    {
        SpeciesViewModel BuildSpecies(string speciesId);
    }
}