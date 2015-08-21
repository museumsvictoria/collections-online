using Nancy;
using Nancy.Responses;

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
        }
    }
}