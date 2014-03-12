using System;
using System.Collections.Generic;
using System.Linq;

namespace CollectionsOnline.Core.Models
{
    public class Story : EmuAggregateRoot
    {
        #region Non-Emu

        public string Summary { get; set; }

        public int Quality
        {
            get
            {
                var qualityCount = 0;

                if (Tags.Any())
                    qualityCount += Tags.Count;
                if (Types.Any())
                    qualityCount += Types.Count;
                if (GeographicTags.Any())
                    qualityCount += GeographicTags.Count;
                if (Authors.Any())
                    qualityCount += Authors.Count;
                if (Media.Any())
                    qualityCount += Media.Count * 2;
                if (Authors.Any())
                    qualityCount += Authors.Count;
                if (ChildStoryIds.Any())
                    qualityCount += ChildStoryIds.Count;
                if (!string.IsNullOrWhiteSpace(ParentStoryId))
                    qualityCount += 1;
                if (RelatedStoryIds.Any())
                    qualityCount += RelatedStoryIds.Count;
                if (RelatedItemIds.Any())
                    qualityCount += RelatedItemIds.Count;

                return qualityCount;
            }
        }

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
    }
}