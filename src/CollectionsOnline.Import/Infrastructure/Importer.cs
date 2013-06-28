//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using CollectionsOnline.Core.Config;
//using CollectionsOnline.Core.DomainModels;
//using IMu;
//using NLog;
//using Raven.Client;

//namespace CollectionsOnline.Import.Infrastructure
//{
//    public class Importer
//    {
//        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

//        public void Import<T>(IDocumentStore documentStore, Session session, IImportTask<T> importTask, DateTime dateLastRun)
//        {
//            _log.Debug("Begining import");
//            var documents = new List<T>();
//            var module = new Module(importTask.Module(), session);
//            var terms = importTask.Terms();

//            if (dateLastRun == default(DateTime))
//            {
//                var hits = module.FindTerms(terms);

//                _log.Debug("Finished Search. {0} Hits", hits);

//                var count = 0;

//                while (true)
//                {
//                    using (var documentSession = documentStore.OpenSession())
//                    {
//                        if (_importCancelled)
//                        {
//                            _log.Debug("Canceling Data import");
//                            return;
//                        }

//                        var results = module.Fetch("start", count, Constants.DataBatchSize, importTask.Columns());

//                        if (results.Count == 0)
//                            break;

//                        // Create documents
//                        documents.AddRange(results.Rows.Select(importTask.Create));

//                        count += results.Count;
//                        _log.Debug("Many Nations Label import progress... {0}/{1}", count, hits);
//                    }
//                }
//            }
//            else
//            {
//                terms.Add("AdmDateModified", dateLastRun.ToString("MMM dd yyyy"), ">=");

//                var hits = module.FindTerms(terms);

//                _log.Debug("Finished Search. {0} Hits", hits);

//                var count = 0;

//                while (true)
//                {
//                    using (var documentSession = documentStore.OpenSession())
//                    {
//                        if (_importCancelled)
//                        {
//                            _log.Debug("Canceling Data import");
//                            return;
//                        }

//                        var results = module.Fetch("start", count, Constants.DataBatchSize, importTask.Columns());

//                        if (results.Count == 0)
//                            break;

//                        // Update documents
//                        var existingDocuments = documentSession.Load<T>(results.Rows.Select(x => @"document/" + x.GetString("irn")));



//                        count += results.Count;
//                        _log.Debug("Many Nations Label import progress... {0}/{1}", count, hits);
//                    }
//                }
//            }
//        }
//    }
//}