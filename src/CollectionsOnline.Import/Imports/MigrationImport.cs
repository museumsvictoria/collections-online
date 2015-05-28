using System;
using System.Collections.Generic;
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
            List<dynamic> results;

            using (var documentSession = _documentStore.OpenSession())
            {
                // Check to see whether we need to run import, so grab the previous date run of any item imports                
                var itemImportPreviousDateRun = documentSession
                    .Load<Application>(Constants.ApplicationId)
                    .ImportStatuses.Where(x => x.ImportType.Contains(typeof(Item).Name, StringComparison.OrdinalIgnoreCase))
                    .Select(x => x.PreviousDateRun)
                    .OrderBy(x => x)
                    .FirstOrDefault(x => x.HasValue);                

                // Exit if the item import has never run before (should never happen given the order)
                if (!itemImportPreviousDateRun.HasValue)
                    return;

                var importStatus = documentSession.Load<Application>(Constants.ApplicationId).GetImportStatus(GetType().ToString());

                // Exit current import if migration has been run before or import has finished
                if (importStatus.PreviousDateRun.HasValue || importStatus.IsFinished)
                    return;

                documentSession.SaveChanges();
            }

            _log.Debug("Beginning item migration");

            // Fetch old comments from collections online
            using (var connection = new SqlConnection(ConfigurationManager.AppSettings["SqlImportConnectionString"]))
            {
                const string commentsSqlQuery = @"SELECT Comment.*, ItemComment.ItemId
                            FROM Comment INNER JOIN
                                ItemComment ON Comment.CommentId = ItemComment.CommentId
                            UNION
                            SELECT Comment.*, ItemCommentArchive.ItemId
                            FROM Comment INNER JOIN
                                    ItemCommentArchive ON Comment.CommentId = ItemCommentArchive.CommentId";

                results = connection.Query(commentsSqlQuery).ToList();

                _log.Debug("Fetched item comments. {0} Hits", results.Count);
            }

            var count = 0;

            while (true)
            {
                using (var documentSession = _documentStore.OpenSession())
                {
                    if (ImportCanceled())
                        return;

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

            using (var documentSession = _documentStore.OpenSession())
            {
                // Mark import status as finished
                documentSession.Load<Application>(Constants.ApplicationId).ImportFinished(GetType().ToString(), DateTime.Now);
                documentSession.SaveChanges();
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