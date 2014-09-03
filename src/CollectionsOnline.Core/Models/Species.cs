using System;
using System.Collections.Generic;

namespace CollectionsOnline.Core.Models
{
    public class Species : EmuAggregateRoot
    {
        public Species()
        {
            InitializeCollections();
        }

        #region Non-Emu

        public string Summary { get; set; }

        public string ThumbnailUri { get; set; }

        #endregion

        public DateTime DateModified { get; set; }

        public string AnimalType { get; set; }

        public string AnimalSubType { get; set; }

        public IList<string> Colours { get; set; }

        public string MaximumSize { get; set; }

        public IList<string> Habitats { get; set; }

        public IList<string> WhereToLook { get; set; }

        public IList<string> WhenActive { get; set; }

        public IList<string> NationalParks { get; set; }

        public string Diet { get; set; }

        public IList<string> DietCategories { get; set; }

        public string FastFact { get; set; }

        public string Habitat { get; set; }

        public string Distribution { get; set; }

        public string Biology { get; set; }

        public string GeneralDescription { get; set; }

        public string BriefId { get; set; }

        public string Hazards { get; set; }

        public string Endemicity { get; set; }

        public string Commercial { get; set; }

        public IList<string> ConservationStatuses { get; set; }

        public string ScientificDiagnosis { get; set; }

        public string Web { get; set; }

        public IList<string> Plants { get; set; }

        public string FlightStart { get; set; }

        public string FlightEnd { get; set; }

        public IList<string> Depths { get; set; }

        public IList<string> WaterColumnLocations { get; set; }

        public IList<string> SpecimenIds { get; set; }

        public IList<Author> Authors { get; set; }

        public IList<Media> Media { get; set; }

        public Taxonomy Taxonomy { get; set; }

        public string ScientificName { get; set; }

        private void InitializeCollections()
        {
            Colours = new List<string>();
            Habitats = new List<string>();
            WhereToLook = new List<string>();
            WhenActive = new List<string>();
            NationalParks = new List<string>();
            DietCategories = new List<string>();
            ConservationStatuses = new List<string>();
            Plants = new List<string>();
            Depths = new List<string>();
            WaterColumnLocations = new List<string>();
            SpecimenIds = new List<string>();
            Authors = new List<Author>();
            Media = new List<Media>();
        }
    }
}