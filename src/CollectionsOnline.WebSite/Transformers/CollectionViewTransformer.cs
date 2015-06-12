using System.Linq;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Models;
using Raven.Client.Indexes;

namespace CollectionsOnline.WebSite.Transformers
{
    public class CollectionViewTransformer : AbstractTransformerCreationTask<Collection>
    {
        public CollectionViewTransformer()
        {
            TransformResults = collections => from collection in collections
                select new
                {
                    Collection = collection,
                    FavoriteItems = from item in collection.FavoriteItems
                        let favoriteItem = LoadDocument<Item>(item.Id)
                        where favoriteItem != null && !favoriteItem.IsHidden
                        select new
                        {
                            favoriteItem.Id,
                            favoriteItem.DisplayTitle,
                            item.Summary,
                            favoriteItem.ThumbnailUri,
                            RecordType = "Item"
                        },
                    FavoriteSpecimens = from specimen in collection.FavoriteSpecimens
                        let favoriteSpecimen = LoadDocument<Specimen>(specimen.Id)
                        where favoriteSpecimen != null && !favoriteSpecimen.IsHidden
                        select new
                        {
                            favoriteSpecimen.Id,
                            favoriteSpecimen.DisplayTitle,
                            specimen.Summary,
                            favoriteSpecimen.ThumbnailUri,
                            RecordType = "Specimen"
                        },
                    SubCollectionArticles = from article in collection.SubCollectionArticles
                        let subCollectionArticle = LoadDocument<Article>(article.Id)
                        where subCollectionArticle != null && !subCollectionArticle.IsHidden
                        select new
                        {
                            subCollectionArticle.Id,
                            subCollectionArticle.DisplayTitle,
                            subCollectionArticle.Summary,
                            subCollectionArticle.ThumbnailUri,
                            RecordType = "Article"
                        }
                };
        }
    }
}