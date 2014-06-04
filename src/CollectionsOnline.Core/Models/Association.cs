namespace CollectionsOnline.Core.Models
{
    public class Association
    {
        public string Type { get; set; }

        public string Name { get; set; }

        public string Date { get; set; }

        public string Comments { get; set; }

        public string Place { get; set; }

        public string PlaceKey { get; set; }

        public GeocodeStatus GeocodeStatus { get; set; }

        public string GeocodePlace { get; set; }

        public string Country { get; set; }
    }
}