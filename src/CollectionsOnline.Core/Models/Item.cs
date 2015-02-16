using System;
using System.Collections.Generic;

namespace CollectionsOnline.Core.Models
{
    public class Item : EmuAggregateRoot
    {
        public Item()
        {
            InitializeCollections();
        }

        #region Non-Emu

        public IList<Comment> Comments { get; set; }

        public string Summary { get; set; }

        public string ThumbnailUri { get; set; }

        public string DisplayTitle { get; set; }

        #endregion

        public DateTime DateModified { get; set; }

        public string Category { get; set; }

        public string Discipline { get; set; }

        public string Type { get; set; }

        public string RegistrationNumber { get; set; }

        public IList<string> CollectionNames { get; set; }

        public IList<string> CollectionPlans { get; set; }

        public string PrimaryClassification { get; set; }

        public string SecondaryClassification { get; set; }

        public string TertiaryClassification { get; set; }

        public string ObjectName { get; set; }

        public string ObjectSummary { get; set; }

        public string PhysicalDescription { get; set; }

        public string Inscription { get; set; }

        public IList<Association> Associations { get; set; }

        public IList<string> Keywords { get; set; }

        public string Significance { get; set; }

        public string ModelScale { get; set; }

        public string Shape { get; set; }

        public IList<Dimension> Dimensions { get; set; }

        public string References { get; set; }

        public IList<string> Bibliographies { get; set; }

        public IList<string> ModelNames { get; set; }

        public IList<string> BrandNames { get; set; }

        public IList<string> RelatedItemIds { get; set; }

        public IList<string> RelatedSpecimenIds { get; set; }

        public IList<string> RelatedArticleIds { get; set; }

        public IList<string> RelatedSpeciesIds { get; set; }

        public IList<Media> Media { get; set; }

        public string AcquisitionInformation { get; set; }

        public string Acknowledgement { get; set; }

        public MuseumLocation MuseumLocation { get; set; }

        #region Archeology

        public string ArcheologyContextNumber { get; set; }

        public string ArcheologySite { get; set; }

        public string ArcheologyDescription { get; set; }

        public string ArcheologyDistinguishingMarks { get; set; }

        public string ArcheologyActivity { get; set; }

        public string ArcheologySpecificActivity { get; set; }

        public string ArcheologyDecoration { get; set; }

        public string ArcheologyPattern { get; set; }

        public string ArcheologyColour { get; set; }

        public string ArcheologyMoulding { get; set; }

        public string ArcheologyPlacement { get; set; }

        public string ArcheologyForm { get; set; }

        public string ArcheologyShape { get; set; }

        public string ArcheologyManufactureName { get; set; }

        public string ArcheologyManufactureDate { get; set; }

        public string ArcheologyTechnique { get; set; }

        public string ArcheologyProvenance { get; set; }

        #endregion

        #region Numismatics

        public string NumismaticsDenomination { get; set; }

        public string NumismaticsDateIssued { get; set; }

        public string NumismaticsSeries { get; set; }

        public string NumismaticsMaterial { get; set; }

        public string NumismaticsAxis { get; set; }

        public string NumismaticsEdgeDescription { get; set; }

        public string NumismaticsObverseDescription { get; set; }

        public string NumismaticsReverseDescription { get; set; }

        #endregion

        #region Philately

        public string PhilatelyColour { get; set; }

        public string PhilatelyDenomination { get; set; }

        public string PhilatelyImprint { get; set; }

        public string PhilatelyIssue { get; set; }

        public string PhilatelyDateIssued { get; set; }

        public string PhilatelyForm { get; set; }

        public string PhilatelyOverprint { get; set; }

        public string PhilatelyGibbonsNumber { get; set; }

        #endregion

        #region ISD

        public string IsdFormat { get; set; }

        public string IsdLanguage { get; set; }

        public string IsdDescriptionOfContent { get; set; }

        public string IsdPeopleDepicted { get; set; }

        #endregion

        #region AudioVisual

        public string AudioVisualRecordingDetails { get; set; }

        public IList<string> AudioVisualContentSummaries { get; set; }

        #endregion

        #region Trade Literature

        public string TradeLiteratureNumberofPages { get; set; }

        public string TradeLiteraturePageSizeFormat { get; set; }

        public string TradeLiteratureCoverTitle { get; set; }

        public string TradeLiteraturePrimarySubject { get; set; }

        public string TradeLiteraturePublicationDate { get; set; }

        public string TradeLiteratureIllustrationTypes { get; set; }

        public string TradeLiteraturePrintingTypes { get; set; }

        public IList<string> TradeLiteraturePublicationTypes { get; set; }

        public string TradeLiteraturePrimaryRole { get; set; }

        public string TradeLiteraturePrimaryName { get; set; }

        #endregion

        #region Indigenous Cultures

        public string IndigenousCulturesLocality { get; set; }

        public string IndigenousCulturesRegion { get; set; }

        public string IndigenousCulturesState { get; set; }

        public string IndigenousCulturesCountry { get; set; }

        public IList<string> IndigenousCulturesCulturalGroups { get; set; }

        public string IndigenousCulturesMedium { get; set; }

        public string IndigenousCulturesDescription { get; set; }

        public string IndigenousCulturesPhotographer { get; set; }

        public string IndigenousCulturesAuthor { get; set; }

        public string IndigenousCulturesIllustrator { get; set; }

        public string IndigenousCulturesMaker { get; set; }

        public string IndigenousCulturesDate { get; set; }

        public string IndigenousCulturesCollector { get; set; }

        public string IndigenousCulturesDateCollected { get; set; }

        public string IndigenousCulturesIndividualsIdentified { get; set; }

        public string IndigenousCulturesTitle { get; set; }

        public string IndigenousCulturesSheets { get; set; }

        public string IndigenousCulturesPages { get; set; }

        public string IndigenousCulturesLetterTo { get; set; }

        public string IndigenousCulturesLetterFrom { get; set; }

        #endregion

        #region Artwork

        public string ArtworkMedium { get; set; }

        public string ArtworkTechnique { get; set; }

        public string ArtworkSupport { get; set; }

        public string ArtworkPlateNumber { get; set; }

        public string ArtworkDrawingNumber { get; set; }

        public string ArtworkState { get; set; }

        public string ArtworkPublisher { get; set; }

        public string ArtworkPrimaryInscriptions { get; set; }

        public string ArtworkSecondaryInscriptions { get; set; }

        public string ArtworkTertiaryInscriptions { get; set; }

        public string TypeStatus { get; set; }

        public string IdentifiedBy { get; set; }

        public string DateIdentified { get; set; }

        public string Qualifier { get; set; }

        public QualifierRankType QualifierRank { get; set; }

        public Taxonomy Taxonomy { get; set; }

        public string ScientificName { get; set; }

        #endregion

        private void InitializeCollections()
        {
            Comments = new List<Comment>();
            CollectionNames = new List<string>();
            CollectionPlans = new List<string>();
            Associations = new List<Association>();
            Keywords = new List<string>();
            Dimensions = new List<Dimension>();
            Bibliographies = new List<string>();
            ModelNames = new List<string>();
            BrandNames = new List<string>();
            RelatedItemIds = new List<string>();
            RelatedSpecimenIds = new List<string>();
            RelatedArticleIds = new List<string>();
            RelatedSpeciesIds = new List<string>();
            Media = new List<Media>();
            AudioVisualContentSummaries = new List<string>();
            TradeLiteraturePublicationTypes = new List<string>();
            IndigenousCulturesCulturalGroups = new List<string>();
        }
    }
}