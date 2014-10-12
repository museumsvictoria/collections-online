namespace CollectionsOnline.WebSite.Features.Articles
{
    public interface IArticleViewModelQuery
    {
        ArticleViewTransformerResult BuildArticle(string articleId);
    }
}