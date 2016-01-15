using CollectionsOnline.Core.Models;
using Shouldly;
using Xunit;

namespace CollectionsOnline.Tests.Core.Models
{
    public class MediaModelTests
    {
        [Fact]
        public void Media_WithNullLicenceInfo_ReturnsFalse()
        {
            // Given
            var media = new ImageMedia();

            // When
            var result = media.PermissionRequired;

            // Then
            result.ShouldBe(false);
        }

        [Fact]
        public void Media_WithRestrictedLicenceInfo_ReturnsTrue()
        {
            // Given
            var media = new ImageMedia
            {
                RightsStatus = "In Copyright: Third Party Copyright",
                Licence = "Restricted (All Rights Reserved)",
            };

            // When
            var result = media.PermissionRequired;

            // Then
            result.ShouldBe(true);
        }

        [Fact]
        public void Media_WithOpenLicenceInfo_ReturnsFalse()
        {
            // Given
            var media = new ImageMedia
            {
                RightsStatus = "Copyright Expired (Public Domain)",
                Licence = "No Known Restrictions",
            };

            // When
            var result = media.PermissionRequired;

            // Then
            result.ShouldBe(true);
        }
    }
}