using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Models;
using Dapper;
using NLog;
using Raven.Client;

namespace CollectionsOnline.Import.Importers
{
    public class ItemMigration : IItemMigration
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly IDocumentStore _documentStore;

        public ItemMigration(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public void Run(DateTime dateLastRun)
        {
            if (dateLastRun == default(DateTime))
            {
                using (var connection = new SqlConnection(ConfigurationManager.AppSettings["SqlImportConnectionString"]))
                {
                    _log.Debug("Beginning Item migration");

                    const string commentsSqlQuery = @"SELECT Comment.*, ItemComment.ItemId
                    FROM Comment INNER JOIN
                        ItemComment ON Comment.CommentId = ItemComment.CommentId
                    UNION
                    SELECT Comment.*, ItemCommentArchive.ItemId
                    FROM Comment INNER JOIN
                         ItemCommentArchive ON Comment.CommentId = ItemCommentArchive.CommentId";

                    var results = connection.Query(commentsSqlQuery).ToList();

                    var count = 0;

                    while (true)
                    {
                        using (var documentSession = _documentStore.OpenSession())
                        {
                            if (Program.ImportCanceled)
                            {
                                _log.Debug("Canceling Item migration");
                                return;
                            }

                            var comments = results
                                .Skip(count)
                                .Take(Constants.DataBatchSize)
                                .ToList();

                            if (comments.Count == 0)
                                break;

                            var existingItems = documentSession.Load<Item>(comments.Select(x => (ValueType) x.ItemId));

                            for (var i = 0; i < comments.Count; i++)
                            {
                                if (existingItems[i] != null)
                                {
                                    existingItems[i].Comments.Add(new Comment
                                    {
                                        Author = comments[i].Name,
                                        Content = comments[i].Message,
                                        Created = comments[i].Created,
                                        Email = comments[i].Email
                                    });
                                }
                            }

                            // Save any changes
                            documentSession.SaveChanges();
                            count += comments.Count;
                            _log.Debug("Item migration progress... {0}/{1}", count, results.Count);
                        }
                    }
                }
            }
        }
    }
}