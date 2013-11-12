using System;
using System.Collections.Generic;
using CollectionsOnline.Core.Models;
using FizzWare.NBuilder;
using FizzWare.NBuilder.Generators;

namespace CollectionsOnline.Tests.Fakes
{
    public class FakeSpecies
    {
        public static Species CreateFakeSpecies(string id = null)
        {
            if (id == null)
            {
                id = "species/" + GetRandom.Int(1, 1500000);
            }

            return Builder<Species>
                .CreateNew()
                .With(x => x.Id = id)
                .With(x => x.DateModified = GetRandom.DateTime(DateTime.Now.AddYears(-2), DateTime.Now.AddDays(-1)))
                .Build();
        }

        public static IList<Species> CreateFakeSpecies(int count = 50)
        {
            var fakeSpecies = new List<Species>();

            for (int i = 1; i < count+1; i++)
            {
                fakeSpecies.Add(CreateFakeSpecies("species/" + i));
            }

            return fakeSpecies;
        }
    }
}
