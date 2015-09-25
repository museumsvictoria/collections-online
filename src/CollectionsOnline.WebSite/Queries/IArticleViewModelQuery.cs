using CollectionsOnline.WebSite.Models.Api;
using CollectionsOnline.WebSite.Transformers;

namespace CollectionsOnline.WebSite.Queries
{
    public interface IArticleViewModelQuery
    {
        ArticleViewTransformerResult BuildArticle(string articleId);

        ApiViewModel BuildArticleApiIndex(ApiInputModel apiInputModel);
    }
}