using System;
using System.Collections.Generic;
using CollectionsOnline.Core.Models;

namespace CollectionsOnline.WebSite.Models.Api
{
    public class SpecimenApiViewModel : AggregateRoot
    {
        public string RecordType { get; set; }

        public DateTime DateModified { get; set; }

        public string Category { get; set; }

        public string ScientificGroup { get; set; }

        public string Discipline { get; set; }

        public string RegistrationNumber { get; set; }

        public IList<string> CollectionNames { get; set; }

        public string Type { get; set; }

        public IList<string> Classifications { get; set; }

        public string ObjectName { get; set; }

        public string ObjectSummary { get; set; }

        public string IsdDescriptionOfContent { get; set; }

        public string Significance { get; set; }

        public IList<string> Keywords { get; set; }

        public IList<string> CollectingAreas { get; set; }

        public IList<Association> Associations { get; set; }

        public IList<string> RelatedItemIds { get; set; }

        public IList<string> RelatedSpecimenIds { get; set; }

        public IList<string> RelatedArticleIds { get; set; }

        public IList<string> RelatedSpeciesIds { get; set; }

        public string AcquisitionInformation { get; set; }

        public string Acknowledgement { get; set; }

        public MuseumLocation MuseumLocation { get; set; }

        public IList<MediaApiViewModel> Media { get; set; }
        
        public string NumberOfSpecimens { get; set; }

        public string ClutchSize { get; set; }

        public string Sex { get; set; }

        public string StageOrAge { get; set; }

        public IList<Storage> Storages { get; set; }

        public string TypeStatus { get; set; }

        public string IdentifiedBy { get; set; }

        public string DateIdentified { get; set; }

        public string Qualifier { get; set; }

        public string QualifierRank { get; set; }

        public TaxonomyApiViewModel Taxonomy { get; set; }

        public CollectionEventApiViewModel CollectionEvent { get; set; }

        public CollectionSiteApiViewModel CollectionSite { get; set; }

        public string PalaeontologyDateCollectedFrom { get; set; }

        public string PalaeontologyDateCollectedTo { get; set; }

        public string MineralogySpecies { get; set; }

        public string MineralogyVariety { get; set; }

        public string MineralogyGroup { get; set; }

        public string MineralogyClass { get; set; }

        public string MineralogyAssociatedMatrix { get; set; }

        public string MineralogyType { get; set; }

        public string MineralogyTypeOfType { get; set; }

        public string MeteoritesName { get; set; }

        public string MeteoritesClass { get; set; }

        public string MeteoritesGroup { get; set; }

        public string MeteoritesType { get; set; }

        public string MeteoritesMinerals { get; set; }

        public string MeteoritesSpecimenWeight { get; set; }

        public string MeteoritesTotalWeight { get; set; }

        public string MeteoritesDateFell { get; set; }

        public string MeteoritesDateFound { get; set; }

        public string TektitesName { get; set; }

        public string TektitesClassification { get; set; }

        public string TektitesShape { get; set; }

        public string TektitesLocalStrewnfield { get; set; }

        public string TektitesGlobalStrewnfield { get; set; }

        public string PetrologyRockClass { get; set; }

        public string PetrologyRockGroup { get; set; }

        public string PetrologyRockName { get; set; }

        public string PetrologyRockDescription { get; set; }

        public string PetrologyMineralsPresent { get; set; }
    }
}