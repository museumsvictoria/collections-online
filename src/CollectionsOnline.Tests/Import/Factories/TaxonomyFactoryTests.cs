using System.Collections.Generic;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Import.Factories;
using IMu;
using Shouldly;
using Xunit;

namespace CollectionsOnline.Tests.Import.Factories
{
    public class TaxonomyFactoryTests
    {
        [Fact]
        public void MakeScientificName_WithNoQualifierOrAuthor_ReturnsCorrectly()
        {
            // Given
            var taxonomyFactory = new TaxonomyFactory();
            var taxonomy = new Taxonomy
            {
                Genus = "Colluricincla",
                Species = "harmonica",
                Subspecies = "rufiventris"
            };

            // When
            var result = taxonomyFactory.MakeScientificName(QualifierRankType.None, null, taxonomy);

            // Then
            result.ShouldBe("<em>Colluricincla harmonica rufiventris</em>");
        }

        [Fact]
        public void MakeScientificName_WithSpeciesQualifier_ReturnsCorrectly()
        {
            // Given
            var taxonomyFactory = new TaxonomyFactory();
            var taxonomy = new Taxonomy
            {
                Genus = "Maxomys",
                Species = "hellwaldii",
                Author = "(Jentink, 1878)"
            };

            // When
            var result = taxonomyFactory.MakeScientificName(QualifierRankType.Species, "cf", taxonomy);

            // Then
            result.ShouldBe("<em>Maxomys</em> cf <em>hellwaldii</em> (Jentink, 1878)");
        }

        [Fact]
        public void MakeScientificName_WithGenusQualifier_ReturnsCorrectly()
        {
            // Given
            var taxonomyFactory = new TaxonomyFactory();
            var taxonomy = new Taxonomy
            {
                Genus = "Austriella",
                Species = "corrugata",
                Author = "(Deshayes, 1843)"
            };

            // When
            var result = taxonomyFactory.MakeScientificName(QualifierRankType.Genus, "df", taxonomy);

            // Then
            result.ShouldBe("df <em>Austriella corrugata</em> (Deshayes, 1843)");
        }
        
        [Fact]
        public void MakeScientificName_WithEmptySpeciesQualifier_ReturnsCorrectly()
        {
            // Given
            var taxonomyFactory = new TaxonomyFactory();
            var taxonomy = new Taxonomy
            {
                Genus = "Pardalotus",
                Species = "striatus",
                Family = "Pardalotidae"
            };

            // When
            var result = taxonomyFactory.MakeScientificName(QualifierRankType.Species, "", taxonomy);

            // Then
            result.ShouldBe("<em>Pardalotus striatus</em>");
        }

        [Fact]
        public void MakeTaxonomy_WithInvalidGenusCharacters_AreRemoved()
        {
            // Given
            var map = MakeEmptyTaxonomyMap();
            map["ClaGenus"] = "Calymenise\" or \"tulabile blumabacka\"";
            var taxonomyFactory = new TaxonomyFactory();

            // When
            var result = taxonomyFactory.Make(map);

            // Then;
            result.Genus.ShouldBe("Calymenise or tulabile blumabacka");
        }

        [Fact]
        public void MakeTaxonomy_WithNullField_ReturnsNull()
        {
            // Given
            var map = MakeEmptyTaxonomyMap();
            var taxonomyFactory = new TaxonomyFactory();

            // When
            var result = taxonomyFactory.Make(map);

            // Then;
            result.ShouldBe(null);
        }

        private Map MakeEmptyTaxonomyMap()
        {
            var map = new Map
            {
                {"irn", default(int).ToString()},
                {"ClaKingdom", null},
                {"ClaPhylum", null},
                {"ClaSubphylum", null},
                {"ClaSuperclass", null},
                {"ClaClass", null},
                {"ClaSubclass", null},
                {"ClaSuperorder", null},
                {"ClaOrder", null},
                {"ClaSuborder", null},
                {"ClaInfraorder", null},
                {"ClaSuperfamily", null},
                {"ClaFamily", null},
                {"ClaSubfamily", null},
                {"ClaGenus", null},
                {"ClaSubgenus", null},
                {"ClaSpecies", null},
                {"ClaSubspecies", null},
                {"AutAuthorString", null},
                {"ClaApplicableCode", null},
                {
                    "comname", new[]
                    {
                        new Map
                        {
                            {"ComStatus_tab", null},
                            {"ComName_tab", null}
                        }
                    }
                }
            };
            
            return map;
        }
    }
}