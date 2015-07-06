using System.Collections.Generic;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Models;

namespace CollectionsOnline.WebSite.Transformers
{
    public class SpecimenViewTransformerResult
    {
        public Specimen Specimen { get; set; }

        public IList<Media> SpecimenMedia { get; set; }

        public IList<EmuAggregateRootViewModel> RelatedItems { get; set; }

        public IList<EmuAggregateRootViewModel> RelatedSpecimens { get; set; }

        public IList<EmuAggregateRootViewModel> RelatedSpecies { get; set; }

        public IList<EmuAggregateRootViewModel> RelatedArticles { get; set; }

        public int RelatedSpeciesSpecimenItemCount { get; set; }

        public string JsonSpecimenMedia { get; set; }

        public string JsonSpecimenLatLongs { get; set; }

        public IList<KeyValuePair<string, string>> GeoSpatial { get; set; }

        public SpecimenViewTransformerResult()
        {
            SpecimenMedia = new List<Media>();
            RelatedItems = new List<EmuAggregateRootViewModel>();
            RelatedSpecimens = new List<EmuAggregateRootViewModel>();
            RelatedSpecies = new List<EmuAggregateRootViewModel>();
            RelatedArticles = new List<EmuAggregateRootViewModel>();
            GeoSpatial = new List<KeyValuePair<string, string>>();
        }
    }
}