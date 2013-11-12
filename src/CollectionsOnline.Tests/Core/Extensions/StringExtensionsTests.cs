using CollectionsOnline.Core.Extensions;
using Shouldly;
using Xunit;

namespace CollectionsOnline.Tests.Core.Extensions
{
    public class StringExtensionsTests
    {
        [Fact]
        public void Concatenate_ReturnsConcatenatedString()
        {
            // Given When
            var result = new[]
            {
                "Arthropoda",
                "Malacostraca",
                "Tanaidacea",
                "Metapseudidae"
            }.Concatenate(" ");
            
            // Then
            result.ShouldBe("Arthropoda Malacostraca Tanaidacea Metapseudidae");
        }

        [Fact]
        public void GivenEmptyStrings_Concatenate_ReturnsNull()
        {
            // Given When
            var result = new[]
            {
                "   ",
                "",
                "   "
            }.Concatenate(" ");

            // Then
            result.ShouldBe(null);
        }
    }
}
