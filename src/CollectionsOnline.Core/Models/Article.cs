using System;
using System.Collections.Generic;

namespace CollectionsOnline.Core.Models
{
    public class Article : EmuAggregateRoot
    {
        public Article()
        {
            InitializeCollections();
        }

        #region Non-Emu

        public string ContentText { get; set; }

        public string Summary { get; set; }        

        public string ThumbnailUri { get; set; }

        public string DisplayTitle { get; set; }

        #endregion        
        
        public DateTime DateModified { get; set; }

        public string Title { get; set; }

        public IList<string> Keywords { get; set; }

        public IList<string> Localities { get; set; }

        public string Content { get; set; }

        public string ContentSummary { get; set; }

        public IList<string> Types { get; set; }

        public IList<Author> Authors { get; set; }

        public IList<Author> Contributors { get; set; }

        public IList<Media> Media { get; set; }

        public string YearWritten { get; set; }

        public string ParentArticleId { get; set; }

        public IList<string> ChildArticleIds { get; set; }

        public IList<string> RelatedArticleIds { get; set; }

        public IList<string> RelatedItemIds { get; set; }

        public IList<string> RelatedSpecimenIds { get; set; }

        private void InitializeCollections()
        {
            Keywords = new List<string>();
            Localities = new List<string>();
            Types = new List<string>();
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