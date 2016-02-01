using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Core.Utilities;
using Nancy;
using Nancy.Responses;
using Raven.Client;

namespace CollectionsOnline.WebSite.Queries
{
    public class MediaResponseQuery : IMediaResponseQuery
    {
        private readonly IDocumentSession _documentSession;

        public MediaResponseQuery(
            IDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }

        public Response BuildMediaResponse(string documentId, long mediaId, string size)
        {
            var document = _documentSession.Load<dynamic>(documentId);

            if (document == null || document.IsHidden)
                return HttpStatusCode.NotFound;

            // Only return ImageMedia at this stage
            IList<Media> documentMedia = document.Media;
            var media = documentMedia.FirstOrDefault(x => x.Irn == mediaId) as ImageMedia;

            if (media == null)
                return HttpStatusCode.NotFound;

            ImageMediaFile imageMediaFile = null;
            switch (size)
            {
                case "small":
                    imageMediaFile = media.Medium;
                    break;
                case "medium":
                    imageMediaFile = media.Large;
                    break;
                case "large":
                    imageMediaFile = media.Original;
                    break;
            }

            if (imageMediaFile == null)
                return HttpStatusCode.NotFound;

            var filePath = Path.GetFullPath(string.Format("{0}\\{1}", AppDomain.CurrentDomain.BaseDirectory, imageMediaFile.Uri));
            var response = new StreamResponse(() => new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read), "image/jpeg");

            if (!string.IsNullOrWhiteSpace(media.Caption))
            {
                response.WithHeader("Content-Disposition", string.Format("attachment; filename={0}-{1}-{2}.jpg",
                    HtmlConverter.HtmlToText(media.Caption)
                    .ToLower()
                    .Truncate(Constants.FileMaxChars)
                    .RemoveLineBreaks()
                    .ReplaceNonWordWithDashes(),
                    media.Irn,
                    size));
            }
            else
            {
                response.WithHeader("Content-Disposition",
                    string.Format("attachment; filename={0}-{1}.jpg", media.Irn, size));
            }

            return response;
        }
    }
}