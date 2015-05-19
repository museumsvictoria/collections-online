using CollectionsOnline.WebSite.Models;

namespace CollectionsOnline.WebSite.Queries
{
    public interface IHomeViewModelQuery
    {
        HomeIndexViewModel BuildHomeIndex();
    }
}