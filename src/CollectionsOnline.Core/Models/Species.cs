using System;
using System.Collections.Generic;

namespace CollectionsOnline.Core.Models
{
    public class Species : EmuAggregateRoot
    {
        #region Non-Emu

        public string Summary { get; set; }

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

        public string IdentifyingCharacters { get; set; }

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

        public IList<string> CommonNames { get; set; }

        public IList<string> OtherNames { get; set; }

        public string Phylum { get; set; }

        public string Subphylum { get; set; }

        public string Superclass { get; set; }

        public string Class { get; set; }

        public string Subclass { get; set; }

        public string Superorder { get; set; }

        public string Order { get; set; }

        public string Suborder { get; set; }

        public string Infraorder { get; set; }

        public string Superfamily { get; set; }

        public string Family { get; set; }

        public string Subfamily { get; set; }

        public string Genus { get; set; }

        public string Subgenus { get; set; }

        public string SpeciesName { get; set; }

        public string Subspecies { get; set; }

        public string MoV { get; set; }

        public string Author { get; set; }

        public string HigherClassification { get; set; }

        public string ScientificName { get; set; }

        public IList<string> SpecimenIds { get; set; }

        public IList<Author> Authors { get; set; }

        public IList<Media> Media { get; set; }
    }
}