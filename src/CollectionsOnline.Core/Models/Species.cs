using System;
using System.Collections.Generic;

namespace CollectionsOnline.Core.Models
{
    public class Species : EmuAggregateRoot
    {
        public DateTime DateModified { get; set; }

        public string AnimalType { get; set; }

        public string AnimalSubType { get; set; }

        public ICollection<string> Colours { get; set; }

        public string MaximumSize { get; set; }

        public ICollection<string> Habitats { get; set; }

        public ICollection<string> WhereToLook { get; set; }

        public ICollection<string> WhenActive { get; set; }

        public ICollection<string> NationalParks { get; set; }

        public string Diet { get; set; }

        public ICollection<string> DietCategories { get; set; }

        public string FastFact { get; set; }

        public string Habitat { get; set; }

        public string Distribution { get; set; }

        public string Biology { get; set; }

        public string IdentifyingCharacters { get; set; }

        public string BriefId { get; set; }

        public string Hazards { get; set; }

        public string Endemicity { get; set; }

        public string Commercial { get; set; }

        public ICollection<string> ConservationStatuses { get; set; }

        public string ScientificDiagnosis { get; set; }

        public string Web { get; set; }

        public ICollection<string> Plants { get; set; }

        public string FlightStart { get; set; }

        public string FlightEnd { get; set; }

        public ICollection<string> Depths { get; set; }

        public ICollection<string> WaterColumnLocations { get; set; }

        public ICollection<string> CommonNames { get; set; }

        public ICollection<string> OtherNames { get; set; }

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

        public ICollection<string> SpecimenIds { get; set; }

        public ICollection<Author> Authors { get; set; }

        public ICollection<Media> Media { get; set; }
    }
}