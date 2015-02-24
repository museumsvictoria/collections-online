using System;
using System.Collections.Generic;
using System.Linq;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Models;
using FizzWare.NBuilder;

namespace CollectionsOnline.WebSite.Metadata
{
    public class SampleItems
    {
        public static ItemApiViewModel Create(string id = "items/1")
        {
            return Builder<ItemApiViewModel>
                .CreateNew()
                .With(x => x.Id = id)
                .With(x => x.Comments = Builder<CommentApiViewModel>
                    .CreateListOfSize(1)
                    .Build())
                .With(x => x.DateModified = new DateTime(2015, 1, 1))
                .With(x => x.Associations = Builder<Association>
                    .CreateListOfSize(1)
                    .Build())
                .Build();
        }

        public static IEnumerable<ItemApiViewModel> CreateList(int count = 1)
        {
            var sampleItems = new List<ItemApiViewModel>();

            for (int i = 1; i < count+1; i++)
            {
                sampleItems.Add(Create("items/" + i));
            }

            return sampleItems;
        }
    }
}
