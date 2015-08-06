using CollectionsOnline.WebSite.Factories;
using CollectionsOnline.WebSite.Queries;
using Nancy;

namespace CollectionsOnline.WebSite.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule(
            IHomeViewModelQuery homeViewModelQuery,
            IMetadataViewModelFactory metadataViewModelFactory)
        {
            Get["home-index", "/"] = parameters =>
            {
                ViewBag.metadata = metadataViewModelFactory.MakeHomeIndex();

                return View["HomeIndex", homeViewModelQuery.BuildHomeIndex()];
            };

            Get["home-closewindow", "/closewindow"] = parameters =>
            {
                return View["CloseWindow"];
            };
        }
    }
}