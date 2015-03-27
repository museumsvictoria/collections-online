using System.Collections.Generic;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Models;

namespace CollectionsOnline.WebSite.Transformers
{
    public class SpecimenViewTransformerResult
    {
        public Specimen Specimen { get; set; }

        public IList<RelatedDocumentViewModel> RelatedItems { get; set; }

        public IList<RelatedDocumentViewModel> RelatedSpecimens { get; set; }

        public IList<RelatedDocumentViewModel> RelatedSpecies { get; set; }

        public IList<RelatedDocumentViewModel> RelatedArticles { get; set; }

        public int RelatedSpeciesSpecimenItemCount { get; set; }

        public IList<ImageMedia> SpecimenImages { get; set; }

        public string JsonSpecimenImages { get; set; }

        public SpecimenViewTransformerResult()
        {
            RelatedItems = new List<RelatedDocumentViewModel>();
            RelatedSpecimens = new List<RelatedDocumentViewModel>();
            RelatedSpecies = new List<RelatedDocumentViewModel>();
            RelatedArticles = new List<RelatedDocumentViewModel>();
            SpecimenImages = new List<ImageMedia>();
        }
    }
}