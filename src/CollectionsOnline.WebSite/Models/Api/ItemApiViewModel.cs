using System;
using System.Collections.Generic;
using CollectionsOnline.Core.Models;

namespace CollectionsOnline.WebSite.Models.Api
{
    public class ItemApiViewModel : AggregateRoot
    {
        public string RecordType { get; set; }

        public IList<CommentApiViewModel> Comments { get; set; }

        public LicenceApiViewModel Licence { get; set; }

        public DateTime DateModified { get; set; }

        public string Category { get; set; }

        public string Discipline { get; set; }

        public string Type { get; set; }

        public string RegistrationNumber { get; set; }

        public IList<string> CollectionNames { get; set; }

        public IList<string> CollectingAreas { get; set; }

        public IList<string> Classifications { get; set; }

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

        public IList<Brand> Brands { get; set; }

        public IList<string> RelatedItemIds { get; set; }

        public IList<string> RelatedSpecimenIds { get; set; }

        public IList<string> RelatedArticleIds { get; set; }

        public IList<string> RelatedSpeciesIds { get; set; }

        public IList<MediaApiViewModel> Media { get; set; }

        public string AcquisitionInformation { get; set; }

        public string Acknowledgement { get; set; }

        public MuseumLocation MuseumLocation { get; set; }

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

        public string NumismaticsDenomination { get; set; }

        public string NumismaticsDateIssued { get; set; }

        public string NumismaticsSeries { get; set; }

        public string NumismaticsMaterial { get; set; }

        public string NumismaticsAxis { get; set; }

        public string NumismaticsEdgeDescription { get; set; }

        public string NumismaticsObverseDescription { get; set; }

        public string NumismaticsReverseDescription { get; set; }

        public string PhilatelyColour { get; set; }

        public string PhilatelyDenomination { get; set; }

        public string PhilatelyImprint { get; set; }

        public string PhilatelyIssue { get; set; }

        public string PhilatelyDateIssued { get; set; }

        public string PhilatelyForm { get; set; }

        public string PhilatelyOverprint { get; set; }

        public string PhilatelyGibbonsNumber { get; set; }

        public string IsdFormat { get; set; }

        public string IsdLanguage { get; set; }

        public string IsdDescriptionOfContent { get; set; }

        public string IsdPeopleDepicted { get; set; }

        public string AudioVisualRecordingDetails { get; set; }

        public IList<string> AudioVisualContentSummaries { get; set; }

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

        public IList<string> IndigenousCulturesLocalities { get; set; }

        public IList<string> IndigenousCulturesCulturalGroups { get; set; }

        public string IndigenousCulturesMedium { get; set; }

        public string IndigenousCulturesDescription { get; set; }

        public string IndigenousCulturesLocalName { get; set; }

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

        public string QualifierRank { get; set; }

        public TaxonomyApiViewModel Taxonomy { get; set; }
    }
}