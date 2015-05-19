using CollectionsOnline.WebSite.Models;
using Nancy;
using Nancy.Routing;

namespace CollectionsOnline.WebSite.Factories
{
    public interface IDevelopersViewModelFactory
    {
        DevelopersIndexViewModel MakeDevelopersIndex(IRouteCache routeCache, Request request);
    }
}