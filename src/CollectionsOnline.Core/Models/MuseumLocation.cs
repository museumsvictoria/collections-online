using System;
using CollectionsOnline.Core.Extensions;

namespace CollectionsOnline.Core.Models
{
    public class MuseumLocation
    {
        public string Venue { get; set; }

        public string Gallery { get; set; }

        public string DisplayLocation {
            get
            {
                if (Gallery.Contains("Bunjilaka", StringComparison.OrdinalIgnoreCase))
                    return "Bunjilaka";
                if (string.Equals(Gallery, "Discovery Centre", StringComparison.OrdinalIgnoreCase))
                    return "Discovery Centre";
                if (string.Equals(Gallery, "Immigration Discovery Centre", StringComparison.OrdinalIgnoreCase))
                    return "Immigration Discovery Centre";
                if (string.Equals(Venue, "Melbourne Museum", StringComparison.OrdinalIgnoreCase))
                    return "Melbourne Museum";
                if (string.Equals(Venue, "Immigration Museum", StringComparison.OrdinalIgnoreCase))
                    return "Immigration Museum";
                if (string.Equals(Venue, "Royal Exhibition Building", StringComparison.OrdinalIgnoreCase))
                    return "Royal Exhibition Building";
                if (string.Equals(Venue, "Scienceworks", StringComparison.OrdinalIgnoreCase))
                    return "Scienceworks";

                return null;
            }
        }
    }
}