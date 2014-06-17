using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Core.Models;
using Dapper;
using NLog;
using Raven.Client;

namespace CollectionsOnline.Import.Imports
{
    public class MigrationImport : IImport
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly IDocumentStore _documentStore;

        public MigrationImport(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public void Run()
        {
            using (var documentSession = _documentStore.OpenSession())
            {
                // Check to see whether we need to run import, so grab the previous date run of any item imports.
                var previousDateRun = documentSession
                    .Load<Application>(Constants.ApplicationId)
                    .ImportStatuses.Where(x => x.ImportType.Contains(typeof(Item).Name, StringComparison.OrdinalIgnoreCase))
                    .Select(x => x.PreviousDateRun)
                    .OrderBy(x => x)
                    .FirstOrDefault(x => x.HasValue);

                // Exit current import if it has run before as we only need to get comments once.
                if (previousDateRun.HasValue)
                    return;
            }

            _log.Debug("Beginning Item migration");

            using (var connection = new SqlConnection(ConfigurationManager.AppSettings["SqlImportConnectionString"]))
            {
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
                        if (ImportCanceled())
                        {
                            return;
                        }

                        var comments = results
                            .Skip(count)
                            .Take(Constants.DataBatchSize)
                            .ToList();

                        if (comments.Count == 0)
                            break;

                        var existingItems = documentSession.Load<Item>(comments.Select(x => (ValueType)x.ItemId));

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