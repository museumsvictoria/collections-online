using System;
using System.Collections.Generic;
using CollectionsOnline.Core.Models;
using FizzWare.NBuilder;
using FizzWare.NBuilder.Generators;

namespace CollectionsOnline.Tests.Fakes
{
    public class FakeItems
    {
        private static readonly string[] Types =
            {
                "Audiovisual",
                "Document",
                "Image",
                "Manuscript",
                "Model (Natural Sciences)",
                "Object",
                "Specimen"
            };

        private static readonly string[] Disciplines =
            {
                "Archaeology - Historical",
                "History",
                "Numismatics",
                "Philately",
                "Technology",
                "Trade Literature"
            };

        public static Item CreateFakeItem(string id = null)
        {
            if (id == null)
            {
                id = "items/" + GetRandom.Int(1, 1500000);
            }

            return Builder<Item>
                .CreateNew()
                .With(x => x.Id = id)
                .With(x => x.DateModified = GetRandom.DateTime(DateTime.UtcNow.AddYears(-2), DateTime.UtcNow.AddDays(-1)))
                .With(x => x.Category = "History & Technology")
                .With(x => x.Type = Types[GetRandom.Int(0, Types.Length - 1)])
                .With(x => x.Discipline = Disciplines[GetRandom.Int(0, Disciplines.Length - 1)])
                .Build();
        }

        public static IList<Item> CreateFakeItems(int count = 50)
        {
            var fakeItems = new List<Item>();

            for (int i = 1; i < count+1; i++)
            {
                fakeItems.Add(CreateFakeItem("items/" + i));
            }

            return fakeItems;
        }
    }
}
