using System;
using System.Collections.Generic;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Core.Models;

namespace CollectionsOnline.Core.Config
{
    public static class Constants
    {
        public static string ApplicationId = "applications/collectionsonline";

        public static int DataBatchSize = 10;

        public static int CachedDataBatchSize = 10000;

        public static int PagingPageSizeDefault = 30;

        public static int PagingPageSizeMax = 120;

        public static int SuggestionsMinResultsSize = 15;

        public static int SummaryMaxChars = 200;

        public static string ImuItemQueryString = "Collections Online - Humanities";

        public static string ImuSpeciesQueryString = "Website - Species profile";

        public static string ImuSpecimenQueryString = "Website  Atlas of Living Australia";

        public static string ImuStoryQueryString = "Website - History & Technology Collections";

        public static string ImuMultimediaQueryString = "Website - Collections Online";

        public static TimeSpan AggressiveCacheTimeSpan = TimeSpan.FromHours(1);

        public static TimeSpan ImuOfflineTimeSpan = new TimeSpan(19, 00, 0); // 7:00pm

        public static Dictionary<Tuple<string, string, string, string>, MuseumLocation> MuseumLocations = new Dictionary
            <Tuple<string, string, string, string>, MuseumLocation>(new OrdinalIgnoreCaseTupleComparer())
        {
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Ground Level", "West", "Administration"),
                new MuseumLocation { Gallery = "Administration", MuseumVenue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Level One", "West", "Balcony"),
                new MuseumLocation { Gallery = "Balcony", MuseumVenue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Level One", "Central", "Balcony"),
                new MuseumLocation { Gallery = "Balcony", MuseumVenue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Level One", "East", "Balcony"),
                new MuseumLocation { Gallery = "Balcony", MuseumVenue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Ground Level", "West", "Gallery 3"),
                new MuseumLocation { Gallery = "Children's Museum", MuseumVenue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Ground Level", "East", "Gallery 8"),
                new MuseumLocation { Gallery = "Bunjilaka", MuseumVenue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Lower Ground Level", "Central", null),
                new MuseumLocation { Gallery = "Public Spaces Lower Ground Floor", MuseumVenue = "Melbourne Museum" }
            }
        };
    }
}