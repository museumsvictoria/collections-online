using CollectionsOnline.Core.Factories;
using Shouldly;
using Xunit;

namespace CollectionsOnline.Tests.Core.Factories
{
    public class SlugFactoryTests
    {
        [Fact]
        public void MakeSlug_StripsOutUnwantedCharacters()
        {
            // Given
            var slugFactory = new SlugFactory();

            // When
            var result = slugFactory.MakeSlug("Mechanical Drawing - CSIRAC Computer, 'Detail 17' “ó”   & , P24133, 8 February 1955    ");

            // Then
            result.ShouldBe("mechanical-drawing-csirac-computer-detail-17-o-and-p24133-8-february-1955");
        }

        [Fact]
        public void MakeSlug_WithMaxLength_ReturnsCorrectly()
        {
            // Given
            var slugFactory = new SlugFactory();

            // When
            var result = slugFactory.MakeSlug("Mechanical Drawing - CSIRAC Computers", 35);

            // Then
            result.ShouldBe("mechanical-drawing-csirac-computers");
        }

        [Fact]
        public void MakeSlug_WithMaxLengthOfOne_ReturnsCorrectly()
        {
            // Given
            var slugFactory = new SlugFactory();

            // When
            var result = slugFactory.MakeSlug("Mechanical Drawing - CSIRAC Computers", 1);

            // Then
            result.ShouldBe("mechanical");
        }
        
        [Fact]
        public void MakeSlug_WithReasonableMaxLength_ReturnsCorrectly()
        {
            // Given
            var slugFactory = new SlugFactory();

            // When
            var result = slugFactory.MakeSlug("Mechanical Drawing - CSIRAC Computer, 'Detail 17'", 20);

            // Then
            result.ShouldBe("mechanical-drawing");
        }
    }
}