using System;
using System.Collections.Generic;
using System.Globalization;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Import.Factories;
using IMu;
using Shouldly;
using Xunit;

namespace CollectionsOnline.Tests.Import.Factories
{
    public class DisplayTitleFactoryTests
    {
        [Fact]
        public void MakeDisplayTitle_ForMineralogySpecimenWithIdentification_ReturnsCorrectly()
        {
            // Given
            var displayTitleFactory = new DisplayTitleFactory(new TaxonomyFactory());
            var specimen = MakeMineralogySpecimenWithIdentification();

            // When
            var result = displayTitleFactory.Make(specimen);

            // Then
            result.ShouldBe("The Crystal King, Quartz");
        }
        
        [Fact]
        public void MakeDisplayTitle_ForMammalogySpecimenWithTaxonomy_ReturnsCorrectly()
        {
            // Given
            var displayTitleFactory = new DisplayTitleFactory(new TaxonomyFactory());
            var specimen = MakeMammalogySpecimenWithTaxonomy();

            // When
            var result = displayTitleFactory.Make(specimen);

            // Then
            result.ShouldBe("Taxidermy Mount - Husky, (Ursa), 2001, <em>Canis lupus familiaris</em>");
        }

        [Fact]
        public void MakeDisplayTitle_ForFirstPeoplesItem_ReturnsCorrectyl()
        {
            // Given
            var displayTitleFactory = new DisplayTitleFactory(new TaxonomyFactory());
            var item = MakeFirstPeoplesItem();
            
            // When
            var result = displayTitleFactory.Make(item);
            
            // Then
            result.ShouldBe("<em>tunga</em>, Container. Tiwi. Bathurst or Melville Islands, North, Northern Territory, Australia. 1997");
        }

        private Specimen MakeMineralogySpecimenWithIdentification()
        {
            var specimen = new Specimen
            {
                HasGeoIdentification = true,
                Discipline = "Mineralogy",
                ObjectName = "The Crystal King",
                Qualifier = null,
                QualifierRank = QualifierRankType.None,
                Taxonomy = null,
                MineralogySpecies = "Quartz",
                MeteoritesName = null,
                TektitesName = null,
                PetrologyRockName = null
            };

            return specimen;
        }
        
        private Specimen MakeMammalogySpecimenWithTaxonomy()
        {
          var specimen = new Specimen
          {
            HasGeoIdentification = false,
            Discipline = "Mammalogy",
            ObjectName = "Taxidermy Mount - Husky, (Ursa), 2001",
            Qualifier = null,
            QualifierRank = QualifierRankType.None,
            Taxonomy = new Taxonomy
            {
              Irn = 5855L,
              Kingdom = "Animalia",
              Phylum = "Chordata",
              Subphylum = "Vertebrata",
              Superclass = null,
              Class = "Mammalia",
              Subclass = null,
              Superorder = null,
              Order = "Carnivora",
              Suborder = "Caniformia",
              Infraorder = null,
              Superfamily = null,
              Family = "Canidae",
              Subfamily = null,
              Genus = "Canis",
              Subgenus = null,
              Species = "lupus",
              Subspecies = "familiaris",
              Author = null,
              Code = "ICZN",
              TaxonRank = "Subspecies",
              TaxonName = "Canis lupus familiaris",
              CommonName = "Domestic Dog",
              OtherCommonNames = new List<string>
              {
                "Common Dog",
                "Dog"
              }
            },
            MineralogySpecies = null,
            MeteoritesName = null,
            TektitesName = null,
            PetrologyRockName = null,
          };

          return specimen;
        }

        private Item MakeFirstPeoplesItem()
        {
          return new Item
          {
            Category = "First Peoples",
            ObjectName = "bark basket",
            FirstPeoplesLocalities = new List<string>
            {
              "Bathurst or Melville Islands",
              "North",
              "Northern Territory",
              "Australia"
            },
            FirstPeoplesCulturalGroups = new List<string>
            {
              "Tiwi"
            },
            FirstPeoplesMedium = "Container",
            FirstPeoplesDescription =
              "A single sheet of bark, Eucalyptus sp., folded over and stitched down the two long sides with cane. It is painted with natural pigments. The string handle is attached at the rim at the rear.",
            FirstPeoplesLocalName = "<em>tunga</em>",
            FirstPeoplesPhotographer = null,
            FirstPeoplesAuthor = null,
            FirstPeoplesIllustrator = null,
            FirstPeoplesMaker = "Marie E. Pautjimi",
            FirstPeoplesDate = "1997",
            FirstPeoplesCollector = null,
            FirstPeoplesDateCollected = "c. 1997",
            FirstPeoplesIndividualsIdentified = null,
            FirstPeoplesTitle = null,
            FirstPeoplesSheets = null,
            FirstPeoplesPages = null,
            FirstPeoplesLetterTo = null,
            FirstPeoplesLetterFrom = null
          };
        }
    }
}