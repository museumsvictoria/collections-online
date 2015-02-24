using CollectionsOnline.WebSite.Models;
using Nancy;
using Nancy.Routing;

namespace CollectionsOnline.WebSite.Factories
{
    public interface IDevelopersViewModelFactory
    {
        DevelopersViewModel MakeViewModel(IRouteCache routeCache, Request request);
    }
}