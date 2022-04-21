using CollectionsOnline.Import.Factories;
using IMu;
using Shouldly;
using Xunit;

namespace CollectionsOnline.Tests.Import.Factories
{
    public class CollectionEventFactoryTests
    {
        [Fact]
        public void MakeCollectionEvent_WithZiObservation_ReturnsNullCollectedBy()
        {
            // Given
            var collectionEventFactory = new CollectionEventFactory(new PartiesNameFactory());
            var map = MakeSampleCollectionEvent();

            // When
            var result = collectionEventFactory.Make(map, "Observation", "ZI", "Invertebrate Zoology");

            // Then
            result.CollectedBy.ShouldBe(null);
        }
        
        [Fact]
        public void MakeCollectionEvent_WithTObservation_ReturnsNonEmptyCollectedBy()
        {
            // Given
            var collectionEventFactory = new CollectionEventFactory(new PartiesNameFactory());
            var map = MakeSampleCollectionEvent();

            // When
            var result = collectionEventFactory.Make(map, "Observation", "T", "Invertebrate Zoology");

            // Then
            result.CollectedBy.ShouldBe("Dr Ken Walker - Museum Victoria");
        }
        
        [Fact]
        public void MakeCollectionEvent_WithZISpecimen_ReturnsNonEmptyCollectedBy()
        {
            // Given
            var collectionEventFactory = new CollectionEventFactory(new PartiesNameFactory());
            var map = MakeSampleCollectionEvent();

            // When
            var result = collectionEventFactory.Make(map, "Specimen", "ZI", "Invertebrate Zoology");

            // Then
            result.CollectedBy.ShouldBe("Dr Ken Walker - Museum Victoria");
        }
        
        [Fact]
        public void MakeCollectionEvent_WithSpecimenTypeModel_ReturnsNull()
        {
            // Given
            var collectionEventFactory = new CollectionEventFactory(new PartiesNameFactory());
            var map = MakeSampleCollectionEvent();

            // When
            var result = collectionEventFactory.Make(map, "Model (Natural Sciences)", "ZI", "Invertebrate Zoology");

            // Then
            result.ShouldBe(null);
        }

        private Map MakeSampleCollectionEvent()
        {
            var map = new Map
            {
                { "irn", 211806L },
                { "ColDateVisitedTo", "21/11/2013" },
                {
                    "collectors", new object[]
                    {
                        new Map
                        {
                            { "NamDepartment", "Collections, Research & Exhibitions" },
                            { "NamFullName", "Dr Ken Walker" },
                            { "NamPartyType", "Person" },
                            { "AddPhysState", null },
                            { "NamSource", null },
                            { "NamBranch", null },
                            {
                                "NamOrganisationOtherNames_tab", new object[]
                                {
                                }
                            },
                            { "AddPhysCountry", null },
                            { "AddPhysCity", null },
                            { "ColCollaborationName", null },
                            { "AddPhysStreet", null },
                            { "NamOrganisation", "Museum Victoria" }
                        }
                    }
                },
                { "ExpExpeditionName", "Alpine National Park Bioscan 2013 (Alps BioScan)" },
                { "ColDateVisitedFrom", "21/11/2013" },
                { "ColCollectionEventCode", "ALB 2013 0710" },
                { "AquDepthFromMet", null },
                { "ColCollectionMethod", "Direct Search" },
                { "AquDepthToMet", null },
                { "ColTimeVisitedTo", "15:47" },
                { "ColTimeVisitedFrom", "14:17" }
            };

            return map;
        }
    }
}