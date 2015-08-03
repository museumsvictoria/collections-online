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

            Get["/object.php"] = parameters => new RedirectResponse("/", RedirectResponse.RedirectType.Permanent);

            Get["/imagedisplay.php"] = parameters => new RedirectResponse("/", RedirectResponse.RedirectType.Permanent);
        }
    }
}