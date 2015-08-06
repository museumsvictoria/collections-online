using CollectionsOnline.Core.Utilities;
using Shouldly;
using Xunit;

namespace CollectionsOnline.Tests.Core.Utilities
{
    public class HtmlConverterTests
    {
        [Fact]
        public void HtmlToText_WithScientificName_ReturnsOnlyText()
        {
            // Given
            string result;
            var html = "df <em>Austriella corrugata</em> (Deshayes, 1843)";

            // When
            result = HtmlConverter.HtmlToText(html);

            // Then
            result.ShouldBe("df Austriella corrugata (Deshayes, 1843)");
        }
    }
}