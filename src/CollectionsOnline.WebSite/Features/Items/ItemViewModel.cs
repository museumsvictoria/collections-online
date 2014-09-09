using System.Collections.Generic;
using CollectionsOnline.Core.Models;

namespace CollectionsOnline.WebSite.Features.Items
{
    public class ItemViewModel
    {
        public Item Item { get; set; }

        public IList<ImageMedia> ImageMedia { get; set; }
    }
}