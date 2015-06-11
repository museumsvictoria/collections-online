using System;
using System.Collections.Generic;

namespace CollectionsOnline.Core.Models
{
    public class Collection : EmuAggregateRoot
    {
        public Collection()
        {
            InitializeCollections();
        }

        #region Non-Emu

        public string Summary { get; set; }

        public string ThumbnailUri { get; set; }

        public string DisplayTitle { get; set; }

        #endregion
        
        public DateTime DateModified { get; set; }

        public string Title { get; set; }

        public string Significance { get; set; }

        public string CollectionSummary { get; set; }

        public string Category { get; set; }
        
        public IList<Author> Authors { get; set; }

        public IList<EmuSummary> FavoriteItems { get; set; }

        public IList<EmuSummary> FavoriteSpecimens { get; set; }

        public IList<EmuSummary> SubCollectionArticles { get; set; }

        public string CollectingArea { get; set; }

        public IList<Media> Media { get; set; }
        
        private void InitializeCollections()
        {
            Authors = new List<Author>();
            FavoriteItems = new List<EmuSummary>();
            FavoriteSpecimens = new List<EmuSummary>();
            SubCollectionArticles = new List<EmuSummary>();
            Media = new List<Media>();
        }
    }
}