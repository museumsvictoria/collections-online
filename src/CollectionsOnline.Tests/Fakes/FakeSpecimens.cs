using System;
using System.Collections.Generic;
using CollectionsOnline.Core.Models;
using FizzWare.NBuilder;
using FizzWare.NBuilder.Generators;

namespace CollectionsOnline.Tests.Fakes
{
    public class FakeSpecimens
    {
        public static Specimen CreateFakeSpecimen(string id = null)
        {
            if (id == null)
            {
                id = "specimens/" + GetRandom.Int(1, 1500000);
            }

            return Builder<Specimen>
                .CreateNew()
                .With(x => x.Id = id)
                .With(x => x.DateModified = GetRandom.DateTime(DateTime.Now.AddYears(-2), DateTime.Now.AddDays(-1)))
                .Build();
        }

        public static IList<Specimen> CreateFakeSpecimens(int count = 50)
        {
            var fakeSpecimens = new List<Specimen>();

            for (int i = 1; i < count+1; i++)
            {
                fakeSpecimens.Add(CreateFakeSpecimen("specimens/" + i));
            }

            return fakeSpecimens;
        }
    }
}
