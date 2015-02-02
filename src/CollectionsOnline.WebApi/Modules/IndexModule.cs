using System.Linq;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Utilities;
using CollectionsOnline.WebApi.Metadata;
using Nancy;
using Nancy.Routing;

namespace CollectionsOnline.WebApi.Modules
{
    public class IndexModule : NancyModule
    {
        public IndexModule(IRouteCacheProvider routeCacheProvider)
            : base(Constants.CurrentWebApiVersionPathSegment)
        {
            Get["/"] = parameters =>
            {
                var webApiOperationMetadatas = routeCacheProvider
                    .GetCache()
                    .GroupBy(x => Inflector.Pluralize(x.Key.Name.Replace("Module", string.Empty)), x => x.Value.Select(y => y.Item2.Metadata.Retrieve<WebApiMetadata>()).Where(y => y != null))
                    .ToDictionary(x => x.Key, x => x.SelectMany(y => y).Where(y => y != null).GroupBy(y => y.Path).Select(y => y.First()))
                    .Where(x => x.Value.Any());

                return View["index", webApiOperationMetadatas];
            };
        }
    }
}