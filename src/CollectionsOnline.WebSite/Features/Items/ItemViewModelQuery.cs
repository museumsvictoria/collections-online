using CollectionsOnline.Core.Models;
using Raven.Client;

namespace CollectionsOnline.WebSite.Features.Items
{
    public class ItemViewModelQuery : IItemViewModelQuery
    {
        private readonly IDocumentSession _documentSession;

        public ItemViewModelQuery(
            IDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }

        public ItemViewTransformerResult BuildItem(string itemId)
        {
            return _documentSession.Load<ItemViewTransformer, ItemViewTransformerResult>(itemId);
        }
    }
}