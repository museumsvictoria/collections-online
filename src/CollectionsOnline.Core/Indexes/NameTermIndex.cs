using System.Linq;
using CollectionsOnline.Core.Models;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace CollectionsOnline.Core.Indexes
{
    public class NameTermIndex : AbstractMultiMapIndexCreationTask<NameTermIndexResult>
    {
        public NameTermIndex()
        {
            AddMap<Item>(items =>
                from item in items
                where item.IsHidden == false
                from name in new string[]
                {
                    item.IndigenousCulturesPhotographer,
                    item.IndigenousCulturesAuthor,
                    item.IndigenousCulturesIllustrator,
                    item.IndigenousCulturesMaker,
                    item.IndigenousCulturesCollector,
                    item.IndigenousCulturesLetterTo,
                    item.IndigenousCulturesLetterFrom,
                    item.ArcheologyManufactureName,
                    item.TradeLiteraturePrimaryName,
                    item.ArtworkPublisher
                }.Where(x => !string.IsNullOrWhiteSpace(x))
                .Concat(item.Brands.Select(x => x.Name))
                .Concat(item.Associations.Where(x => !string.IsNullOrWhiteSpace(x.Name)).Select(x => x.Name))
                select new
                {
                    Name = name
                });

            AddMap<Specimen>(specimens =>
                from specimen in specimens
                where specimen.IsHidden == false
                from name in specimen.Associations.Where(x => !string.IsNullOrWhiteSpace(x.Name)).Select(x => x.Name)
                select new
                {
                    Name = name
                });

            Reduce = results =>
                from result in results
                group result by result.Name
                into g
                select new
                {
                    Name = g.Key
                };

            Analyzers.Add(x => x.Name, "Lucene.Net.Analysis.Standard.StandardAnalyzer");

            Index(x => x.Name, FieldIndexing.Analyzed);
            Store(x => x.Name, FieldStorage.Yes);

            // Increase maximum map output per document
            MaxIndexOutputsPerDocument = 256;
        }
    }

    public class NameTermIndexResult
    {
        public string Name { get; set; }
    }
}
