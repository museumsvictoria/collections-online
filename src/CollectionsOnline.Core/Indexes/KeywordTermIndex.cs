using System.Linq;
using CollectionsOnline.Core.Models;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace CollectionsOnline.Core.Indexes
{
    public class KeywordTermIndex : AbstractMultiMapIndexCreationTask<KeywordTermIndexResult>
    {
        public KeywordTermIndex()
        {
            AddMap<Article>(articles =>
                from article in articles
                where article.IsHidden == false
                from keyword in article.Keywords
                select new
                {
                    Keyword = keyword
                });

            AddMap<Item>(items =>
                from item in items
                where item.IsHidden == false
                from keyword in new string[]
                {
                    item.AudioVisualRecordingDetails,
                    item.ArcheologyActivity,
                    item.ArcheologySpecificActivity,
                    item.ArcheologyDecoration,
                    item.NumismaticsSeries,
                    item.TradeLiteraturePrimarySubject,
                    item.TradeLiteraturePrimaryRole
                }.Where(x => !string.IsNullOrWhiteSpace(x))
                .Concat(item.Keywords.Where(x => !string.IsNullOrWhiteSpace(x)))
                .Concat(item.ModelNames.Where(x => !string.IsNullOrWhiteSpace(x)))
                .Concat(item.TradeLiteraturePublicationTypes.Where(x => !string.IsNullOrWhiteSpace(x)))
                select new
                {
                    Keyword = keyword
                });

            AddMap<Species>(speciesDocs =>
                from species in speciesDocs
                where species.IsHidden == false
                from keyword in new string[]
                {
                    species.AnimalSubType
                }.Where(x => !string.IsNullOrWhiteSpace(x))
                .Concat(species.ConservationStatuses.Where(x => !string.IsNullOrWhiteSpace(x)))
                select new
                {
                    Keyword = keyword
                });

            AddMap<Specimen>(specimens =>
                from specimen in specimens
                where specimen.IsHidden == false
                from keyword in new string[]
                {
                    specimen.ExpeditionName
                }.Where(x => !string.IsNullOrWhiteSpace(x))
                .Concat(specimen.Keywords.Where(x => !string.IsNullOrWhiteSpace(x)))
                select new
                {
                    Keyword = keyword
                });

            Reduce = results =>
                from result in results
                group result by result.Keyword
                into g
                select new
                {
                    Keyword = g.Key
                };

            Analyzers.Add(x => x.Keyword, "Lucene.Net.Analysis.Standard.StandardAnalyzer");

            Index(x => x.Keyword, FieldIndexing.Analyzed);
            Store(x => x.Keyword, FieldStorage.Yes);

            // Increase maximum map output per document
            MaxIndexOutputsPerDocument = 256;
        }
    }

    public class KeywordTermIndexResult
    {
        public string Keyword { get; set; }
    }
}
