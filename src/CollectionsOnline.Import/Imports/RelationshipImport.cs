using System.Collections.Generic;
using System.Linq;
using CollectionsOnline.Core.Config;
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
            _log.Debug("Starting {0} import", GetType().Name);

            // Perform item relationship import
            IList<string> cachedItemResult;
            using (var documentSession = _documentStore.OpenSession())
            {
                cachedItemResult = documentSession.Load<Application>(Constants.ApplicationId).GetImportStatus(typeof(ImuImport<Item>).ToString()).CachedResult.Select(x => string.Format("items/{0}", x)).ToList();
            }

            var currentOffset = 0;
            // Collection Name
            while (true)
            {
                using (var documentSession = _documentStore.OpenSession())
                {
                    if (ImportCanceled())
                        return;

                    var items = documentSession.Load<Item>(cachedItemResult
                        .Skip(currentOffset)
                        .Take(Constants.DataBatchSize))
                        .ToList();

                    if(items.Count == 0)
                        break;
                    
                    foreach (var item in items.Where(x => x != null))
                    {
                        item.RelatedArticleIds.AddRangeUnique(documentSession
                            .Query<Article>()
                            .Where(x => x.Title.In(item.CollectionNames))
                            .ToList()
                            .Select(x => x.Id));
                    }

                    currentOffset += items.Count;
                    documentSession.SaveChanges();
                }
            }

            _log.Debug("{0} import complete", GetType().Name);
        }

        public int Order
        {
            get { return 20; }
        }

        private bool ImportCanceled()
        {
            return Program.ImportCanceled;
        }
    }
}