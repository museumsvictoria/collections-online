using CollectionsOnline.Import.Factories;
using IMu;
using Shouldly;
using Xunit;

namespace CollectionsOnline.Tests.Import.Factories
{
    public class CollectionSiteFactoryTests
    {
        [Fact]
        public void MakeCollectionSite_ForMeteoriteModel_ReturnsNull()
        {
            // Given
            var collectionSiteFactory = new CollectionSiteFactory(new PartiesNameFactory());
            var map = MakeSampleCollectionSiteFromMeteoriteModel();

            // When
            var result = collectionSiteFactory.Make(map, "Model (Natural Sciences)", "Meteorites", "Geology");

            // Then
            result.ShouldBe(null);
        }
        
        [Fact]
        public void MakeCollectionSite_ForMeteorite_Specimen_ReturnsResult()
        {
            // Given
            var collectionSiteFactory = new CollectionSiteFactory(new PartiesNameFactory());
            var map = MakeSampleCollectionSiteFromMeteoriteSpecimen();

            // When
            var result = collectionSiteFactory.Make(map, "Specimen", "Meteorites", "Geology");

            // Then
            result.ShouldNotBe(null);
        }

        private Map MakeSampleCollectionSiteFromMeteoriteModel()
        {
            var map = new Map
            {
                {
                    "EraMvRockUnit_tab", new object[]
                    {
                    }
                },
                { "EraAge1", null },
                { "EraAge2", null },
                { "EraMvStage", null },
                { "irn", 80L },
                { "SitSiteCode", null },
                {
                    "geo", new object[]
                    {
                        new Map
                        {
                            { "LocTownship_tab", null },
                            { "LocDistrictCountyShire_tab", null },
                            { "LocOcean_tab", null },
                            { "LocNearestNamedPlace_tab", null },
                            { "LocProvinceStateTerritory_tab", "Victoria" },
                            { "LocContinent_tab", null },
                            { "LocCountry_tab", "Australia" }
                        }
                    }
                },
                { "AdmPublishWebNoPassword", "Yes" },
                { "LocElevationASLFromMt", null },
                { "SitSiteNumber", null },
                {
                    "EraMvGroup_tab", new object[]
                    {
                    }
                },
                { "EraEra", null },
                {
                    "EraLithology_tab", new object[]
                    {
                    }
                },
                {
                    "latlong", new object[]
                    {
                    }
                },
                { "LocElevationASLToMt", null },
                { "LocPreciseLocation", "Gippsland" },
                { "EraMvMember_tab", new object[] { } }
            };

            return map;
        }
        
        private Map MakeSampleCollectionSiteFromMeteoriteSpecimen()
        {
            var map = new Map
            {
                {
                    "EraMvGroup_tab", new object[]
                    {
                    }
                },
                { "SitSiteNumber", null },
                {
                    "latlong", new object[]
                    {
                    }
                },
                {
                    "EraLithology_tab", new object[]
                    {
                    }
                },
                { "EraEra", null },
                { "LocElevationASLFromMt", null },
                { "LocElevationASLToMt", null },
                {
                    "EraMvMember_tab", new object[]
                    {
                    }
                },
                { "LocPreciseLocation", "Cranbourne" },
                { "EraMvStage", null },
                { "irn", 3L },
                {
                    "EraMvRockUnit_tab", new object[]
                    {
                    }
                },
                { "EraAge1", null },
                { "EraAge2", null },
                {
                    "geo", new object[]
                    {
                        new Map
                        {
                            { "LocNearestNamedPlace_tab", "Cranbourne" },
                            { "LocOcean_tab", null },
                            { "LocDistrictCountyShire_tab", null },
                            { "LocTownship_tab", null },
                            { "LocCountry_tab", "Australia" },
                            { "LocProvinceStateTerritory_tab", "Victoria" },
                            { "LocContinent_tab", null }
                        }
                    }
                },
                { "SitSiteCode", null },
                { "AdmPublishWebNoPassword", "Yes" }
            };

            return map;
        }
    }
}