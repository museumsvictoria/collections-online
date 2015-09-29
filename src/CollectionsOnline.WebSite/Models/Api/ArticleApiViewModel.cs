using System;
using System.Collections.Generic;
using CollectionsOnline.Core.Models;

namespace CollectionsOnline.WebSite.Models.Api
{
    public class ArticleApiViewModel : AggregateRoot
    {
        public string RecordType { get; set; }

        public DateTime DateModified { get; set; }

        public string Title { get; set; }

        public IList<string> Keywords { get; set; }

        public IList<string> Localities { get; set; }

        public string Content { get; set; }

        public string ContentSummary { get; set; }

        public IList<string> Types { get; set; }

        public IList<AuthorApiViewModel> Authors { get; set; }

        public IList<AuthorApiViewModel> Contributors { get; set; }

        public IList<MediaApiViewModel> Media { get; set; }

        public string YearWritten { get; set; }

        public string ParentArticleId { get; set; }

        public IList<string> ChildArticleIds { get; set; }

        public IList<string> RelatedArticleIds { get; set; }

        public IList<string> RelatedItemIds { get; set; }

        public IList<string> RelatedSpecimenIds { get; set; }
    }
}