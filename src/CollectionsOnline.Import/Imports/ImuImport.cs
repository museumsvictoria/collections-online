using System;
using System.Linq;
using AutoMapper;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Import.Factories;
using IMu;
using NLog;
using Raven.Client;

namespace CollectionsOnline.Import.Imports
{
    public class ImuImport<T> : IImport where T : EmuAggregateRoot
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly IDocumentStore _documentStore;
        private readonly Session _session;
        private readonly IImuFactory<T> _imuFactory;

        public ImuImport(
            IDocumentStore documentStore,
            Session session,
            IImuFactory<T> imuFactory)
        {
            _documentStore = documentStore;
            _session = session;
            _imuFactory = imuFactory;
        }

        public void Run(DateTime dateLastRun)
        {
            _log.Debug("Beginning {0} import", typeof(T).Name);

            var module = new Module(_imuFactory.ModuleName, _session);
            var terms = _imuFactory.Terms;

            if (dateLastRun == default(DateTime))
            {
                // Import has never run, do a fresh import
                var hits = module.FindTerms(terms);

                _log.Debug("Finished Search. {0} Hits", hits);

                var count = 0;

                while (true)
                {
                    using (var documentSession = _documentStore.OpenSession())
                    {
                        if (Program.ImportCanceled)
                        {
                            _log.Debug("Canceling Data import");
                            return;
                        }

                        // TODO: REMOVE IMPORT LIMIT
                        if (count >= 100)
                            break;

                        var results = module.Fetch("start", count, Constants.DataBatchSize, _imuFactory.Columns);

                        if (results.Count == 0)
                            break;

                        // Create and store documents
                        results.Rows
                            .Select(_imuFactory.MakeDocument)
                            .ToList()
                            .ForEach(documentSession.Store);

                        // Save any changes
                        documentSession.SaveChanges();
                        count += results.Count;
                        _log.Debug("{0} import progress... {1}/{2}", typeof(T).Name, count, hits);
                    }
                }
            }
            else
            {
                // Import has been run before, do an update import
                _imuFactory.RegisterAutoMapperMap();

                terms.Add("AdmDateModified", dateLastRun.ToString("MMM dd yyyy"), ">=");

                var hits = module.FindTerms(terms);

                _log.Debug("Finished Search. {0} Hits", hits);

                var count = 0;

                while (true)
                {
                    using (var documentSession = _documentStore.OpenSession())
                    {
                        if (Program.ImportCanceled)
                        {
                            _log.Debug("Canceling Data import");
                            return;
                        }

                        // TODO: REMOVE IMPORT LIMIT
                        if (count >= 100)
                            break;

                        var results = module.Fetch("start", count, Constants.DataBatchSize, _imuFactory.Columns);

                        if (results.Count == 0)
                            break;

                        // Update documents
                        var newDocuments = results.Rows.Select(_imuFactory.MakeDocument).ToList();
                        var existingDocuments = documentSession.Load<T>(newDocuments.Select(x => x.Id));

                        for (var i = 0; i < newDocuments.Count; i++)
                        {
                            if (existingDocuments[i] != null)
                            {
                                // Update existing story
                                Mapper.Map(newDocuments[i], existingDocuments[i]);
                            }
                            else
                            {
                                // Create new story
                                documentSession.Store(newDocuments[i]);
                            }
                        }

                        // Save any changes
                        documentSession.SaveChanges();
                        count += results.Count;
                        _log.Debug("{0} import progress... {1}/{2}", typeof(T).Name, count, hits);
                    }
                }
            }
        }
    }
}