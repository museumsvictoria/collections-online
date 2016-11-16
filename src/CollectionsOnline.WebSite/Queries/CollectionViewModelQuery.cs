using System.Linq;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Models;
using CollectionsOnline.WebSite.Transformers;
using Raven.Client;

namespace CollectionsOnline.WebSite.Queries
{
    public class CollectionViewModelQuery : ICollectionViewModelQuery
    {
        private readonly IDocumentSession _documentSession;

        public CollectionViewModelQuery(
            IDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }

        public CollectionIndexViewModel BuildCollectionIndex()
        {
            using (_documentSession.Advanced.DocumentStore.AggressivelyCacheFor(Constants.AggressiveCacheTimeSpan))
            {
                var collections = _documentSession
                    .Query<Collection>()
                    .ToList();

                foreach (var collection in collections.Where(x => !string.IsNullOrWhiteSpace(x.Summary)))
                {
                    collection.Summary = collection.Summary.Truncate(Constants.CollectionSummaryMaxChars);
                }

                // Todo : move linq query to an index
                var collectionIndexViewModel = new CollectionIndexViewModel
                {
                    Collections = collections
                        .Where(x => x.Media.Any() && x.IsHidden == false)
                        .GroupBy(x => x.Category)
                        .OrderBy(x => x.Key)
                };
                
                return collectionIndexViewModel;
            }
        }

        public CollectionViewTransformerResult BuildCollection(string collectionId)
        {
            return _documentSession.Load<CollectionViewTransformer, CollectionViewTransformerResult>(collectionId);
        }
    }
}