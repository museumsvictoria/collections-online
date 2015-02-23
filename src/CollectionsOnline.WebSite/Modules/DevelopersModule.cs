using System.Linq;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Utilities;
using CollectionsOnline.WebSite.Metadata;
using Nancy;
using Nancy.Routing;

namespace CollectionsOnline.WebSite.Modules
{
    public class DevelopersModule : NancyModule
    {
        public DevelopersModule(IRouteCacheProvider routeCacheProvider)
            : base(Constants.CurrentApiVersionPathSegment)
        {
            Get["/developers"] = parameters =>
            {
                var apiOperationMetadatas = routeCacheProvider
                    .GetCache()
                    .GroupBy(x => Inflector.Pluralize(x.Key.Name.Replace("ApiModule", string.Empty)), x => x.Value.Select(y => y.Item2.Metadata.Retrieve<ApiMetadata>()).Where(y => y != null))
                    .ToDictionary(x => x.Key, x => x.SelectMany(y => y).Where(y => y != null).GroupBy(y => y.Path).Select(y => y.First()))
                    .Where(x => x.Value.Any());

                return View["developers", apiOperationMetadatas];
            };
        }
    }
}