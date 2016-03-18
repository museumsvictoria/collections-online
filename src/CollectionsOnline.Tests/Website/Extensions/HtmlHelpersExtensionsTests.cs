using CollectionsOnline.WebSite.Extensions;
using CollectionsOnline.WebSite.Transformers;
using Nancy.ViewEngines.Razor;
using Shouldly;
using Xunit;

namespace CollectionsOnline.Tests.Website.Extensions
{
    public class HtmlHelpersExtensionsTests
    {
        [Fact]
        public void ConvertNewlines_ReturnsMarkup()
        {
            // Given 
            var objectSummary = "Candlestick from Room 21 - Chinese Bedroom, Main Building Section, Pendle Hall. From top of dressing table.\n\nIt was created by Felicity Clemons as part of a 40 project to create a grand fully-furnished Georgian style country dolls' house. The project began in the 1940's when Felicity's daughter, Antonia, was given a three-room dolls' house by her grandmother. Mrs Clemons was not happy with its scale or authenticity and so began a rebuilding project which resulted in a four storey grand house, now known as Pendle Hall. Although some items were purchased during shopping trips to Melbourne, the majority of furnishings were constructed by Felicity's own hands, using numerous reference books and magazines.";
            var itemHtmlHelper = new HtmlHelpers<ItemViewTransformerResult>(null, null, new ItemViewTransformerResult());

            //When
            var result = itemHtmlHelper.ConvertNewlines(objectSummary);            

            // Then
            result.ToHtmlString().ShouldBe("Candlestick from Room 21 - Chinese Bedroom, Main Building Section, Pendle Hall. From top of dressing table.<br /><br />It was created by Felicity Clemons as part of a 40 project to create a grand fully-furnished Georgian style country dolls' house. The project began in the 1940's when Felicity's daughter, Antonia, was given a three-room dolls' house by her grandmother. Mrs Clemons was not happy with its scale or authenticity and so began a rebuilding project which resulted in a four storey grand house, now known as Pendle Hall. Although some items were purchased during shopping trips to Melbourne, the majority of furnishings were constructed by Felicity's own hands, using numerous reference books and magazines.");
        }

        [Fact]
        public void ConvertNewlines_ReturnsNonEncodedHtmlString()
        {
            // Given 
            var objectSummary = "Candlestick from Room 21 - Chinese Bedroom, Main Building Section, Pendle Hall. From top of dressing table.\n\nIt was created by Felicity Clemons as part of a 40 project to create a grand fully-furnished Georgian style country dolls' house. The project began in the 1940's when Felicity's daughter, Antonia, was given a three-room dolls' house by her grandmother. Mrs Clemons was not happy with its scale or authenticity and so began a rebuilding project which resulted in a four storey grand house, now known as Pendle Hall. Although some items were purchased during shopping trips to Melbourne, the majority of furnishings were constructed by Felicity's own hands, using numerous reference books and magazines.";
            var itemHtmlHelper = new HtmlHelpers<ItemViewTransformerResult>(null, null, new ItemViewTransformerResult());

            //When
            var result = itemHtmlHelper.ConvertNewlines(objectSummary);

            // Then
            result.ShouldBeOfType(typeof(NonEncodedHtmlString));
        }

        [Fact]
        public void Raw_ReturnsNonEncodedHtmlString()
        {
            // Given 
            var objectSummary = "Candlestick from Room 21 - Chinese Bedroom, Main Building Section, Pendle Hall. From top of dressing table.\n\nIt was created by Felicity Clemons as part of a 40 project to create a grand fully-furnished Georgian style country dolls' house. The project began in the 1940's when Felicity's daughter, Antonia, was given a three-room dolls' house by her grandmother. Mrs Clemons was not happy with its scale or authenticity and so began a rebuilding project which resulted in a four storey grand house, now known as Pendle Hall. Although some items were purchased during shopping trips to Melbourne, the majority of furnishings were constructed by Felicity's own hands, using numerous reference books and magazines.";
            var itemHtmlHelper = new HtmlHelpers<ItemViewTransformerResult>(null, null, new ItemViewTransformerResult());

            //When
            var result = itemHtmlHelper.Raw(objectSummary);

            // Then
            result.ShouldBeOfType(typeof(NonEncodedHtmlString));
        }

    }
}