using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Factories;
using CollectionsOnline.WebSite.Queries;
using Nancy;
using Raven.Client;

namespace CollectionsOnline.WebSite.Modules
{
    public class CollectionsModule : NancyModule
    {
        public CollectionsModule(
            ICollectionViewModelQuery collectionViewModelQuery,
            IDocumentSession documentSession,
            IMetadataViewModelFactory metadataViewModelFactory)
        {
            Get["collection-index", "/collections"] = parameters =>
            {
                ViewBag.metadata = metadataViewModelFactory.MakeCollectionIndex();

                return View["CollectionIndex", collectionViewModelQuery.BuildCollectionIndex()];
            };

            Get["/collections/{id}"] = parameters =>
            {
                var collection = documentSession.Load<Collection>("collections/" + parameters.id as string);

                if (collection == null || collection.IsHidden) 
                    return HttpStatusCode.NotFound;

                ViewBag.metadata = metadataViewModelFactory.Make(collection);

                return View["Collections", collectionViewModelQuery.BuildCollection("collections/" + parameters.id)];
            };
        }
    }
}