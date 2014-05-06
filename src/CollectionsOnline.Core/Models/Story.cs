using System;
using System.Collections.Generic;
using System.Linq;

namespace CollectionsOnline.Core.Models
{
    public class Story : EmuAggregateRoot
    {
        public Story()
        {
            InitializeCollections();
        }

        #region Non-Emu

        public string Summary { get; set; }

        #endregion

        public DateTime DateModified { get; set; }

        public string Title { get; set; }

        public IList<string> Tags { get; set; }

        public string Content { get; set; }

        public string ContentSummary { get; set; }

        public IList<string> Types { get; set; }

        public IList<string> GeographicTags { get; set; }

        public IList<Author> Authors { get; set; }

        public IList<Media> Media { get; set; }

        public string ParentStoryId { get; set; }

        public IList<string> ChildStoryIds { get; set; }

        public IList<string> RelatedStoryIds { get; set; }

        public IList<string> RelatedItemIds { get; set; }

        private void InitializeCollections()
        {
            Tags = new List<string>();
            Types = new List<string>();
            GeographicTags = new List<string>();
            Authors = new List<Author>();
            Media = new List<Media>();
            ChildStoryIds = new List<string>();
            RelatedStoryIds = new List<string>();
            RelatedItemIds = new List<string>();
        }
    }
}