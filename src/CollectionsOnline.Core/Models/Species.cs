using System;
using System.Collections.Generic;
using System.Linq;

namespace CollectionsOnline.Core.Models
{
    public class Species : EmuAggregateRoot
    {
        #region Non-Emu

        public string Summary { get; set; }

        public int Quality
        {
            get
            {
                var qualityCount = 0;

                if (!string.IsNullOrWhiteSpace(AnimalType))
                    qualityCount += 1;
                if (!string.IsNullOrWhiteSpace(AnimalSubType))
                    qualityCount += 1;
                if (Colours.Any())
                    qualityCount += 1;
                if (!string.IsNullOrWhiteSpace(MaximumSize))
                    qualityCount += 1;
                if (Habitats.Any())
                    qualityCount += 1;
                if (WhereToLook.Any())
                    qualityCount += 1;
                if (WhenActive.Any())
                    qualityCount += 1;
                if (NationalParks.Any())
                    qualityCount += 1;
                if (!string.IsNullOrWhiteSpace(Diet))
                    qualityCount += 1;
                if (DietCategories.Any())
                    qualityCount += 1;
                if (!string.IsNullOrWhiteSpace(FastFact))
                    qualityCount += 1;
                if (!string.IsNullOrWhiteSpace(Habitat))
                    qualityCount += 1;
                if (!string.IsNullOrWhiteSpace(Distribution))
                    qualityCount += 1;
                if (!string.IsNullOrWhiteSpace(Biology))
                    qualityCount += 1;
                if (!string.IsNullOrWhiteSpace(IdentifyingCharacters))
                    qualityCount += 1;
                if (!string.IsNullOrWhiteSpace(BriefId))
                    qualityCount += 1;
                if (!string.IsNullOrWhiteSpace(Hazards))
                    qualityCount += 1;
                if (!string.IsNullOrWhiteSpace(Endemicity))
                    qualityCount += 1;
                if (!string.IsNullOrWhiteSpace(Commercial))
                    qualityCount += 1;
                if (ConservationStatuses.Any())
                    qualityCount += 1;
                if (!string.IsNullOrWhiteSpace(ScientificDiagnosis))
                    qualityCount += 1;
                if (!string.IsNullOrWhiteSpace(Web))
                    qualityCount += 1;
                if (Plants.Any())
                    qualityCount += 1;
                if (!string.IsNullOrWhiteSpace(FlightStart) || !string.IsNullOrWhiteSpace(FlightEnd))
                    qualityCount += 1;
                if (Depths.Any())
                    qualityCount += 1;
                if (WaterColumnLocations.Any())
                    qualityCount += 1;
                if (CommonNames.Any())
                    qualityCount += 1;
                if (OtherNames.Any())
                    qualityCount += 1;
                if (!string.IsNullOrWhiteSpace(SpeciesName))
                    qualityCount += 1;
                if (!string.IsNullOrWhiteSpace(Author))
                    qualityCount += 1;
                if (!string.IsNullOrWhiteSpace(ScientificName))
                    qualityCount += 1;
                if (SpecimenIds.Any())
                    qualityCount += SpecimenIds.Count;
                if (Authors.Any())
                    qualityCount += Authors.Count;
                if (Media.Any())
                    qualityCount += Media.Count * 2;

                return qualityCount;
            }
        }

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