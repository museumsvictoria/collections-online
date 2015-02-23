using System.Collections.Generic;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Models;

namespace CollectionsOnline.WebSite.Transformers
{
    public class SpeciesViewTransformerResult
    {
        public Species Species { get; set; }

        public IList<RelatedDocumentViewModel> RelatedItems { get; set; }

        public IList<RelatedDocumentViewModel> RelatedSpecimens { get; set; }

        public int RelatedSpecimenCount { get; set; }

        public ImageMedia SpeciesHeroImage { get; set; }

        public IList<ImageMedia> SpeciesImages { get; set; }

        public SpeciesViewTransformerResult()
        {
            RelatedItems = new List<RelatedDocumentViewModel>();
            RelatedSpecimens = new List<RelatedDocumentViewModel>();
            SpeciesImages = new List<ImageMedia>();
        }
    }
}