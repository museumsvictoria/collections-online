using System;
using System.Collections.Generic;
using CollectionsOnline.Core.Models;
using FizzWare.NBuilder;
using FizzWare.NBuilder.Generators;

namespace CollectionsOnline.Tests
{
    public class FakeStories
    {
        public static Story CreateFakeStory(string id = null)
        {
            if (id == null)
            {
                id = "stories/" + GetRandom.Int(1, 1500000);
            }

            return Builder<Story>
                .CreateNew()
                .With(x => x.Id = id)
                .With(x => x.DateModified = GetRandom.DateTime(DateTime.Now.AddYears(-2), DateTime.Now.AddDays(-1)))
                .Build();
        }

        public static IList<Story> CreateFakeStories(int count = 50)
        {
            var fakeSpecimens = new List<Story>();

            for (int i = 1; i < count+1; i++)
            {
                fakeSpecimens.Add(CreateFakeStory("stories/" + i));
            }

            return fakeSpecimens;
        }
    }
}
