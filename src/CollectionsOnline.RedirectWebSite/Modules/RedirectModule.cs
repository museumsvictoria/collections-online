using System.Configuration;
using Nancy;
using Nancy.Responses;

namespace CollectionsOnline.RedirectWebSite.Modules
{
    public class RedirectModule : NancyModule
    {
        public RedirectModule()
        {
            Get["/"] = parameters => new RedirectResponse(ConfigurationManager.AppSettings["WebsiteUrl"], RedirectResponse.RedirectType.Permanent);
        }
    }
}