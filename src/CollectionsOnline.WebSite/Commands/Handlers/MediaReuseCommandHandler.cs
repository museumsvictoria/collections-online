using CollectionsOnline.Core.Models;
using Raven.Client;

namespace CollectionsOnline.WebSite.Commands.Handlers
{

    public class MediaReuseCommandHandler : IMediaReuseCommandHandler
    {
        private readonly IDocumentSession _documentSession;

        public MediaReuseCommandHandler(
            IDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }

        public void Handle(MediaReuseCommand mediaReuseCommand)
        {
            var mediaReuse = new MediaReuse
            {
                DocumentId = mediaReuseCommand.DocumentId,
                MediaId = mediaReuseCommand.MediaId,
                Usage = mediaReuseCommand.Usage,
                UsageMore = mediaReuseCommand.UsageMore
            };

            _documentSession.Store(mediaReuse);
            _documentSession.SaveChanges();
        }
    }
}