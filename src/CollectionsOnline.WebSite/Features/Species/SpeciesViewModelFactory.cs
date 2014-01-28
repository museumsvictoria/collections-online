namespace CollectionsOnline.WebSite.Features.Species
{
    public class SpeciesViewModelFactory : ISpeciesViewModelFactory
    {
        public SpeciesViewModel MakeViewModel(Core.Models.Species species)
        {
            var speciesViewModel = new SpeciesViewModel
                {
                    Species = species
                };

            return speciesViewModel;
        }
    }
}