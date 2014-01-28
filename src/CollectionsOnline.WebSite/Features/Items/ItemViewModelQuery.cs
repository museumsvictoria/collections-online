using CollectionsOnline.Core.Models;
using Raven.Client;

namespace CollectionsOnline.WebSite.Features.Items
{
    public class ItemViewModelQuery : IItemViewModelQuery
    {
        private readonly IDocumentSession _documentSession;
        private readonly IItemViewModelFactory _itemViewModelFactory;

        public ItemViewModelQuery(
            IDocumentSession documentSession,
            IItemViewModelFactory itemViewModelFactory)
        {
            _documentSession = documentSession;
            _itemViewModelFactory = itemViewModelFactory;
        }

        public ItemViewModel BuildItem(string itemId)
        {
            var item = _documentSession
                .Load<Item>(itemId);

            return _itemViewModelFactory.MakeViewModel(item);
        }
    }
}