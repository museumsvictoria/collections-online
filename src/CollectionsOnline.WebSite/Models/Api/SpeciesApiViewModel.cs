using System;
using System.Collections.Generic;
using CollectionsOnline.Core.Models;

namespace CollectionsOnline.WebSite.Models.Api
{
    public class SpeciesApiViewModel : AggregateRoot
    {
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

        public string Web { get; set; }

        public IList<string> Plants { get; set; }

        public string FlightStart { get; set; }

        public string FlightEnd { get; set; }

        public IList<string> Depths { get; set; }

        public IList<string> WaterColumnLocations { get; set; }

        public IList<string> RelatedItemIds { get; set; }

        public IList<string> RelatedSpecimenIds { get; set; }

        public IList<Author> Authors { get; set; }

        public IList<Media> Media { get; set; }

        public Taxonomy Taxonomy { get; set; }
    }
}