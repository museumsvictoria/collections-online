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

            // When
            result = taxonomyFactory.MakeScientificName(QualifierRankType.None, null, "Colluricincla", null, "harmonica",
                "rufiventris", null);

            // Then
            result.ShouldBe("<em>Colluricincla harmonica rufiventris</em>");
        }

        [Fact]
        public void MakeScientificName_WithSpeciesQualifier_ReturnsCorrectly()
        {
            // Given
            string result;
            var taxonomyFactory = new TaxonomyFactory();

            // When
            result = taxonomyFactory.MakeScientificName(QualifierRankType.Species, "cf", "Maxomys", null, "hellwaldii",
                null, "(Jentink, 1878)");

            // Then
            result.ShouldBe("<em>Maxomys</em> cf <em>hellwaldii</em> (Jentink, 1878)");
        }

        [Fact]
        public void MakeScientificName_WithGenusQualifier_ReturnsCorrectly()
        {
            // Given
            string result;
            var taxonomyFactory = new TaxonomyFactory();

            // When
            result = taxonomyFactory.MakeScientificName(QualifierRankType.Genus, "df", "Austriella", null, "corrugata",
                null, "(Deshayes, 1843)");

            // Then
            result.ShouldBe("df <em>Austriella corrugata</em> (Deshayes, 1843)");
        }
    }
}