using System.Linq;
using CollectionsOnline.Core.Models;
using Raven.Client.Indexes;

namespace CollectionsOnline.Core.Indexes
{
    public class ItemsNotHidden : AbstractIndexCreationTask<Item>
    {
        public ItemsNotHidden()
        {
            Map = items =>
                from item in items
                where item.IsHidden == false
                select new {};
        }
    }
}
