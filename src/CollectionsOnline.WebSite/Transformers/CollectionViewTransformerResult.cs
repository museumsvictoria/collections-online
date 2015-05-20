using System.Collections.Generic;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Models;

namespace CollectionsOnline.WebSite.Transformers
{
    public class CollectionViewTransformerResult
    {
        public Collection Collection { get; set; }

        public IList<EmuAggregateRootViewModel> FavoriteItems { get; set; }

        public IList<EmuAggregateRootViewModel> FavoriteSpecimens { get; set; }

        public IList<EmuAggregateRootViewModel> SubCollectionArticles { get; set; }

        public CollectionViewTransformerResult()
        {
            FavoriteItems = new List<EmuAggregateRootViewModel>();
            FavoriteSpecimens = new List<EmuAggregateRootViewModel>();
            SubCollectionArticles = new List<EmuAggregateRootViewModel>();
        }
    }
}