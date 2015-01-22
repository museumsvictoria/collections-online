using System.Linq;
using CollectionsOnline.WebApi.Metadata;
using Nancy;
using Nancy.Routing;

namespace CollectionsOnline.WebApi.Modules
{
    public class IndexModule : NancyModule
    {
        public IndexModule(IRouteCacheProvider routeCacheProvider)
        {
            Get["/"] = parameters =>
            {
                return View["index", routeCacheProvider.GetCache().RetrieveMetadata<WebapiMetadata>().Where(x => x != null).GroupBy(x => x.Path).Select(x => x.First())];
            };
        }
    }
}