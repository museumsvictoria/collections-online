namespace CollectionsOnline.WebSite.Models
{
    public class EmuAggregateRootCsvModel
    {
        public string Id { get; set; }

        public string RecordType { get; set; }

        public string RegistrationNumber { get; set; }

        #region Item

        public string ObjectName { get; set; }

        public string ObjectSummary { get; set; }

        public string IsdDescriptionOfContent { get; set; }

        public string TradeLiteratureCoverTitle { get; set; }

        public string PhysicalDescription { get; set; }

        public string NumismaticsObverseDescription { get; set; }

        public string NumismaticsReverseDescription { get; set; }

        public string ArcheologyDescription { get; set; }

        public string TradeLiteraturePrimaryRole { get; set; }

        public string TradeLiteraturePrimaryName { get; set; }

        public string ArcheologyContextNumber { get; set; }

        public string ArcheologySite { get; set; }

        public string Associations { get; set; } /* FLATTEN THIS PLOISE */

        public string ArcheologyManufactureName { get; set; }

        public string ArcheologyManufactureDate { get; set; }

        public string Dimensions { get; set; } /* FLATTEN THIS PLOISE */

        #endregion 
        

        public string DisplayTitle { get; set; }

        public string SubDisplayTitle { get; set; }

        public string Summary { get; set; }

        public string ThumbnailUri { get; set; }

        
    }
}   