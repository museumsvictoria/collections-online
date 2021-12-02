using CollectionsOnline.Core.Models;
using CollectionsOnline.Import.Factories;
using CollectionsOnline.Tests.Fakes;
using Shouldly;
using Xunit;

namespace CollectionsOnline.Tests.Import.Factories
{
    public class MuseumLocationFactoryTests
    {
        [Fact]
        public void MakeMuseumLocation_MatchingOnDisplayConceptualParent_ReturnsCorrectLocation()
        {
            // Given
            var museumLocationFactory = new MuseumLocationFactory(new PartiesNameFactory());
            var partsMaps = FakeMaps.CreateValidPartsMap();
            var objectStatusMaps = FakeMaps.CreateEmptyMapArray();
            
            // When
            var result = museumLocationFactory.Make("Conceptual", objectStatusMaps, partsMaps);
            
            // Then
            result.DisplayStatus.ShouldBe(DisplayStatus.OnDisplay);
            result.DisplayLocation.ShouldBe(DisplayLocation.Scienceworks);
        }
        
        [Fact]
        public void MakeMuseumLocation_MatchingOnDisplayObjectStatus_ReturnsCorrectLocation()
        {
            // Given
            var museumLocationFactory = new MuseumLocationFactory(new PartiesNameFactory());
            var partsMaps = FakeMaps.CreateEmptyMapArray();
            var objectStatusMaps = FakeMaps.CreateValidObjectStatusMap();

            // When
            var result = museumLocationFactory.Make("Physical", objectStatusMaps, partsMaps);
            
            // Then
            result.DisplayStatus.ShouldBe(DisplayStatus.OnDisplay);
            result.DisplayLocation.ShouldBe(DisplayLocation.MelbourneMuseum);
        }
        
        [Fact]
        public void MakeMuseumLocation_MatchingIncorrectObjectStatus_ReturnsOnLoan()
        {
            // Given
            var museumLocationFactory = new MuseumLocationFactory(new PartiesNameFactory());
            var partsMaps = FakeMaps.CreateEmptyMapArray();
            var objectStatusMaps = FakeMaps.CreateIncorrectObjectStatusMap();

            // When
            var result = museumLocationFactory.Make(null, objectStatusMaps, partsMaps);
            
            // Then
            result.DisplayStatus.ShouldBe(DisplayStatus.OnLoan);
        }
    }
}
