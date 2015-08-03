using System.Configuration;
using Nancy;
using Nancy.Responses;

namespace CollectionsOnline.RedirectWebSite.Modules
{
    public class RedirectModule : NancyModule
    {
        public RedirectModule()
        {
            var baseWebsiteUrl = ConfigurationManager.AppSettings["WebsiteUrl"];

            Get["/"] = parameters => new RedirectResponse(baseWebsiteUrl, RedirectResponse.RedirectType.Permanent);

            Get["/search"] = parameters => new RedirectResponse(string.Format("{0}search", baseWebsiteUrl), RedirectResponse.RedirectType.Permanent);

            Get["items/{id:int}/{name*}"] = parameters => new RedirectResponse(string.Format("{0}items/{1}", baseWebsiteUrl, parameters.id), RedirectResponse.RedirectType.Permanent);

            Get["themes/{id:int}/{name*}"] = parameters => new RedirectResponse(string.Format("{0}articles/{1}", baseWebsiteUrl, parameters.id), RedirectResponse.RedirectType.Permanent);

            Get["/{everythingelse*}"] = parameters => new RedirectResponse(baseWebsiteUrl, RedirectResponse.RedirectType.Permanent);
        }
    }
}