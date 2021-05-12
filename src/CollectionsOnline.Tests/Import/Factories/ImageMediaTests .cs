using System.IO;
using CollectionsOnline.Tests.Resources;
using ImageMagick;
using Shouldly;
using Xunit;

namespace CollectionsOnline.Tests.Import.Factories
{
    public class ImageMediaTests : FileBasedTest
    {
        [Fact]
        public void ImageMagick_Correctly_SavesThumbnailImage()
        {
            // Given
            var outputFileName = $"{Files.OutputFolder}{Path.GetFileNameWithoutExtension(Files.Fish)}.jpg";

            using (var image = new MagickImage())
            {
                image.Read(Files.Fish);

                var profile = image.GetColorProfile();
                image.Strip();
                if (profile != null)
                    image.SetProfile(profile);

                image.Format = MagickFormat.Jpg;
                image.Quality = 90;
                image.FilterType = FilterType.Lanczos;
                image.ColorSpace = ColorSpace.sRGB;
                image.Resize(new MagickGeometry(250) {FillArea = true});
                image.Crop(250, 250, Gravity.Center);
                image.UnsharpMask(0.5, 0.5, 0.6, 0.025);

                // When
                image.Write(outputFileName);
            }

            // Then
            File.Exists(outputFileName).ShouldBe(true);
        }
    }
}