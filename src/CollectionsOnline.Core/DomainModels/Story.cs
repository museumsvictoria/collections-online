using System;
using System.Collections.Generic;
using CollectionsOnline.Core.Config;

namespace CollectionsOnline.Core.DomainModels
{
    public class Story : DomainModel
    {
        public string Title { get; private set; }

        public ICollection<string> Tags { get; private set; }

        public Story(
            string irn,
            string title,
            ICollection<string> tags)
        {
            Id = "Stories/" + irn;
            Title = title;
            Tags = tags;
        }

        public void Update(
            string title)
        {
            Title = title;
        }
    }
}