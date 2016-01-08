using Nancy;

namespace CollectionsOnline.WebSite.Queries
{
    public interface IMediaResponseQuery
    {
        Response BuildMediaResponse(string documentId, long mediaId, string size);
    }
}