using CollectionsOnline.Core.Models;

namespace CollectionsOnline.WebSite.Features.Articles
{
    public class ArticleViewModelFactory : IArticleViewModelFactory
    {
        public ArticleViewModel MakeViewModel(Article article)
        {
            var articleViewModel = new ArticleViewModel
                {
                    Article = article
                };

            return articleViewModel;
        }
    }
}