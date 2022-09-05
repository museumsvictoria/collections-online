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
        
        [Fact]
        public void TruncateHtml_GivenNoHtml()
        {
            // Given
            var input =
                "English artist Richard Du Bourg (1738-1826) learned the emerging technique of cork modelling of classical sites while living in Italy in the 1760s.";

            // When
            var result = input.TruncateHtml(50, " ...");

            // Then
            result.ShouldBe("English artist Richard Du Bourg (1738-1826) ...");
        }
        
        [Fact]
        public void TruncateHtml_GivenHtmlOneLevelDeep()
        {
            // Given
            var input =
                "<div>English artist Richard Du Bourg (1738-1826) learned the emerging technique of cork modelling of classical sites while living in Italy in the 1760s.</div>";

            // When
            var result = input.TruncateHtml(50, " ...");

            // Then
            result.ShouldBe("<div>English artist Richard Du Bourg (1738-1826) ...</div>");
        }
        
        [Fact]
        public void TruncateHtml_GivenHtmlManyLevelsDeep()
        {
            // Given
            var input =
                "<div><h2>English artist Richard Du Bourg</h2> <strong>(1738-1826)</strong> <span>learned <span>the emerging <span>technique of cork</span> modelling of classical sites</span> while living in Italy in the 1760s.</span></div>";

            // When
            var result = input.TruncateHtml(50, " ...");

            // Then
            result.ShouldBe("<div><h2>English artist Richard Du Bourg</h2> <strong>(1738-1826)</strong> <span>learned ...</span></div>");
        }
    }
}