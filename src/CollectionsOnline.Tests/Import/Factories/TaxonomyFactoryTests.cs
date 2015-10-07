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
            string result;
            var taxonomyFactory = new TaxonomyFactory();
            var taxonomy = new Taxonomy
            {
                Genus = "Colluricincla",
                Species = "harmonica",
                Subspecies = "rufiventris"
            };

            // When
            result = taxonomyFactory.MakeScientificName(QualifierRankType.None, null, taxonomy);

            // Then
            result.ShouldBe("<em>Colluricincla harmonica rufiventris</em>");
        }

        [Fact]
        public void MakeScientificName_WithSpeciesQualifier_ReturnsCorrectly()
        {
            // Given
            string result;
            var taxonomyFactory = new TaxonomyFactory();
            var taxonomy = new Taxonomy
            {
                Genus = "Maxomys",
                Species = "hellwaldii",
                Author = "(Jentink, 1878)"
            };

            // When
            result = taxonomyFactory.MakeScientificName(QualifierRankType.Species, "cf", taxonomy);

            // Then
            result.ShouldBe("<em>Maxomys</em> cf <em>hellwaldii</em> (Jentink, 1878)");
        }

        [Fact]
        public void MakeScientificName_WithGenusQualifier_ReturnsCorrectly()
        {
            // Given
            string result;
            var taxonomyFactory = new TaxonomyFactory();
            var taxonomy = new Taxonomy
            {
                Genus = "Austriella",
                Species = "corrugata",
                Author = "(Deshayes, 1843)"
            };

            // When
            result = taxonomyFactory.MakeScientificName(QualifierRankType.Genus, "df", taxonomy);

            // Then
            result.ShouldBe("df <em>Austriella corrugata</em> (Deshayes, 1843)");
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

            // Then
            result.Genus.ShouldBe("Calymenise or tulabile blumabacka");
        }

        private Map MakeEmptyTaxonomyMap()
        {
            var map = new Map
            {
                {"irn", default(int).ToString()},
                {"ClaKingdom", string.Empty},
                {"ClaPhylum", string.Empty},
                {"ClaSubphylum", string.Empty},
                {"ClaSuperclass", string.Empty},
                {"ClaClass", string.Empty},
                {"ClaSubclass", string.Empty},
                {"ClaSuperorder", string.Empty},
                {"ClaOrder", string.Empty},
                {"ClaSuborder", string.Empty},
                {"ClaInfraorder", string.Empty},
                {"ClaSuperfamily", string.Empty},
                {"ClaFamily", string.Empty},
                {"ClaSubfamily", string.Empty},
                {"ClaGenus", string.Empty},
                {"ClaSubgenus", string.Empty},
                {"ClaSpecies", string.Empty},
                {"ClaSubspecies", string.Empty},
                {"AutAuthorString", string.Empty},
                {"ClaApplicableCode", string.Empty},
                {
                    "comname", new Map[]
                    {
                        new Map
                        {
                            {"ComStatus_tab", string.Empty},
                            {"ComName_tab", string.Empty}
                        }
                    }
                }
            };
            
            return map;
        }
    }
}