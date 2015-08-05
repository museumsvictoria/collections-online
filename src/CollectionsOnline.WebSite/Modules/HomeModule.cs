using CollectionsOnline.WebSite.Models;
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
                ViewBag.metadata = new MetadataViewModel
                {
                    Description = "Museum Victoria Collections Description goes here",
                    Title = "Museum Victoria Collections",
                    CanonicalUri = "http://collections.museumvictoria.com.au/"
                };

                return View["HomeIndex", homeViewModelQuery.BuildHomeIndex()];
            };

            Get["home-closewindow", "/closewindow"] = parameters =>
            {
                return View["CloseWindow"];
            };
        }
    }
}