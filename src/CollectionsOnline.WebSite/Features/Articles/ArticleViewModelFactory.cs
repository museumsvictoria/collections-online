using System.Linq;
using CollectionsOnline.Core.Models;

namespace CollectionsOnline.WebSite.Features.Articles
{
    public class ArticleViewModelFactory : IArticleViewModelFactory
    {
        public ArticleViewModel MakeViewModel(Article article)
        {
            var articleViewModel = new ArticleViewModel
                {
                    Article = article,
                    ImageMedia = article.Media.Where(x => x is ImageMedia).Cast<ImageMedia>().ToList(),
                };

            return articleViewModel;
        }
    }
}