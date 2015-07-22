using System;
using System.Linq;
using CollectionsOnline.Core.Models;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace CollectionsOnline.Core.Indexes
{
    public class MediaByIrnWithChecksum : AbstractMultiMapIndexCreationTask<MediaByIrnWithChecksumResult>
    {
        public MediaByIrnWithChecksum()
        {
            AddMap<Article>(articles =>
                from article in articles
                where article.IsHidden == false
                from media in article.Media.Concat(article.Authors.Where(x => x != null).Select(x => x.ProfileImage))
                where media != null
                select new
                {
                    Irn = media.Irn,
                    Md5Checksum = ((IHasChecksum) media).Md5Checksum,
                    DateModified = article.DateModified
                });

            AddMap<Item>(items =>
                from item in items
                where item.IsHidden == false
                from media in item.Media
                select new
                {
                    Irn = media.Irn,
                    Md5Checksum = ((IHasChecksum)media).Md5Checksum,
                    DateModified = item.DateModified
                });

            AddMap<Species>(speciesDocs =>
                from species in speciesDocs
                where species.IsHidden == false
                from media in species.Media.Concat(species.Authors.Where(x => x != null).Select(x => x.ProfileImage))
                select new
                {
                    Irn = media.Irn,
                    Md5Checksum = ((IHasChecksum)media).Md5Checksum,
                    DateModified = species.DateModified
                });

            AddMap<Specimen>(specimens =>
                from specimen in specimens
                where specimen.IsHidden == false
                from media in specimen.Media
                select new
                {
                    Irn = media.Irn,
                    Md5Checksum = ((IHasChecksum)media).Md5Checksum,
                    DateModified = specimen.DateModified
                });

            Reduce = results => from result in results
                group result by new {result.Irn, result.Md5Checksum}
                into g
                select new
                {
                    g.Key.Irn,
                    g.Key.Md5Checksum,
                    DateModified = g.Max(x => x.DateModified)
                };

            Index(x => x.Irn, FieldIndexing.NotAnalyzed);
            Index(x => x.Md5Checksum, FieldIndexing.No);
            Index(x => x.DateModified, FieldIndexing.No);

            Store(x => x.Md5Checksum, FieldStorage.Yes);
            Store(x => x.Irn, FieldStorage.Yes);
            
            Sort(x => x.DateModified, SortOptions.String);

            MaxIndexOutputsPerDocument = 1024;
        }
    }

    public class MediaByIrnWithChecksumResult
    {
        public long Irn { get; set; }

        public string Md5Checksum { get; set; }

        public DateTime DateModified { get; set; }
    }
}