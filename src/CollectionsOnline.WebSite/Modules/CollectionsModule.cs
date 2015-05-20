using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Queries;
using Nancy;
using Raven.Client;

namespace CollectionsOnline.WebSite.Modules
{
    public class CollectionsModule : NancyModule
    {
        public CollectionsModule(
            ICollectionViewModelQuery collectionViewModelQuery,
            IDocumentSession documentSession)
        {
            Get["collection-index", "/collections"] = parameters =>
            {
                return View["CollectionIndex", collectionViewModelQuery.BuildCollectionIndex()];
            };

            Get["/collections/{id}"] = parameters =>
            {
                var collection = documentSession.Load<Collection>("collections/" + parameters.id as string);

                return (collection == null || collection.IsHidden) ? HttpStatusCode.NotFound : View["Collections", collectionViewModelQuery.BuildCollection("collections/" + parameters.id)];
            };
        }
    }
}