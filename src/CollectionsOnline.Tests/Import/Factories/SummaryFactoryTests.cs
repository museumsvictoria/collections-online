using System.Collections.Generic;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Import.Builders;
using CollectionsOnline.Import.Factories;
using Shouldly;
using Xunit;

namespace CollectionsOnline.Tests.Import.Factories
{
    public class SummaryFactoryTests
    {
        [Fact]
        public void MakeItem_FirstPeoplesItem_CorrectlyCreatesSummary()
        {
            // Given
            var summaryFactory = new SummaryFactory();
            var item = MakeFirstPeoplesItem();
            
            // When
            var result = summaryFactory.Make(item);

            // Then
            result.ShouldBe("This work is part of a collection formed over a four year period by Baillieu Myer and Carrillo Gantner with support from Neilma Gantner and guidance from Jennifer Isaacs. It was first shown at the California Palace of the Legion ...");
        }
        
        private Item MakeFirstPeoplesItem()
        {
            return new Item
            {
                Category = "First Peoples",
                ObjectSummary = "This work is part of a collection formed over a four year period by Baillieu Myer and Carrillo Gantner with support from Neilma Gantner and guidance from Jennifer Isaacs. It was first shown at the California Palace of the Legion of Honour in San Francisco in 1999 to commemorate the arrival in Australia of Sidney (Baevski) Myer, the found of the Myer Emporium, and to celebrate the trans-Pacific links between the Myer and Gantner families. The collection of contemporary Australian Aboriginal art was made to celebrate and share the distinctive creativity of Australia's Indigenous people.",
                CollectingAreas = new List<string>
                {
                    "Australian Indigenous - Northern Australia and Queensland and Torres Strait Islands",
                    "Australian Indigenous Identity and Contemporary Life"
                }
            };
        }
    }
}
