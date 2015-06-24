using System.Collections.Generic;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Models;

namespace CollectionsOnline.WebSite.Transformers
{
    public class SpeciesViewTransformerResult
    {
        public Species Species { get; set; }

        public IList<EmuAggregateRootViewModel> RelatedItems { get; set; }

        public IList<EmuAggregateRootViewModel> RelatedSpecimens { get; set; }

        public int RelatedSpecimenCount { get; set; }

        public IList<ImageMedia> SpeciesImages { get; set; }        

        public IList<FileMedia> SpeciesFiles { get; set; }

        public IList<VideoMedia> SpeciesVideos { get; set; }

        public string JsonSpeciesImages { get; set; }

        public SpeciesViewTransformerResult()
        {
            RelatedItems = new List<EmuAggregateRootViewModel>();
            RelatedSpecimens = new List<EmuAggregateRootViewModel>();
            SpeciesImages = new List<ImageMedia>();
            SpeciesFiles = new List<FileMedia>();
            SpeciesVideos = new List<VideoMedia>();
        }
    }
}