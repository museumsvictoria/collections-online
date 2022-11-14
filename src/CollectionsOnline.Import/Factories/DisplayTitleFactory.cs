using System;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Core.Models;

namespace CollectionsOnline.Import.Factories
{
    public class DisplayTitleFactory : IDisplayTitleFactory
    {
        private readonly ITaxonomyFactory _taxonomyFactory;
        
        public DisplayTitleFactory(ITaxonomyFactory taxonomyFactory)
        {
            _taxonomyFactory = taxonomyFactory;
        }

        public string Make(Article article)
        {
            var displayTitle = string.Empty;
            
            if (!string.IsNullOrWhiteSpace(article.Title))
                displayTitle = article.Title;

            if (string.IsNullOrWhiteSpace(displayTitle))
                displayTitle = "Article";

            return displayTitle;
        }

        public string Make(Item item)
        {
            var displayTitle = string.Empty;
            
            if (string.Equals(item.Category, "First Peoples", StringComparison.OrdinalIgnoreCase))
            {
                displayTitle = new[]
                {
                    new[]
                    {
                        item.FirstPeoplesLocalName,
                        item.FirstPeoplesMedium
                    }.Concatenate(" | "),
                    item.FirstPeoplesCulturalGroups.Concatenate(", "),
                    item.FirstPeoplesLocalities.Concatenate(", "),
                    item.FirstPeoplesDate
                }.Concatenate(". ");
            }
            else if (!string.IsNullOrWhiteSpace(item.ObjectName))
                displayTitle = item.ObjectName;

            if (string.IsNullOrWhiteSpace(displayTitle))
                displayTitle = "Item";

            return displayTitle;
        }

        public string Make(Species species)
        {
            var displayTitle = string.Empty;
            
            if (species.Taxonomy != null)
            {
                var scientificName = _taxonomyFactory.MakeScientificName(QualifierRankType.None, null, species.Taxonomy);

                displayTitle = new[]
                {
                    scientificName, 
                    species.Taxonomy.CommonName
                }.Concatenate(", ");
            }

            if (string.IsNullOrWhiteSpace(displayTitle))
                displayTitle = "Species";
            
            return displayTitle;
        }

        public string Make(Specimen specimen)
        {
            var displayTitle = string.Empty;
            
            var hasScientificName = false;
            if (string.Equals(specimen.Discipline, "Tektites", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(specimen.TektitesName))
                displayTitle = specimen.TektitesName;
            else if (string.Equals(specimen.Discipline, "Meteorites", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(specimen.MeteoritesName))
                displayTitle = $"{specimen.MeteoritesName} meteorite";
            else if (string.Equals(specimen.Discipline, "Petrology", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(specimen.PetrologyRockName))
                displayTitle = specimen.PetrologyRockName;
            else if (string.Equals(specimen.Discipline, "Mineralogy", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(specimen.MineralogySpecies))
                displayTitle = specimen.MineralogySpecies;
            else if (specimen.Taxonomy != null)
            {
                var scientificName = _taxonomyFactory.MakeScientificName(specimen.QualifierRank, specimen.Qualifier, specimen.Taxonomy);

                if (!string.IsNullOrWhiteSpace(scientificName))
                {
                    hasScientificName = true;
                    displayTitle = scientificName;
                }
            }

            displayTitle = new[]
            {
                specimen.ObjectName,
                displayTitle
            }.Concatenate(hasScientificName || specimen.HasGeoIdentification ? ", " : " ");
            
            if (string.IsNullOrWhiteSpace(displayTitle))
                displayTitle = "Specimen";

            return displayTitle;
        }
    }
}