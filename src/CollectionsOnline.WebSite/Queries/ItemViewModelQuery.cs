using System.Linq;
using AutoMapper;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Models.Api;
using CollectionsOnline.WebSite.Transformers;
using Raven.Client;
using StackExchange.Profiling;

namespace CollectionsOnline.WebSite.Queries
{
    public class ItemViewModelQuery : IItemViewModelQuery
    {
        private readonly IDocumentSession _documentSession;

        public ItemViewModelQuery(
            IDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }

        public ItemViewTransformerResult BuildItem(string itemId)
        {
            using (MiniProfiler.Current.Step("Build Item view model"))
            using (_documentSession.Advanced.DocumentStore.AggressivelyCacheFor(Constants.AggressiveCacheTimeSpan))
            {
                var result = _documentSession.Load<ItemViewTransformer, ItemViewTransformerResult>(itemId);

                if (result.Item.Taxonomy != null)
                {
                    var query = _documentSession.Advanced
                        .DocumentQuery<CombinedIndexResult, CombinedIndex>()
                        .WhereEquals("Taxon", result.Item.Taxonomy.TaxonName)
                        .Take(1);

                    // Dont allow a link to search page if the current item is the only result
                    if (query.SelectFields<CombinedIndexResult>("Id").Select(x => x.Id).Except(new[] {itemId}).Any())
                    {
                        result.RelatedSpeciesSpecimenItemCount = query.QueryResult.TotalResults;
                    }
                }

                return result;
            }
        }

        public ApiViewModel BuildItemApiIndex(ApiInputModel apiInputModel)
        {
            using (MiniProfiler.Current.Step("Build Item Api Index view model"))
            using (_documentSession.Advanced.DocumentStore.AggressivelyCacheFor(Constants.AggressiveCacheTimeSpan))
            {
                RavenQueryStatistics statistics;
                var results = _documentSession.Advanced
                    .DocumentQuery<dynamic, CombinedIndex>()
                    .WhereEquals("RecordType", "Item")
                    .Statistics(out statistics)
                    .Skip((apiInputModel.Page - 1) * apiInputModel.PerPage)
                    .Take(apiInputModel.PerPage);

                return new ApiViewModel
                {
                    Results = results.Cast<Item>().Select<Item, dynamic>(Mapper.Map<Item, ItemApiViewModel>).ToList(),
                    ApiPageInfo = new ApiPageInfo(statistics.TotalResults, apiInputModel.PerPage)
                };
            }
        }
    }
}