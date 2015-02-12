using Nancy;

namespace CollectionsOnline.WebSite.Features.Home
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