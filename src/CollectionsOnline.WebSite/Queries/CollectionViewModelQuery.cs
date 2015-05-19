using System.Linq;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Models;
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
                var collectionIndexViewModel = new CollectionIndexViewModel
                {
                    Collections = _documentSession
                        .Query<Collection>()                        
                        .Take(50)
                        .ToList()
                };

                collectionIndexViewModel.Collections = collectionIndexViewModel.Collections
                    .Where(x => x.Media.Any())
                    .ToList();

                foreach (var collection in collectionIndexViewModel.Collections)
                {
                    collection.Summary = collection.Summary.Truncate(Constants.SummaryMaxChars);
                }

                return collectionIndexViewModel;
            }
        }

        public Collection BuildCollection(string collectionId)
        {
            throw new System.NotImplementedException();
        }
    }
}