using System.Configuration;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Ninject.Activation;
using Serilog;

namespace CollectionsOnline.Import.Infrastructure
{
    public class YoutubeServiceProvider : Provider<YouTubeService>
    {
        protected override YouTubeService CreateInstance(IContext context)
        {
            // Create youtube service
            var youtubeService = new YouTubeService(new BaseClientService.Initializer
            {
                ApiKey = ConfigurationManager.AppSettings["GoogleApiKey"],
                ApplicationName = GetType().ToString()
            });

            return youtubeService;
        }
    }
}