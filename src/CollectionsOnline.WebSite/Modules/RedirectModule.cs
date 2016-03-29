using System;
using Nancy;
using Nancy.Responses;
using CollectionsOnline.Core.Extensions;

namespace CollectionsOnline.WebSite.Modules
{
    public class RedirectModule : NancyModule
    {
        public RedirectModule()
        {
            Get["/search.php"] = parameters => new RedirectResponse("/search", RedirectResponse.RedirectType.Permanent);

            Get["/browser.php"] = parameters => new RedirectResponse("/search", RedirectResponse.RedirectType.Permanent);            

            Get["/results.php"] = parameters => new RedirectResponse("/search", RedirectResponse.RedirectType.Permanent);

            Get["/resultsns.php"] = parameters => new RedirectResponse("/search", RedirectResponse.RedirectType.Permanent);

            Get["/results-browse.php"] = parameters => new RedirectResponse("/search", RedirectResponse.RedirectType.Permanent);

            Get["/object.php"] = parameters =>
            {
                var irn = Request.Query["irn"];

                if(string.IsNullOrWhiteSpace(irn))
                    return new RedirectResponse("/", RedirectResponse.RedirectType.Permanent);

                return new RedirectResponse(string.Format("/specimens/{0}", irn), RedirectResponse.RedirectType.Permanent);
            };

            Get["/imagedisplay.php"] = parameters => new RedirectResponse("/", RedirectResponse.RedirectType.Permanent);

            Get["/webmedia.php"] = parameters => new RedirectResponse("/", RedirectResponse.RedirectType.Permanent);

            #region temp redirects
            // Handle canonical site redirect via episerver i.e. http://museumvictoria.com.au/collections => http://collections.museumvictoria.com.au
            // All remaining redirects handled below temporarily until reverse proxy fixed.

            // Phar lap
            Get["/items/1499868/{name?}"] = parameters => new RedirectResponse("/specimens/139139", RedirectResponse.RedirectType.Permanent);
            // Sam the koala
            Get["/items/1550331/{name?}"] = parameters => new RedirectResponse("/specimens/1487596", RedirectResponse.RedirectType.Permanent);

            Get["/items/{id:int}/{name}"] = parameters => new RedirectResponse(string.Format("/items/{0}", parameters.id), RedirectResponse.RedirectType.Permanent);

            Get["/themes/{id:int}/{name?}"] = parameters => new RedirectResponse(string.Format("/articles/{0}", parameters.id), RedirectResponse.RedirectType.Permanent);

            //Images
            Get["/itemimages/{id1:int}/{id2:int}/{filename}"] = parameters =>
            {
                int imageIrn = int.Parse(string.Format("{0}{1}", parameters.id1, parameters.id2));

                var originalFilename = parameters.filename.Value as string;

                var location = string.Empty;

                if (originalFilename.Contains("large.jpg", StringComparison.OrdinalIgnoreCase))
                    location = string.Format("/content/media/{0}/{1}-large.jpg", imageIrn % 50, imageIrn);
                else if (originalFilename.Contains("medium.jpg", StringComparison.OrdinalIgnoreCase))
                    location = string.Format("/content/media/{0}/{1}-medium.jpg", imageIrn % 50, imageIrn);
                else if (originalFilename.Contains("small.jpg", StringComparison.OrdinalIgnoreCase))
                    location = string.Format("/content/media/{0}/{1}-small.jpg", imageIrn % 50, imageIrn);
                else if (originalFilename.Contains("thumb", StringComparison.OrdinalIgnoreCase))
                    location = string.Format("/content/media/{0}/{1}-thumbnail.jpg", imageIrn % 50, imageIrn);

                return new RedirectResponse(location, RedirectResponse.RedirectType.Permanent);
            };

            #endregion
        }
    }
}