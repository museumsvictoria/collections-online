using System.Linq;
using CollectionsOnline.Core.Models;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace CollectionsOnline.Core.Indexes
{
    public class LocalityTermIndex : AbstractMultiMapIndexCreationTask<LocalityTermIndexResult>
    {
        public LocalityTermIndex()
        {
            AddMap<Item>(items =>
                from item in items
                where item.IsHidden == false
                from locality in item.IndigenousCulturesLocalities.Where(x => !string.IsNullOrWhiteSpace(x))
                .Concat(item.Associations.Where(x => !string.IsNullOrWhiteSpace(x.Locality)).Select(x => x.Locality))
                .Concat(item.Associations.Where(x => !string.IsNullOrWhiteSpace(x.Region)).Select(x => x.Region))
                .Concat(item.Associations.Where(x => !string.IsNullOrWhiteSpace(x.State)).Select(x => x.State))
                .Concat(item.Associations.Where(x => !string.IsNullOrWhiteSpace(x.Country)).Select(x => x.Country))
                select new
                {
                    Locality = locality
                });

            AddMap<Species>(speciesDocs =>
                from species in speciesDocs
                where species.IsHidden == false
                from locality in species.NationalParks
                select new
                {
                    Locality = locality
                });

            AddMap<Specimen>(specimens =>
                from specimen in specimens
                where specimen.IsHidden == false
                from locality in new string[]
                {
                    specimen.Ocean,
                    specimen.Continent,
                    specimen.Country,
                    specimen.State,
                    specimen.District,
                    specimen.Town,
                    specimen.NearestNamedPlace,
                    specimen.TektitesLocalStrewnfield,
                    specimen.TektitesGlobalStrewnfield
                }.Where(x => !string.IsNullOrWhiteSpace(x))
                .Concat(specimen.Associations.Where(x => !string.IsNullOrWhiteSpace(x.Locality)).Select(x => x.Locality))
                .Concat(specimen.Associations.Where(x => !string.IsNullOrWhiteSpace(x.Region)).Select(x => x.Region))
                .Concat(specimen.Associations.Where(x => !string.IsNullOrWhiteSpace(x.State)).Select(x => x.State))
                .Concat(specimen.Associations.Where(x => !string.IsNullOrWhiteSpace(x.Country)).Select(x => x.Country))
                select new
                {
                    Locality = locality
                });

            Reduce = results =>
                from result in results
                group result by result.Locality
                into g
                select new
                {
                    Locality = g.Key
                };

            Analyzers.Add(x => x.Locality, "Lucene.Net.Analysis.Standard.StandardAnalyzer");

            Index(x => x.Locality, FieldIndexing.Analyzed);
            Store(x => x.Locality, FieldStorage.Yes);

            // Increase maximum map output per document
            MaxIndexOutputsPerDocument = 256;
        }
    }

    public class LocalityTermIndexResult
    {
        public string Locality { get; set; }
    }
}
