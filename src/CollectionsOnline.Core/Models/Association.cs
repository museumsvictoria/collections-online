using CollectionsOnline.Core.Extensions;

namespace CollectionsOnline.Core.Models
{
    public class Association
    {
        public string Type { get; set; }

        public string Name { get; set; }

        public string Country { get; set; }

        public string State { get; set; }

        public string Region { get; set; }

        public string Locality { get; set; }

        public string Street { get; set; }

        public string Date { get; set; }

        public string Notes { get; set; }

        // TODO: fix formatting
        public string Summary
        {
            get
            {
                return new[]
                {
                    Street,
                    Locality,
                    State,
                    Region,
                    Country,
                    Date,
                    Notes
                }.Concatenate(", ");
            }
        }
    }
}