using System.Collections.Generic;
using System.Linq;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Core.Extensions;
using NLog;
using Raven.Client;
using Raven.Client.Linq;

namespace CollectionsOnline.Import.Imports
{
    public class RelationshipImport : IImport
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly IDocumentStore _documentStore;

        public RelationshipImport(
            IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public void Run()
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

            _log.Debug("Starting {0} import", GetType().Name);

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

                    if(foundRelatedArticleCount > 0)
                        _log.Debug("found {0} related articles via collection name", foundRelatedArticleCount);
                    _log.Debug("relationship import progress... {0}/{1}", currentOffset, importCacheItemIds.Count);
                }
            }

            _log.Debug("relationship import complete");
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