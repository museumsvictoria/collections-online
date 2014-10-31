using System.Linq;
using CollectionsOnline.Core.Indexes;
using Raven.Client;

namespace CollectionsOnline.WebSite.Features.Items
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
                    .LuceneQuery<CombinedResult, Combined>()
                    .WhereEquals("Taxon", result.Item.Taxonomy.TaxonName);

                // Dont allow a link to search page if the current item is the only result
                if (query.SelectFields<CombinedResult>("Id").Select(x => x.Id).Except(new[] { itemId }).Any())
                {
                    result.RelatedSpeciesSpecimenItemCount = query.QueryResult.TotalResults;
                }
            }

            return result;
        }
    }
}