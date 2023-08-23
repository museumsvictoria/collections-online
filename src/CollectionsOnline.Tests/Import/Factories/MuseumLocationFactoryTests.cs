using CollectionsOnline.Core.Models;
using CollectionsOnline.Import.Factories;
using IMu;
using Shouldly;
using Xunit;

namespace CollectionsOnline.Tests.Import.Factories
{
    public class MuseumLocationFactoryTests
    {
        [Fact]
        public void MakeMuseumLocation_NotMatchingExcludedLocation_ReturnsCorrectLocation()
        {
            // Given
            var museumLocationFactory = new MuseumLocationFactory();
            var locationMap = new Map
            {
                {"LocLevel1", "MELBOURNE (MvCIS)"},
                {"LocLevel2", "GROUND LEVEL"},
                {"LocLevel3", "GALLERY 8"},
                {"LocLevel4", null },
                {"LocLocationType", "Location"},
            };

            // When
            var result = museumLocationFactory.Make(locationMap);

            // Then
            result.Gallery.ShouldBe("Bunjilaka - First Peoples");
            result.Venue.ShouldBe("Melbourne Museum");
        }

        [Fact]
        public void MakeMuseumLocation_NotMatchingExcludedLocationWithValidLocLevel4_ReturnsCorrectLocation()
        {
            // Given
            var museumLocationFactory = new MuseumLocationFactory();
            var locationMap = new Map
            {
                {"LocLevel1", "MELBOURNE (MvCIS)"},
                {"LocLevel2", "GROUND LEVEL"},
                {"LocLevel3", "GALLERY 8"},
                {"LocLevel4", "GRID E1" },
                {"LocLocationType", "Location"},
            };

            // When
            var result = museumLocationFactory.Make(locationMap);

            // Then
            result.Gallery.ShouldBe("Bunjilaka - First Peoples");
            result.Venue.ShouldBe("Melbourne Museum");
        }
    }
}
