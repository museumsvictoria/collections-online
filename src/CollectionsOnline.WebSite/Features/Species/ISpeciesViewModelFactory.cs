namespace CollectionsOnline.WebSite.Features.Species
{
    public interface ISpeciesViewModelFactory
    {
        SpeciesViewModel MakeViewModel(Core.Models.Species species);
    }
}