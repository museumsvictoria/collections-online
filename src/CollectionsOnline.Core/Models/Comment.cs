using System;

namespace CollectionsOnline.Core.Models
{
    public class Comment
    {
        public string Author { get; set; }

        public string Content { get; set; }

        public string Email { get; set; }

        public bool IsSpam { get; set; }

        public DateTime Created { get; set; }
    }
}