using System;
using System.Configuration;
using CollectionsOnline.RedirectWebSite.Extensions;
using Nancy;
using Nancy.Responses;

namespace CollectionsOnline.RedirectWebSite.Modules
{
    public class RedirectModule : NancyModule
    {
        public RedirectModule()
        {
            var canonicalSiteBase = ConfigurationManager.AppSettings["CanonicalSiteBase"];

            Get["/"] = parameters => new RedirectResponse(canonicalSiteBase, RedirectResponse.RedirectType.Permanent);

            Get["/search"] = parameters => new RedirectResponse(string.Format("{0}/search", canonicalSiteBase), RedirectResponse.RedirectType.Permanent);

            // Phar lap
            Get["/items/1499868/{name?}"] = parameters => new RedirectResponse(string.Format("{0}/specimens/139139", canonicalSiteBase), RedirectResponse.RedirectType.Permanent);
            // Sam the koala
            Get["/items/1550331/{name?}"] = parameters => new RedirectResponse(string.Format("{0}/specimens/1487596", canonicalSiteBase), RedirectResponse.RedirectType.Permanent);

            Get["/items/{id:int}/{name?}"] = parameters => new RedirectResponse(string.Format("{0}/items/{1}", canonicalSiteBase, parameters.id), RedirectResponse.RedirectType.Permanent);

            Get["/themes/{id:int}/{name?}"] = parameters => new RedirectResponse(string.Format("{0}/articles/{1}", canonicalSiteBase, parameters.id), RedirectResponse.RedirectType.Permanent);

            //Images
            Get["/itemimages/{id1:int}/{id2:int}/{filename}"] = parameters =>
            {
                int imageIrn = int.Parse(string.Format("{0}{1}", parameters.id1, parameters.id2));

                var originalFilename = parameters.filename.Value as string;

                string location = canonicalSiteBase;

                if (originalFilename.Contains("large.jpg", StringComparison.OrdinalIgnoreCase))
                    location = string.Format("{0}/content/media/{1}/{2}-large.jpg", canonicalSiteBase, imageIrn % 50, imageIrn);
                else if (originalFilename.Contains("medium.jpg", StringComparison.OrdinalIgnoreCase))
                    location = string.Format("{0}/content/media/{1}/{2}-medium.jpg", canonicalSiteBase, imageIrn % 50, imageIrn);
                else if (originalFilename.Contains("small.jpg", StringComparison.OrdinalIgnoreCase))
                    location = string.Format("{0}/content/media/{1}/{2}-small.jpg", canonicalSiteBase, imageIrn % 50, imageIrn);
                else if (originalFilename.Contains("thumb", StringComparison.OrdinalIgnoreCase))
                    location = string.Format("{0}/content/media/{1}/{2}-thumbnail.jpg", canonicalSiteBase, imageIrn % 50, imageIrn);

                return new RedirectResponse(location, RedirectResponse.RedirectType.Permanent);
            };

            Get["/{everythingelse*}"] = parameters => new RedirectResponse(canonicalSiteBase, RedirectResponse.RedirectType.Permanent);
        }
    }
}