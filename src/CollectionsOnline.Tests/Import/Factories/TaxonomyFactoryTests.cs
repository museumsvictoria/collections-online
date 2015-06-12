using CollectionsOnline.Core.Models;
using CollectionsOnline.Import.Factories;
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
    }
}