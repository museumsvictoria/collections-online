namespace CollectionsOnline.Core.Models
{
    public class Association
    {
        public string Type { get; set; }

        public string Name { get; set; }

        public string Date { get; set; }

        public string Comments { get; set; }

        public string StreetAddress { get; set; }

        public string Locality { get; set; }

        public string Region { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string Place { get; set; }

        public string PlaceKey { get; set; }

        public string GeocodedPlace { get; set; }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public GeocodeStatus GeocodeStatus { get; set; }
    }
}