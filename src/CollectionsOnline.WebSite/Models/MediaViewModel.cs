using System.Collections.Generic;
using System.Linq;
using CollectionsOnline.Core.Models;

namespace CollectionsOnline.WebSite.Models
{
    public class MediaViewModel
    {
        public IEnumerable<Media> Medias { get; set; }

        public string DocumentId { get; set; }
    }
}