namespace CollectionsOnline.WebSite.Models
{
    public class EmuAggregateRootCsvModel
    {
        public string Id { get; set; }

        public string RecordType { get; set; }

        public string RegistrationNumber { get; set; }

        public string DisplayTitle { get; set; }

        public string Licence { get; set; }

        public string Authors { get; set; }

        #region Article

        public string Contributors { get; set; }

        public string ContentSummary { get; set; }

        public string ContentText { get; set; }

        public string ArticleTypes { get; set; }

        #endregion

        #region Item

        public string ObjectName { get; set; }

        public string ObjectSummary { get; set; }

        public string IsdDescriptionOfContent { get; set; }

        public string TradeLiteratureCoverTitle { get; set; }

        public string PhysicalDescription { get; set; }

        public string NumismaticsObverseDescription { get; set; }

        public string NumismaticsReverseDescription { get; set; }

        public string ArcheologyDescription { get; set; }

        public string TradeLiteraturePrimaryRoleAndName { get; set; }

        public string ArcheologyContextNumber { get; set; }

        public string ArcheologySite { get; set; }

        public string Associations { get; set; }

        public string ArcheologyManufactureName { get; set; }

        public string ArcheologyManufactureDate { get; set; }

        public string Dimensions { get; set; }

        public string IndigenousCulturesDescription { get; set; }

        public string IndigenousCulturesLocalName { get; set; }

        public string IndigenousCulturesCulturalGroups { get; set; }

        public string IndigenousCulturesLocalities { get; set; }

        public string IndigenousCulturesPhotographer { get; set; }

        public string IndigenousCulturesAuthor { get; set; }

        public string IndigenousCulturesIllustrator { get; set; }

        public string IndigenousCulturesMaker { get; set; }

        public string IndigenousCulturesDate { get; set; }

        public string IndigenousCulturesCollector { get; set; }

        public string IndigenousCulturesDateCollected { get; set; }

        #endregion

        #region Species

        public string AnimalType { get; set; }

        public string AnimalSubType { get; set; }

        public string Habitat { get; set; }

        public string Distribution { get; set; }

        public string Biology { get; set; }

        public string GeneralDescription { get; set; }

        public string BriefId { get; set; }

        #endregion

        #region Specimen/Items

        public string Category { get; set; }

        public string ScientificGroup { get; set; }

        public string TypeStatus { get; set; }

        public string Qualifier { get; set; }

        public string QualifierRank { get; set; }

        public string NumberOfSpecimens { get; set; }

        public string ClutchSize { get; set; }

        public string Sex { get; set; }

        public string StageOrAge { get; set; }

        public string Storage { get; set; }

        public string IdentifiedBy { get; set; }

        public string DateIdentified { get; set; }

        public string CollectionEventExpeditionName { get; set; }

        public string CollectionEventCollectionEventCode { get; set; }

        public string CollectionEventSamplingMethod { get; set; }

        public string CollectionEventDateVisitedFrom { get; set; }

        public string CollectionEventDateVisitedTo { get; set; }

        public string CollectionEventDepthTo { get; set; }

        public string CollectionEventDepthFrom { get; set; }

        public string CollectionSiteSiteCode { get; set; }

        public string CollectionSiteOcean { get; set; }

        public string CollectionSiteContinent { get; set; }

        public string CollectionSiteCountry { get; set; }

        public string CollectionSiteState { get; set; }

        public string CollectionSiteDistrict { get; set; }

        public string CollectionSiteTown { get; set; }

        public string CollectionSiteNearestNamedPlace { get; set; }

        public string CollectionSitePreciseLocation { get; set; }

        public string CollectionSiteMinimumElevation { get; set; }

        public string CollectionSiteMaximumElevation { get; set; }

        public string CollectionSiteLatitude { get; set; }

        public string CollectionSiteLongitude { get; set; }

        public string CollectionSiteGeodeticDatum { get; set; }

        public string CollectionSiteSiteRadius { get; set; }

        public string CollectionSiteGeoreferenceBy { get; set; }

        public string CollectionSiteGeoreferenceDate { get; set; }

        public string CollectionSiteGeoreferenceProtocol { get; set; }

        public string CollectionSiteGeoreferenceSource { get; set; }

        #endregion

        #region Taxonomy

        public string TaxonomyKingdom { get; set; }

        public string TaxonomyPhylum { get; set; }

        public string TaxonomySubphylum { get; set; }

        public string TaxonomySuperclass { get; set; }

        public string TaxonomyClass { get; set; }

        public string TaxonomySubclass { get; set; }

        public string TaxonomySuperorder { get; set; }

        public string TaxonomyOrder { get; set; }

        public string TaxonomySuborder { get; set; }

        public string TaxonomyInfraorder { get; set; }

        public string TaxonomySuperfamily { get; set; }

        public string TaxonomyFamily { get; set; }

        public string TaxonomySubfamily { get; set; }

        public string TaxonomyGenus { get; set; }

        public string TaxonomySubgenus { get; set; }

        public string TaxonomySpecies { get; set; }

        public string TaxonomySubspecies { get; set; }

        public string TaxonomyAuthor { get; set; }

        public string TaxonomyCommonName { get; set; }

        #endregion
    }
}   