using System;
using System.Collections.Generic;

namespace CollectionsOnline.Core.DomainModels
{
    public class Story : DomainModel
    {
        public DateTime DateModified { get; set; }

        public string Title { get; set; }

        public ICollection<string> Tags { get; set; }

        public string Content { get; set; }

        public string ContentSummary { get; set; }

        public ICollection<string> Types { get; set; }

        public ICollection<string> GeographicTags { get; set; }        

        public ICollection<Author> Authors { get; set; }

        public Story(string irn)
        {
            Id = "Stories/" + irn;
        }
    }
}