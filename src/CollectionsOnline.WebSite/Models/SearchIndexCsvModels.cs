using System.Collections.Generic;

namespace CollectionsOnline.WebSite.Models
{
    public class SearchIndexCsvModel
    {
        public IList<EmuAggregateRootCsvModel> Results { get; set; }

        public SearchInputModel SearchInputModel { get; set; }

        public SearchIndexCsvModel()
        {
            Results = new List<EmuAggregateRootCsvModel>();
        }
    }
}