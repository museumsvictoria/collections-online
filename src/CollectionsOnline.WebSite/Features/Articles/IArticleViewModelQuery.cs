namespace CollectionsOnline.WebSite.Features.Articles
{
    public interface IArticleViewModelQuery
    {
        ArticleViewModel BuildArticle(string articleId);
    }
}