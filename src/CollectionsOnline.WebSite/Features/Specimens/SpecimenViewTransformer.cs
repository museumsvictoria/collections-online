using System.Collections.Generic;
using System.Linq;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Features.Shared;
using Raven.Client.Indexes;

namespace CollectionsOnline.WebSite.Features.Specimens
{
    public class SpecimenViewTransformer : AbstractTransformerCreationTask<Specimen>
    {
        public SpecimenViewTransformer()
        {
            TransformResults = specimens => from specimen in specimens
                select new
                {
                    Specimen = specimen,
                    RelatedItems = from itemId in specimen.RelatedItemIds
                        let relatedItem = LoadDocument<Item>(itemId)
                        where relatedItem != null && !relatedItem.IsHidden
                        select new
                        {
                            relatedItem.Id, 
                            relatedItem.ThumbnailUri,
                            relatedItem.DisplayTitle
                        },
                    RelatedSpecimens = from specimenId in specimen.RelatedSpecimenIds
                        let relatedSpecimen = LoadDocument<Specimen>(specimenId)
                        where relatedSpecimen != null && !relatedSpecimen.IsHidden
                        select new
                        {
                            relatedSpecimen.Id,
                            relatedSpecimen.ThumbnailUri,
                            relatedSpecimen.DisplayTitle
                        },
                    RelatedSpecies = from speciesId in specimen.RelatedSpeciesIds
                        let relatedSpecies = LoadDocument<Core.Models.Species>(speciesId)
                        where relatedSpecies != null && !relatedSpecies.IsHidden
                        select new
                        {
                            relatedSpecies.Id,
                            relatedSpecies.ThumbnailUri,
                            relatedSpecies.DisplayTitle
                        },
                    RelatedArticles = from articleId in specimen.RelatedArticleIds
                        let relatedArticle = LoadDocument<Article>(articleId)
                        where relatedArticle != null && !relatedArticle.IsHidden
                        select new
                        {
                            relatedArticle.Id,
                            relatedArticle.ThumbnailUri,
                            relatedArticle.DisplayTitle
                        }
                };
        }
    }

    public class SpecimenViewTransformerResult
    {
        public Specimen Specimen { get; set; }

        public IList<RelatedDocumentViewModel> RelatedItems { get; set; }

        public IList<RelatedDocumentViewModel> RelatedSpecimens { get; set; }

        public IList<RelatedDocumentViewModel> RelatedSpecies { get; set; }

        public IList<RelatedDocumentViewModel> RelatedArticles { get; set; }

        public int RelatedSpeciesSpecimenItemCount { get; set; }
        
        public ImageMedia SpecimenHeroImage { get; set; }
        
        public IList<ImageMedia> SpecimenImages { get; set; }

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