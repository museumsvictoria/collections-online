using System;
using System.Collections.Generic;
using CollectionsOnline.Core.Models;
using FizzWare.NBuilder;
using FizzWare.NBuilder.Generators;

namespace CollectionsOnline.Tests.Fakes
{
    public class FakeArticles
    {
        public static Article CreateFakeArticle(string id = null)
        {
            if (id == null)
            {
                id = "articles/" + GetRandom.Int(1, 1500000);
            }

            return Builder<Article>
                .CreateNew()
                .With(x => x.Id = id)
                .With(x => x.DateModified = GetRandom.DateTime(DateTime.UtcNow.AddYears(-2), DateTime.UtcNow.AddDays(-1)))
                .Build();
        }

        public static IList<Article> CreateFakeArticles(int count = 50)
        {
            var fakeSpecimens = new List<Article>();

            for (int i = 1; i < count+1; i++)
            {
                fakeSpecimens.Add(CreateFakeArticle("articles/" + i));
            }

            return fakeSpecimens;
        }
    }
}