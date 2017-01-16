using System;
using System.Collections.Generic;
using System.Linq;
using CollectionsOnline.Core.Indexes;
using Raven.Client;

namespace CollectionsOnline.WebSite.Queries
{
    public static class QueryBuilder<T>
    {
        public static IDocumentQuery<T> BuildIndexQuery(IDocumentSession documentSession,
            int page,
            int perPage,
            string sort,
            IList<string> queries,
            IDictionary<string, string> facets,
            IDictionary<string, string[]> multiFacets,
            IDictionary<string, string> terms,
            DateTime? minDateModified = null,
            DateTime? maxDateModified = null,
            EscapeQueryOptions escapeQueryOptions = EscapeQueryOptions.RawQuery)
        {
            // perform query                
            var query = documentSession.Advanced
                .DocumentQuery<T, CombinedIndex>()
                .Skip((page - 1) * perPage)
                .Take(perPage);

            // search query (only add AndAlso() after first query)
            for (int i = 0; i < queries.Count; i++)
            {
                if (i == 0)
                {
                    query = query.Search("Content", queries[i], escapeQueryOptions);
                }
                else
                {
                    query = query.AndAlso().Search("Content", queries[i], escapeQueryOptions);
                }
            }

            // Add sorting
            switch (sort)
            {
                case "quality":
                    query = query
                        .OrderByDescending("Quality");
                    break;
                case "date":
                    query = query
                        .OrderByDescending("DateModified")
                        .OrderByDescending("Quality");
                    break;
                case "random":
                    query = query
                        .RandomOrdering();
                    break;
            }

            if (queries.Any())
            {
                query = query.OrderByScoreDescending();
            }

            // facet queries
            foreach (var facet in facets)
            {
                query = query.AndAlso().WhereEquals(facet.Key, facet.Value);
            }

            // multiple facet queries
            foreach (var multiFacet in multiFacets)
            {
                foreach (var facetValue in multiFacet.Value)
                {
                    query = query.AndAlso().WhereEquals(multiFacet.Key, facetValue);
                }
            }

            // term queries
            foreach (var term in terms)
            {
                query = query.AndAlso().WhereEquals(term.Key, term.Value);
            }

            // DateModified
            if (minDateModified.HasValue && maxDateModified.HasValue)
                query = query.AndAlso().WhereBetweenOrEqual("DateModified", minDateModified, maxDateModified);
            else if (minDateModified.HasValue)
                query = query.AndAlso().WhereGreaterThanOrEqual("DateModified", minDateModified);
            else if (maxDateModified.HasValue)
                query = query.AndAlso().WhereLessThanOrEqual("DateModified", maxDateModified);

            return query;
        }
    }
}