using System;
using System.Collections.Generic;
using CollectionsOnline.Core.Models;

namespace CollectionsOnline.WebSite.Models
{
    public class ArticleApiViewModel : AggregateRoot
    {
        public DateTime DateModified { get; set; }

        public string Title { get; set; }

        public IList<string> Keywords { get; set; }

        public string Content { get; set; }

        public string ContentSummary { get; set; }

        public IList<string> Types { get; set; }

        public IList<Author> Authors { get; set; }

        public IList<Author> Contributors { get; set; }

        public IList<Media> Media { get; set; }

        public string ParentArticleId { get; set; }

        public IList<string> ChildArticleIds { get; set; }

        public IList<string> RelatedArticleIds { get; set; }

        public IList<string> RelatedItemIds { get; set; }

        public IList<string> RelatedSpecimenIds { get; set; }
    }
}