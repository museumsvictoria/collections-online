using System;
using System.Collections.Generic;

namespace CollectionsOnline.Core.Models
{
    public class Specimen : EmuAggregateRoot
    {
        public Specimen()
        {
            InitializeCollections();
        }

        #region Non-Emu

        public string Summary { get; set; }

        public string ThumbnailUri { get; set; }

        public string DisplayTitle { get; set; }

        public Licence Licence { get; set; }

        #endregion

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

        public IList<Media> Media { get; set; }
        
        public string NumberOfSpecimens { get; set; }

        public string ClutchSize { get; set; }

        public string Sex { get; set; }

        public string StageOrAge { get; set; }

        public IList<Storage> Storages { get; set; }

        public string TypeStatus { get; set; }

        public string IdentifiedBy { get; set; }

        public string DateIdentified { get; set; }

        public string Qualifier { get; set; }

        public QualifierRankType QualifierRank { get; set; }

        public Taxonomy Taxonomy { get; set; }

        public CollectionEvent CollectionEvent { get; set; }

        public CollectionSite CollectionSite { get; set; }

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

        public string SpecimenNature { get; set; }
        
        public string SpecimenForm { get; set; }
        
        public string Preservative { get; set; }
        
        public string FixativeTreatment { get; set; }
        
        public string StorageMedium { get; set; }
        
        public string StorageTemperature { get; set; }
        
        public string DateOfPreparation { get; set; }
        
        public string TissueSampledFrom { get; set; }

        private void InitializeCollections()
        {
            CollectionNames = new List<string>();
            CollectingAreas = new List<string>();
            Classifications = new List<string>();
            Associations = new List<Association>();
            RelatedItemIds = new List<string>();
            RelatedSpecimenIds = new List<string>();
            RelatedArticleIds = new List<string>();
            RelatedSpeciesIds = new List<string>();
            Keywords = new List<string>();
            Media = new List<Media>();
            Storages = new List<Storage>();
        }
    }
}