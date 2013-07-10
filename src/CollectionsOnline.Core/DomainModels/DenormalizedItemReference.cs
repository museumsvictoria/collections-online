using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CollectionsOnline.Core.DesignByContract;

namespace CollectionsOnline.Core.DomainModels
{
    public class DenormalizedItemReference
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public static implicit operator DenormalizedItemReference(Item item)
        {
            Requires.IsNotNull(item, "item");

            return new DenormalizedItemReference
            {
                Id = item.Id,
                Name = item.Name
            };
        }
    }
}
