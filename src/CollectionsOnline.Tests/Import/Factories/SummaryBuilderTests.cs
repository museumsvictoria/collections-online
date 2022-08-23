using CollectionsOnline.Import.Builders;
using Shouldly;
using Xunit;

namespace CollectionsOnline.Tests.Import.Factories
{
    public class SummaryBuilderTests
    {
        [Fact]
        public void AddText_GivenShortHtmlString_RemainsTheSame()
        {
            // Given
            var summaryBuilder = new SummaryBuilder();
            summaryBuilder.AddText(
                "The young caterpillars of the <em>Imperial Hairstreak</em> are tended by ants which protect them from predators and parasites.", true);
            
            // When
            var result = summaryBuilder.ToString();

            // Then
            result.ShouldBe("The young caterpillars of the <em>Imperial Hairstreak</em> are tended by ants which protect them from predators and parasites.");
        }
        
        [Fact]
        public void AddText_GivenLongHtmlString_HandlesHtmlTruncation()
        {
            // Given
            var summaryBuilder = new SummaryBuilder();
            summaryBuilder.AddText(
                "The young caterpillars of the Imperial Hairstreak are tended by ants which protect them from predators and parasites. In return, the ants collect sweet secretions from the caterpillars. The caterpillars feed on young (<em>Acacia mearnsii</em>) wattle trees, particularly Black Wattle, Silver Wattle (<em>A. dealbata</em>) and Blackwood (<em>A. malanoxylon</em>). The caterpillars turn into pupae (cocoons) on a communal web and the butterflies emerge from November to April.",
                true);
            
            // When
            var result = summaryBuilder.ToString();

            // Then
            result.ShouldBe("The young caterpillars of the Imperial Hairstreak are tended by ants which protect them from predators and parasites. In return, the ants collect sweet secretions from the caterpillars. The caterpillars feed on young (<em>Acacia ...</em>");
        }
        
        [Fact]
        public void AddText_GivenLongString_HandlesTruncation()
        {
            // Given
            var summaryBuilder = new SummaryBuilder();
            summaryBuilder.AddText(
                "The young caterpillars of the Imperial Hairstreak are tended by ants which protect them from predators and parasites. In return, the ants collect sweet secretions from the caterpillars. The caterpillars feed on young wattle trees, particularly Black Wattle, Silver Wattle");
            
            // When
            var result = summaryBuilder.ToString();

            // Then
            result.ShouldBe("The young caterpillars of the Imperial Hairstreak are tended by ants which protect them from predators and parasites. In return, the ants collect sweet secretions from the caterpillars. The caterpillars feed on young wattle trees, ...");
        }
        
        [Fact]
        public void AddText_GivenLongString_HandlesHtmlTruncation()
        {
            // Given
            var summaryBuilder = new SummaryBuilder();
            summaryBuilder.AddText(
                "The young caterpillars of the Imperial Hairstreak are tended by ants which protect them from predators and parasites. In return, the ants collect sweet secretions from the caterpillars. The caterpillars feed on young wattle trees, particularly Black Wattle, Silver Wattle",
                true);
            
            // When
            var result = summaryBuilder.ToString();

            // Then
            result.ShouldBe("The young caterpillars of the Imperial Hairstreak are tended by ants which protect them from predators and parasites. In return, the ants collect sweet secretions from the caterpillars. The caterpillars feed on young wattle trees, ...");
        }
        
        [Fact]
        public void AddText_GivenSeparatorOnBoundary_PreservesSeparator()
        {
            // Given
            var summaryBuilder = new SummaryBuilder();
            summaryBuilder.AddText(
                "<em>Eurythenes</em> are known throughout the world's oceans. Inhabiting depths between 175 and 8000 m, species are thought to have diverged as a result of depth rather than geographical locations. It has been observed that <em>Eurythenes</em> show ontogenetic stratification, meaning that juveniles will live and feed in shallower waters than mature individuals. At lower depths, it is also possible they burrow into the seafloor to avoid possible dangers. As a scavenger of the deep sea, <em>Eurythenes</em>is one of the forerunning animal groups being used to understand connectivity within the deep sea.", 
                true);
            
            // When
            var result = summaryBuilder.ToString();

            // Then
            result.ShouldBe("<em>Eurythenes</em> are known throughout the world's oceans. Inhabiting depths between 175 and 8000 m, species are thought to have diverged as a result of depth rather than geographical locations. It has been observed that <em>Eurythenes</em> show ...");
        }
    }
}
