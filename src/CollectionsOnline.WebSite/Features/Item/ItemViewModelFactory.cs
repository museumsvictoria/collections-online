using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.WebSite.Features.Search;
using Nancy;
using Raven.Abstractions.Data;

namespace CollectionsOnline.WebSite.Features.Item
{
    public class ItemViewModelFactory : IItemViewModelFactory
    {
        public ItemViewModel MakeViewModel()
        {
            var itemViewModel = new ItemViewModel();

            return itemViewModel;
        }
    }
}