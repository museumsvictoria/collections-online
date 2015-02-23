using CollectionsOnline.WebSite.Queries;
using Nancy;

namespace CollectionsOnline.WebSite.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule(IHomeViewModelQuery homeViewModelQuery)            
        {
            Get["home", "/"] = parameters =>
            {
                return View["home", homeViewModelQuery.BuildHome()];
            };
        }
    }
}