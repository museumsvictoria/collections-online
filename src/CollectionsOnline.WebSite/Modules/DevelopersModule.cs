using CollectionsOnline.Core.Config;
using CollectionsOnline.WebSite.Factories;
using Nancy;
using Nancy.Routing;

namespace CollectionsOnline.WebSite.Modules
{
    public class DevelopersModule : NancyModule
    {
        public DevelopersModule(
            IRouteCacheProvider routeCacheProvider,
            IDevelopersViewModelFactory developersViewModelFactory)
            : base(Constants.CurrentApiVersionPathSegment)
        {
            Get["/developers"] = parameters =>
            {
                return View["developers", developersViewModelFactory.MakeViewModel(routeCacheProvider.GetCache(), Request)];
            };
        }
    }
}