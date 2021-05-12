using System.Configuration;
using System.Linq;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Shouldly;
using Xunit;

namespace CollectionsOnline.Tests.Import.Factories
{
    public class VideoMediaFactoryTests
    {
        [Fact]
        public void YouTubeService_Returns_CorrectVideoUrl()
        {
            // Given
            var result = string.Empty;
            var youtubeService = new YouTubeService(new BaseClientService.Initializer
            {
                ApiKey = ConfigurationManager.AppSettings["GoogleApiKey"],
                ApplicationName = GetType().ToString()
            });
            var request = youtubeService.Videos.List("snippet");
            request.Id = "d83nSFEoz2A";

            // When
            var listResponse = request.Execute();
            var youtubeVideo = listResponse.Items.FirstOrDefault();
            if (youtubeVideo != null)
            {
                var thumbnail = youtubeVideo.Snippet.Thumbnails.Maxres ?? youtubeVideo.Snippet.Thumbnails.High;

                result = thumbnail.Url;
            }

            // Then
            result.ShouldBe("https://i.ytimg.com/vi/d83nSFEoz2A/maxresdefault.jpg");
        }
    }
}