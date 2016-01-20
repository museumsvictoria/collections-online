using System.Collections.Generic;
using System.Linq;
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
            var document = _documentSession.Load<dynamic>(mediaReuseCommand.DocumentId);
            IList<Media> documentMedia = document.Media;

            var media = documentMedia
                .Single(x => x.Irn == mediaReuseCommand.MediaId);

            var mediaReuse = new MediaReuse
            {
                DocumentId = mediaReuseCommand.DocumentId,
                Media = media,
                Usage = mediaReuseCommand.Usage,
                UsageMore = mediaReuseCommand.UsageMore
            };

            _documentSession.Store(mediaReuse);
            _documentSession.SaveChanges();
        }
    }
}