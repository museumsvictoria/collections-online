using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Import.Utilities;
using Shouldly;
using Xunit;

namespace CollectionsOnline.Tests.Import.Utilities
{
    public class NaturalDateConverterTests
    {
        [Fact]
        public void Concatenate_ReturnsConcatenatedString()
        {
            // Given
            string result;
            var date = "circa 1872-1900";
            
            // When
            result = NaturalDateConverter.ConvertToYearSpan(date);
            
            // Then
            result.ShouldBe("1880 - 1890");
        }
    }
}
