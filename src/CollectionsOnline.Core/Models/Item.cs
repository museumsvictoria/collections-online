using System;
using System.Collections.Generic;
using System.Linq;

namespace CollectionsOnline.Core.Models
{
    public class Item : EmuAggregateRoot
    {
        #region Non-Emu

        public IList<Comment> Comments { get; set; }

        public string Summary { get; set; }

        public IList<string> AssociatedDates { get; set; }

        #endregion

        public DateTime DateModified { get; set; }

        public string Category { get; set; }

        public string Discipline { get; set; }

        public string Type { get; set; }

        public string RegistrationNumber { get; set; }

        public IList<string> CollectionNames { get; set; }

        public string PrimaryClassification { get; set; }

        public string SecondaryClassification { get; set; }

        public string TertiaryClassification { get; set; }

        public string Name { get; set; }

        public string ObjectSummary { get; set; }

        public string Description { get; set; }

        public string Inscription { get; set; }

        public IList<Association> Associations { get; set; }

        public IList<string> Tags { get; set; }

        public string Significance { get; set; }

        public string ModelScale { get; set; }

        public string Shape { get; set; }

        public IList<string> Dimensions { get; set; }

        public string References { get; set; }

        public IList<string> Bibliographies { get; set; }

        public string ModelNames { get; set; }

        public string BrandNames { get; set; }

        public IList<string> RelatedItemIds { get; set; }

        public IList<Media> Media { get; set; }

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

        public string AudioVisualContentSummary { get; set; }

        #endregion

        #region Trade Literature

        public string TradeLiteratureNumberofPages { get; set; }

        public string TradeLiteraturePageSizeFormat { get; set; }

        public string TradeLiteratureCoverTitle { get; set; }

        public string TradeLiteraturePrimarySubject { get; set; }

        public string TradeLiteraturePublicationDate { get; set; }

        public string TradeLiteratureIllustrationTypes { get; set; }

        public string TradeLiteraturePrintingTypes { get; set; }

        public string TradeLiteraturePublicationTypes { get; set; }

        public string TradeLiteraturePrimaryRole { get; set; }

        public string TradeLiteraturePrimaryName { get; set; }

        #endregion

        #region Indigenous Cultures

        public string IndigenousCulturesLocalName { get; set; }

        public string IndigenousCulturesLocality { get; set; }

        public string IndigenousCulturesCulturalGroups { get; set; }

        public string IndigenousCulturesDescription { get; set; }

        public string IndigenousCulturesDateMade { get; set; }

        public string IndigenousCulturesDateCollected { get; set; }

        public string IndigenousCulturesCaption { get; set; }

        public string IndigenousCulturesIndividualsIdentified { get; set; }

        public string IndigenousCulturesTitle { get; set; }

        public string IndigenousCulturesSheets { get; set; }

        public string IndigenousCulturesPages { get; set; }

        public string IndigenousCulturesLetterTo { get; set; }

        public string IndigenousCulturesLetterFrom { get; set; }

        public string IndigenousCulturesIndividualsMentioned { get; set; }

        public string IndigenousCulturesLocalitiesMentioned { get; set; }

        public string IndigenousCulturesStateProvinceMentioned { get; set; }

        public string IndigenousCulturesRegionsMentioned { get; set; }

        public string IndigenousCulturesCountryMentioned { get; set; }

        public string IndigenousCulturesGroupNames { get; set; }

        public string IndigenousCulturesNamesMentioned { get; set; }        

        #endregion
    }
}