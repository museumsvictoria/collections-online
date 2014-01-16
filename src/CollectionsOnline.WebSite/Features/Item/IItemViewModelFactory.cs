using System.Collections.Generic;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.WebSite.Features.Search;
using Nancy;
using Raven.Abstractions.Data;

namespace CollectionsOnline.WebSite.Features.Item
{
    public interface IItemViewModelFactory
    {
        ItemViewModel MakeViewModel();
    }
}