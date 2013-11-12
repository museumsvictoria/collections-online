using System;
using System.Linq;
using AutoMapper;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Models;
using IMu;
using NLog;
using Raven.Client;

namespace CollectionsOnline.Import.Importers
{
    public abstract class Import<T> : IImport<T> where T : EmuAggregateRoot
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly IDocumentStore _documentStore;
        private readonly Session _session;

        protected Import(
            IDocumentStore documentStore,
            Session session)
        {
            _documentStore = documentStore;
            _session = session;
        }

        public void Run(DateTime dateLastRun)
        {
            _log.Debug("Begining {0} import", typeof(T).Name);

            var module = new Module(ModuleName, _session);
            var terms = Terms;

            if (dateLastRun == default(DateTime))
            {
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
                        if (count >= 500)
                            break;

                        var results = module.Fetch("start", count, Constants.DataBatchSize, Columns);

                        if (results.Count == 0)
                            break;

                        // Create and store documents
                        results.Rows
                            .Select(MakeDocument)
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
                Mapper.CreateMap<T, T>().ForMember(x => x.Id, y => y.Ignore());

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

                        var results = module.Fetch("start", count, Constants.DataBatchSize, Columns);

                        if (results.Count == 0)
                            break;

                        // Update documents
                        var newDocuments = results.Rows.Select(MakeDocument).ToList();
                        var existingDocuments = documentSession.Load<T>(newDocuments.Select(x => x.Id));

                        foreach (var newDocument in newDocuments)
                        {
                            var existingDocument = existingDocuments.SingleOrDefault(x => x != null && x.Id == newDocument.Id);

                            if (existingDocument != null)
                            {
                                // Update existing story
                                Mapper.Map(newDocument, existingDocument);
                            }
                            else
                            {
                                // Create new story
                                documentSession.Store(newDocument);
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

        public abstract string ModuleName { get; }

        public abstract string[] Columns { get; }

        public abstract Terms Terms { get; }

        public abstract T MakeDocument(Map map);
    }
}