using CollectionsOnline.WebSite.Queries;
using Nancy;

namespace CollectionsOnline.WebSite.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule(IHomeViewModelQuery homeViewModelQuery)            
        {
            Get["home-index", "/"] = parameters =>
            {
                return View["HomeIndex", homeViewModelQuery.BuildHomeIndex()];
            };
        }
    }
}