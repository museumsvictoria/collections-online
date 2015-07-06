using System.Linq;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Transformers;
using Newtonsoft.Json;
using Raven.Client;

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
            var result = _documentSession.Load<ItemViewTransformer, ItemViewTransformerResult>(itemId);

            if (result.Item.Taxonomy != null)
            {
                var query = _documentSession.Advanced
                    .DocumentQuery<CombinedIndexResult, CombinedIndex>()
                    .WhereEquals("Taxon", result.Item.Taxonomy.TaxonName)
                    .Take(1);

                // Dont allow a link to search page if the current item is the only result
                if (query.SelectFields<CombinedIndexResult>("Id").Select(x => x.Id).Except(new[] { itemId }).Any())
                {
                    result.RelatedSpeciesSpecimenItemCount = query.QueryResult.TotalResults;
                }
            }

            // Exclude file media as that is handled differently
            result.ItemMedia = result.Item.Media.Where(x => !(x is FileMedia)).ToList();
            result.JsonItemMedia = JsonConvert.SerializeObject(result.ItemMedia, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects });

            return result;
        }
    }
}