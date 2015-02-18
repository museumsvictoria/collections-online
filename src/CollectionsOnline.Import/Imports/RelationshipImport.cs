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
            _log.Debug("Starting relationship import");

            // Perform item relationship import
            var documentSession = _documentStore.OpenSession();
            
            // Get the id's of all items that were cached in the current import
            var importCache = documentSession.Load<ImportCache>("importCaches/item") ?? new ImportCache();
            var importCacheItemIds = importCache.Irns.Select(x => string.Format("items/{0}", x)).ToList();

            var currentOffset = 0;
            // Collection Name
            while (true)
            {
                documentSession = _documentStore.OpenSession();
                if (ImportCanceled())
                    return;

                var items = documentSession.Load<Item>(importCacheItemIds
                    .Skip(currentOffset)
                    .Take(Constants.DataBatchSize))
                    .ToList();

                if(items.Count == 0)
                    break;
                    
                foreach (var item in items.Where(x => x != null))
                {
                    item.RelatedArticleIds.AddRangeUnique(documentSession
                        .Query<object, Combined>()
                        .Where(x => ((CombinedResult)x).DisplayTitle.In(item.CollectionNames) && ((CombinedResult)x).Type == "article")
                        .ToList()
                        .Select(x => ((CombinedResult)x).Id));
                }

                currentOffset += items.Count;
                documentSession.SaveChanges();
                documentSession.Dispose();

                _log.Debug("relationship import progress... {0}/{1}", currentOffset, importCacheItemIds.Count);
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