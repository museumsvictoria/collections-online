using System;

namespace CollectionsOnline.WebSite.Models.Api
{
    public class CommentApiViewModel
    {
        public string Author { get; set; }

        public string Content { get; set; }

        public DateTime Created { get; set; }
    }
}