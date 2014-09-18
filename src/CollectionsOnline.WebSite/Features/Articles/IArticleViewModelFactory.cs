using CollectionsOnline.Core.Models;

namespace CollectionsOnline.WebSite.Features.Articles
{
    public interface IArticleViewModelFactory
    {
        ArticleViewModel MakeViewModel(Article article);
    }
}