using System;
using System.Collections.Generic;

namespace CollectionsOnline.Core.Models
{
    public class Story : EmuAggregateRoot
    {
        public DateTime DateModified { get; set; }

        public string Title { get; set; }

        public ICollection<string> Tags { get; set; }

        public string Content { get; set; }

        public string ContentSummary { get; set; }

        public ICollection<string> Types { get; set; }

        public ICollection<string> GeographicTags { get; set; }        

        public ICollection<Author> Authors { get; set; }

        public ICollection<Media> Media { get; set; }

        public string ParentStoryId { get; set; }

        public ICollection<string> RelatedStoryIds { get; set; }

        public ICollection<string> RelatedItemIds { get; set; }

        public Story(string irn)
        {
            Id = "stories/" + irn;
        }
    }
}