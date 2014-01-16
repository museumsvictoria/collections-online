using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.WebSite.Features.Search;
using Nancy;
using Raven.Abstractions.Data;
using Raven.Client;
using Constants = CollectionsOnline.Core.Config.Constants;

namespace CollectionsOnline.WebSite.Features.Item
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

        public ItemViewModel BuildItem()
        {
            return _itemViewModelFactory.MakeViewModel();
        }
    }
}