using CollectionsOnline.Core.Utilities;
using Shouldly;
using Xunit;

namespace CollectionsOnline.Tests.Core.Utilities
{
    public class IdShortenerTests
    {
        [Fact]
        public void GivenId_EncodeThenDecode_ReturnsOriginalId()
        {
            // Given
            var id = "items/2110488";

            // When
            var shortId = IdShortener.Encode(id);
            var result = IdShortener.Decode(shortId);

            // Then
            result.ShouldBe(id);
        }
    }
}