﻿using System.Collections.Generic;
using CollectionsOnline.Core.Models;

namespace CollectionsOnline.WebSite.Features.Articles
{
    public class ArticleViewModel
    {
        public Article Article { get; set; }

        public IList<ImageMedia> ImageMedia { get; set; }
    }
}