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

        [Fact]
        public void HtmlSanitizer_WithMalformedHTML_ReturnsSanitizedHTML()
        {
            // Given
            string result;
            var html = @"<h3><font size=""3"">Peter Hunter OAM, Kodak Australasia Pty Ltd: Public Relations, 1961 - 1995</font></h3><h3><font size=""3"">Early career in England</font></h3>";
            
            // When
            result = HtmlConverter.HtmlSanitizer(html).Html;
            
            result.ShouldBe("<h3>Peter Hunter OAM, Kodak Australasia Pty Ltd: Public Relations, 1961 - 1995</h3><h3>Early career in England</h3>");
        }
    }
}