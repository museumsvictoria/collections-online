using System.Collections.Generic;

namespace CollectionsOnline.Core.Models
{
    public class CollectionSite
    {
        public CollectionSite()
        {
            InitializeCollections();
        }

        public long Irn { get; set; }

        public string SiteCode { get; set; }

        public string Ocean { get; set; }

        public string Continent { get; set; }

        public string Country { get; set; }

        public string State { get; set; }

        public string District { get; set; }

        public string Town { get; set; }

        public string NearestNamedPlace { get; set; }

        public string PreciseLocation { get; set; }

        public string MinimumElevation { get; set; }

        public string MaximumElevation { get; set; }

        public IList<string> Latitudes { get; set; }

        public IList<string> Longitudes { get; set; }

        public string GeodeticDatum { get; set; }

        public string SiteRadius { get; set; }

        public string GeoreferenceBy { get; set; }

        public string GeoreferenceDate { get; set; }

        public string GeoreferenceProtocol { get; set; }

        public string GeoreferenceSource { get; set; }

        public string GeologyEra { get; set; }

        public string GeologyPeriod { get; set; }

        public string GeologyEpoch { get; set; }

        public string GeologyStage { get; set; }

        public string GeologyGroup { get; set; }

        public string GeologyFormation { get; set; }

        public string GeologyMember { get; set; }

        public string GeologyRockType { get; set; }

        private void InitializeCollections()
        {
            Latitudes = new List<string>();
            Longitudes = new List<string>();
        }
    }
}