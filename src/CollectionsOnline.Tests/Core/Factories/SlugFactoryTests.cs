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
    }
}