using System.Collections.Generic;
using System.Linq;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.WebSite.Models;
using Raven.Client;

namespace CollectionsOnline.WebSite.Queries
{
    public class TermSearchViewModelQuery : ITermSearchViewModelQuery
    {
        private readonly IDocumentSession _documentSession;

        public TermSearchViewModelQuery(
            IDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }

        public IList<TermSearchViewModel> BuildLocalityTermSearch(TermSearchInputModel termSearchInputModel)
        {
            var result = new List<TermSearchViewModel>();

            if (!string.IsNullOrWhiteSpace(termSearchInputModel.Query))
            {
                result = _documentSession.Advanced
                    .DocumentQuery<LocalityTermIndexResult, LocalityTermIndex>()
                    .Search("Locality", string.Format("{0}*", termSearchInputModel.Query.Replace("*", string.Empty)),
                        EscapeQueryOptions.AllowPostfixWildcard)
                    .OrderByScoreDescending()
                    .Take(termSearchInputModel.Limit)
                    .Select(x => new TermSearchViewModel { Label = x.Locality })
                    .ToList();
            }

            return result;
        }

        public IList<TermSearchViewModel> BuildKeywordTermSearch(TermSearchInputModel termSearchInputModel)
        {
            var result = new List<TermSearchViewModel>();

            if (!string.IsNullOrWhiteSpace(termSearchInputModel.Query))
            {
                result = _documentSession.Advanced
                    .DocumentQuery<KeywordTermIndexResult, KeywordTermIndex>()
                    .Search("Keyword", string.Format("{0}*", termSearchInputModel.Query.Replace("*", string.Empty)),
                        EscapeQueryOptions.AllowPostfixWildcard)
                    .OrderByScoreDescending()
                    .Take(termSearchInputModel.Limit)
                    .Select(x => new TermSearchViewModel { Label = x.Keyword })
                    .ToList();
            }

            return result;
        }

        public IList<TermSearchViewModel> BuildNameTermSearch(TermSearchInputModel termSearchInputModel)
        {
            var result = new List<TermSearchViewModel>();

            if (!string.IsNullOrWhiteSpace(termSearchInputModel.Query))
            {
                result = _documentSession.Advanced
                    .DocumentQuery<NameTermIndexResult, NameTermIndex>()
                    .Search("Name", string.Format("{0}*", termSearchInputModel.Query.Replace("*", string.Empty)),
                        EscapeQueryOptions.AllowPostfixWildcard)
                    .OrderByScoreDescending()
                    .Take(termSearchInputModel.Limit)
                    .Select(x => new TermSearchViewModel { Label = x.Name })
                    .ToList();
            }

            return result;
        }

    }
}