using System;
using System.Collections.Generic;

namespace CollectionsOnline.Core.DomainModels
{
    public class Specimen : DomainModel
    {
        public DateTime DateModified { get; set; }

        public ICollection<Media> Media { get; set; }

        #region Record-level Terms

        public string DctermsType { get; set; }

        public string DctermsModified 
        {
            get { return DateModified.ToString("s"); }
        }

        public string DctermsLanguage { get; set; }

        public string DctermsRights { get; set; }

        public string DctermsRightsHolder { get; set; }

        public string InstitutionId { get; set; }

        public string CollectionId { get; set; }
        
        public string DatasetId { get; set; }

        public string InstitutionCode { get; set; }

        public string CollectionCode { get; set; }

        public string DatasetName { get; set; }

        public string OwnerInstitutionCode { get; set; }

        public string BasisOfRecord { get; set; }

        #endregion

        #region Occurrence

        public string OccurrenceId { get; set; }

        public string CatalogNumber { get; set; }

        public string RecordedBy { get; set; }

        public string IndividualCount { get; set; }

        public string Sex { get; set; }

        public string LifeStage { get; set; }

        public string OccurrenceStatus { get; set; }

        public string Preparations { get; set; }

        public string OtherCatalogNumbers { get; set; }

        public string AssociatedMedia { get; set; }

        #endregion

        #region Event

        public string EventId { get; set; }

        public string SamplingProtocol { get; set; }

        public string EventDate { get; set; }

        public string EventTime { get; set; }

        public string Year { get; set; }

        public string Month { get; set; }

        public string Day { get; set; }

        public string VerbatimEventDate { get; set; }

        public string FieldNumber { get; set; }

        #endregion

        #region Location

        public string LocationID { get; set; }

        public string HigherGeography { get; set; }

        public string Continent { get; set; }

        public string WaterBody { get; set; }

        public string Country { get; set; }

        public string StateProvince { get; set; }

        public string County { get; set; }

        public string Municipality { get; set; }

        public string Locality { get; set; }

        public string VerbatimLocality { get; set; }

        public string MinimumElevationInMeters { get; set; }

        public string MaximumElevationInMeters { get; set; }

        public string MinimumDepthInMeters { get; set; }

        public string MaximumDepthInMeters { get; set; }

        public string DecimalLatitude { get; set; }

        public string DecimalLongitude { get; set; }

        public string GeodeticDatum { get; set; }

        public string GeoreferencedBy { get; set; }

        public string GeoreferencedDate { get; set; }

        public string GeoreferenceProtocol { get; set; }

        public string GeoreferenceSources { get; set; }

        #endregion

        #region Identification

        public string IdentifiedBy { get; set; }

        public string DateIdentified { get; set; }

        public string IdentificationRemarks { get; set; }

        public string IdentificationQualifier { get; set; }

        public string TypeStatus { get; set; }

        #endregion

        #region Taxon

        public string ScientificName { get; set; }

        public string AcceptedNameUsage { get; set; }

        public string OriginalNameUsage { get; set; }

        public string HigherClassification { get; set; }

        public string Kingdom { get; set; }

        public string Phylum { get; set; }

        public string Class { get; set; }

        public string Order { get; set; }

        public string Family { get; set; }

        public string Genus { get; set; }

        public string Subgenus { get; set; }

        public string SpecificEpithet { get; set; }

        public string InfraspecificEpithet { get; set; }

        public string TaxonRank { get; set; }

        public string ScientificNameAuthorship { get; set; }

        public string VernacularName { get; set; }

        public string NomenclaturalCode { get; set; }

        #endregion

        public Specimen(string irn)
        {
            Id = "specimens/" + irn;
        }
    }
}