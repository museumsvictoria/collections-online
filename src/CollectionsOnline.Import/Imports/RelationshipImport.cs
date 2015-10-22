using System.Collections.Generic;
using System.Linq;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Core.Extensions;
using Raven.Client;
using Raven.Client.Linq;
using Serilog;

namespace CollectionsOnline.Import.Imports
{
    public class RelationshipImport : IImport
    {
        private readonly IDocumentStore _documentStore;

        public RelationshipImport(
            IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public void Run()
        {
            using (Log.Logger.BeginTimedOperation("Relationship Import starting", "RelationshipImport.Run"))
            {
                ImportCache importCache;
                List<string> importCacheItemIds;

                // Perform item relationship import
                using (var documentSession = _documentStore.OpenSession())
                {
                    // Get the id's of all items that were cached in the current import
                    importCache = documentSession.Load<ImportCache>("importCaches/item") ?? new ImportCache();
                    importCacheItemIds = importCache.Irns.Select(x => string.Format("items/{0}", x)).ToList();
                }
            
                var currentOffset = 0;
                // Collection Name
                while (true)
                {
                    using (var documentSession = _documentStore.OpenSession())
                    {
                        if (ImportCanceled())
                            return;

                        var items = documentSession.Load<Item>(importCacheItemIds
                            .Skip(currentOffset)
                            .Take(Constants.DataBatchSize))
                            .ToList();

                        if (items.Count == 0)
                            break;

                        var foundRelatedArticleCount = 0;
                        foreach (var item in items.Where(x => x != null))
                        {
                            using (var relatedDocumentSession = _documentStore.OpenSession())
                            {
                                var relatedArticleIds = relatedDocumentSession
                                    .Query<object, CombinedIndex>()
                                    .Where(x => ((CombinedIndexResult) x).DisplayTitle.In(item.CollectionNames) && ((CombinedIndexResult) x).RecordType == "article")
                                    .Select(x => ((CombinedIndexResult) x).Id)
                                    .ToList();

                                var originalArticleCount = item.RelatedArticleIds.Count;

                                item.RelatedArticleIds.AddRangeUnique(relatedArticleIds);

                                foundRelatedArticleCount += item.RelatedArticleIds.Count - originalArticleCount;
                            }
                        }

                        currentOffset += items.Count;
                        documentSession.SaveChanges();
                        documentSession.Dispose();

                        if (foundRelatedArticleCount > 0)
                            Log.Logger.Debug("Found {RelatedArticleCount} related articles via collection name", foundRelatedArticleCount);
                        Log.Logger.Information("Relationship import progress... {Offset}/{TotalResults}", currentOffset, importCacheItemIds.Count);
                    }
                }
            }
        }

        public int Order
        {
            get { return 200; }
        }

        private bool ImportCanceled()
        {
            return Program.ImportCanceled;
        }
    }
}