using System;
using System.Collections.Generic;
using System.Linq;

namespace CollectionsOnline.Core.Models
{
    public class Article : EmuAggregateRoot
    {
        public Article()
        {
            InitializeCollections();
        }

        #region Non-Emu

        public string Summary { get; set; }

        public string ThumbnailUri { get; set; }

        #endregion

        public DateTime DateModified { get; set; }

        public string Title { get; set; }

        public IList<string> Tags { get; set; }

        public string Content { get; set; }

        public string ContentSummary { get; set; }

        public IList<string> Types { get; set; }

        public IList<string> GeographicTags { get; set; }

        public IList<Author> Authors { get; set; }

        public IList<Author> Contributors { get; set; }

        public IList<Media> Media { get; set; }

        public string ParentArticleId { get; set; }

        public IList<string> ChildArticleIds { get; set; }

        public IList<string> RelatedArticleIds { get; set; }

        public IList<string> RelatedItemIds { get; set; }

        public IList<string> RelatedSpecimenIds { get; set; }

        private void InitializeCollections()
        {
            Tags = new List<string>();
            Types = new List<string>();
            GeographicTags = new List<string>();
            Authors = new List<Author>();
            Contributors = new List<Author>();
            Media = new List<Media>();
            ChildArticleIds = new List<string>();
            RelatedArticleIds = new List<string>();
            RelatedItemIds = new List<string>();
            RelatedSpecimenIds = new List<string>();
        }
    }
}