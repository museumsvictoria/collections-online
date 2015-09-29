using System.Configuration;
using System.Linq;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Utilities;
using CollectionsOnline.WebSite.Models;
using CollectionsOnline.WebSite.Models.Api;
using Nancy;
using Nancy.Routing;

namespace CollectionsOnline.WebSite.Factories
{
    public class DevelopersViewModelFactory : IDevelopersViewModelFactory
    {
        public DevelopersIndexViewModel MakeDevelopersIndex(IRouteCache routeCache, Request request)
        {
            return new DevelopersIndexViewModel
            {
                OperationMetadata = routeCache.GroupBy(
                    x => Inflector.Pluralize(x.Key.Name.Replace("ApiModule", string.Empty)),
                    x => x.Value.Select(y => y.Item2.Metadata.Retrieve<ApiMetadata>()).Where(y => y != null))
                    .Select(x => new ApiOperationMetadata
                    {
                        Name = x.Key,
                        Metadata = x.SelectMany(y => y).Where(y => y != null).GroupBy(y => y.Path).Select(y => y.First())
                    })
                    .Where(x => x.Metadata.Any()),
                ApiRootUrl = string.Format("{0}/{1}", ConfigurationManager.AppSettings["CanonicalSiteBase"], Constants.ApiPathBase),
                ApiCurrentVersionRootUrl = string.Format("{0}/{1}{2}", ConfigurationManager.AppSettings["CanonicalSiteBase"], Constants.ApiPathBase, Constants.CurrentApiVersionPath),
            };
        }
    }
}