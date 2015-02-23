using System.Collections.Generic;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Models;

namespace CollectionsOnline.WebSite.Transformers
{
    public class ItemViewTransformerResult
    {
        public Item Item { get; set; }

        public IList<RelatedDocumentViewModel> RelatedItems { get; set; }

        public IList<RelatedDocumentViewModel> RelatedSpecimens { get; set; }

        public IList<RelatedDocumentViewModel> RelatedArticles { get; set; }

        public IList<RelatedDocumentViewModel> RelatedSpecies { get; set; }

        public int RelatedSpeciesSpecimenItemCount { get; set; }

        public ImageMedia ItemHeroImage { get; set; }

        public IList<ImageMedia> ItemImages { get; set; }

        public ItemViewTransformerResult()
        {
            RelatedItems = new List<RelatedDocumentViewModel>();
            RelatedSpecimens = new List<RelatedDocumentViewModel>();
            RelatedArticles = new List<RelatedDocumentViewModel>();
            RelatedSpecies = new List<RelatedDocumentViewModel>();
            ItemImages = new List<ImageMedia>();
        }
    }
}