using System.Linq;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Utilities;
using CollectionsOnline.WebSite.Metadata;
using CollectionsOnline.WebSite.Models;
using Nancy;
using Nancy.Routing;

namespace CollectionsOnline.WebSite.Factories
{
    public class DevelopersViewModelFactory : IDevelopersViewModelFactory
    {
        public DevelopersViewModel MakeViewModel(IRouteCache routeCache, Request request)
        {
            var developersViewModel = new DevelopersViewModel
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
                ApiRootUrl = string.Format("{0}{1}", request.Url.SiteBase, Constants.ApiBasePath),
                ApiCurrentVersionRootUrl = string.Format("{0}{1}{2}", request.Url.SiteBase, Constants.ApiBasePath, Constants.CurrentApiVersionPath),
                PagingPageSizeMax = Constants.PagingPageSizeMax.ToString()
            };

            return developersViewModel;
        }
    }
}