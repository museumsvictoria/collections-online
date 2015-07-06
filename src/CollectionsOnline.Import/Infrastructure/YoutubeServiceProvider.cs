using System.Configuration;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Ninject.Activation;
using NLog;

namespace CollectionsOnline.Import.Infrastructure
{
    public class YoutubeServiceProvider : Provider<YouTubeService>
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        protected override YouTubeService CreateInstance(IContext context)
        {
            // Create youtube service
            _log.Debug("Initializing youtube service");
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = ConfigurationManager.AppSettings["GoogleApiKey"],
                ApplicationName = GetType().ToString()
            });

            return youtubeService;
        }
    }
}