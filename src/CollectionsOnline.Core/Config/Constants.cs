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

        public static string ImuSpecimenQueryString = "Website - Atlas of Living Australia";

        public static string ImuStoryQueryString = "Website - History & Technology Collections";

        public static string ImuMultimediaQueryString = "Website - Collections Online";

        public static TimeSpan AggressiveCacheTimeSpan = TimeSpan.FromHours(1);

        public static TimeSpan ImuOfflineTimeSpan = new TimeSpan(19, 00, 0); // 7:00pm

        public static Dictionary<Tuple<string, string, string, string>, MuseumLocation> MuseumLocations = new Dictionary
            <Tuple<string, string, string, string>, MuseumLocation>(new OrdinalIgnoreCaseTupleComparer())
        {
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Ground Level", "Central", "Foyer"),
                new MuseumLocation { Gallery = "Main Entrance", MuseumVenue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Ground Level", "West", "Administration"),
                new MuseumLocation { Gallery = "Administration", MuseumVenue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Ground Level", "West", "Vestibule"),
                new MuseumLocation { Gallery = "Public Spaces Ground Floor", MuseumVenue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Ground Level", "West", "Walk"),
                new MuseumLocation { Gallery = "Public Spaces Ground Floor", MuseumVenue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Ground Level", "Central", "Walk"),
                new MuseumLocation { Gallery = "Public Spaces Ground Floor", MuseumVenue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Ground Level", "East", "Walk"),
                new MuseumLocation { Gallery = "Public Spaces Ground Floor", MuseumVenue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Ground Level", "East", "Corridor"),
                new MuseumLocation { Gallery = "Public Spaces Ground Floor", MuseumVenue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Ground Level", "West", "Corridor"),
                new MuseumLocation { Gallery = "Public Spaces Ground Floor", MuseumVenue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Ground Level", "West", "Gallery 3"),
                new MuseumLocation { Gallery = "Children's Museum", MuseumVenue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Ground Level", "West", "Gallery 4"),
                new MuseumLocation { Gallery = "Science & Life Gallery", MuseumVenue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Ground Level", "West", "Gallery 5"),
                new MuseumLocation { Gallery = "Evolution Gallery", MuseumVenue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Level One", "West", "Gallery 11"),
                new MuseumLocation { Gallery = "Evolution Gallery", MuseumVenue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Ground Level", "Central", "Gallery 6"),
                new MuseumLocation { Gallery = "Forest Gallery", MuseumVenue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Ground Level", "East", "Gallery 8"),
                new MuseumLocation { Gallery = "Bunjilaka", MuseumVenue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Ground Level", "East", "Gallery 9"),
                new MuseumLocation { Gallery = "Kalaya & Milarri Gardens", MuseumVenue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Level One", "East", "Gallery 7"),
                new MuseumLocation { Gallery = "Te Pasifika", MuseumVenue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Ground Level", "East", "Gallery 7"),
                new MuseumLocation { Gallery = "Te Pasifika", MuseumVenue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Level One", "East", "Gallery 12"),
                new MuseumLocation { Gallery = "Melbourne Gallery", MuseumVenue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Level One", "West", "Balcony"),
                new MuseumLocation { Gallery = "Upper Level Balcony", MuseumVenue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Level One", "Central", "Balcony"),
                new MuseumLocation { Gallery = "Upper Level Balcony", MuseumVenue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Level One", "East", "Balcony"),
                new MuseumLocation { Gallery = "Upper Level Balcony", MuseumVenue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Level One", "West", "Gallery 10"),
                new MuseumLocation { Gallery = "Human Mind and Body Gallery", MuseumVenue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Lower Ground", "West", "Gallery 1"),
                new MuseumLocation { Gallery = "Discovery Centre", MuseumVenue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Lower Ground", "Central", string.Empty),
                new MuseumLocation { Gallery = "Public Spaces Lower Level", MuseumVenue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Lower Ground", "West", string.Empty),
                new MuseumLocation { Gallery = "Public Spaces Lower Level", MuseumVenue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Lower Ground", "East", string.Empty),
                new MuseumLocation { Gallery = "Public Spaces Lower Level", MuseumVenue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Lower Ground", "East", "Gallery 2"),
                new MuseumLocation { Gallery = "Touring Hall", MuseumVenue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("ROYAL EXHIBITION BUILDING (MvCIS)", "North", "Ground", string.Empty),
                new MuseumLocation { Gallery = "Ground Level", MuseumVenue = "Royal Exhibition Building" }
            },
            {
                new Tuple<string, string, string, string>("ROYAL EXHIBITION BUILDING (MvCIS)", "South", "Ground", string.Empty),
                new MuseumLocation { Gallery = "Ground Level", MuseumVenue = "Royal Exhibition Building" }
            },
            {
                new Tuple<string, string, string, string>("ROYAL EXHIBITION BUILDING (MvCIS)", "East", "Ground", string.Empty),
                new MuseumLocation { Gallery = "Ground Level", MuseumVenue = "Royal Exhibition Building" }
            },
            {
                new Tuple<string, string, string, string>("ROYAL EXHIBITION BUILDING (MvCIS)", "West", "Ground", string.Empty),
                new MuseumLocation { Gallery = "Ground Level", MuseumVenue = "Royal Exhibition Building" }
            },
            {
                new Tuple<string, string, string, string>("ROYAL EXHIBITION BUILDING (MvCIS)", "Central", "Ground", string.Empty),
                new MuseumLocation { Gallery = "Ground Level", MuseumVenue = "Royal Exhibition Building" }
            },
            {
                new Tuple<string, string, string, string>("ROYAL EXHIBITION BUILDING (MvCIS)", "North", "Balcony", string.Empty),
                new MuseumLocation { Gallery = "Mezzanine Level", MuseumVenue = "Royal Exhibition Building" }
            },
            {
                new Tuple<string, string, string, string>("ROYAL EXHIBITION BUILDING (MvCIS)", "South", "Balcony", string.Empty),
                new MuseumLocation { Gallery = "Mezzanine Level", MuseumVenue = "Royal Exhibition Building" }
            },
            {
                new Tuple<string, string, string, string>("ROYAL EXHIBITION BUILDING (MvCIS)", "East", "Balcony", string.Empty),
                new MuseumLocation { Gallery = "Mezzanine Level", MuseumVenue = "Royal Exhibition Building" }
            },
            {
                new Tuple<string, string, string, string>("ROYAL EXHIBITION BUILDING (MvCIS)", "West", "Balcony", string.Empty),
                new MuseumLocation { Gallery = "Mezzanine Level", MuseumVenue = "Royal Exhibition Building" }
            },
            {
                new Tuple<string, string, string, string>("SCIENCEWORKS (MvCIS)", "Ground Level", "Central", string.Empty),
                new MuseumLocation { Gallery = "Drum", MuseumVenue = "Scienceworks" }
            },
            {
                new Tuple<string, string, string, string>("SCIENCEWORKS (MvCIS)", "Level 1", "Central", string.Empty),
                new MuseumLocation { Gallery = "Drum", MuseumVenue = "Scienceworks" }
            },
            {
                new Tuple<string, string, string, string>("SCIENCEWORKS (MvCIS)", "Ground Level", "South", "Gallery"),
                new MuseumLocation { Gallery = "Think Ahead", MuseumVenue = "Scienceworks" }
            },
            {
                new Tuple<string, string, string, string>("SCIENCEWORKS (MvCIS)", "Level 1", "South", "Gallery"),
                new MuseumLocation { Gallery = "Nitty Gritty Super City", MuseumVenue = "Scienceworks" }
            },
            {
                new Tuple<string, string, string, string>("SCIENCEWORKS (MvCIS)", "Planetarium", string.Empty, string.Empty),
                new MuseumLocation { Gallery = "Planetarium", MuseumVenue = "Scienceworks" }
            },
            {
                new Tuple<string, string, string, string>("SCIENCEWORKS (MvCIS)", "Ground Level", "South", "Gallery"),
                new MuseumLocation { Gallery = "Sportsworks", MuseumVenue = "Scienceworks" }
            },
            {
                new Tuple<string, string, string, string>("SCIENCEWORKS (MvCIS)", "Ground Level", "North", "Gallery"),
                new MuseumLocation { Gallery = "Touring Hall", MuseumVenue = "Scienceworks" }
            },
            {
                new Tuple<string, string, string, string>("SCIENCEWORKS (MvCIS)", "Ground Level", "South", "Turret"),
                new MuseumLocation { Gallery = "Turret", MuseumVenue = "Scienceworks" }
            },
            {
                new Tuple<string, string, string, string>("SCIENCEWORKS (MvCIS)", "Level 1", "South", "Turret"),
                new MuseumLocation { Gallery = "Turret", MuseumVenue = "Scienceworks" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "Ground Level", "Foyer", string.Empty),
                new MuseumLocation { Gallery = "Museum Entrance", MuseumVenue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "Ground Level", "Discovery Centre", string.Empty),
                new MuseumLocation { Gallery = "Discovery Centre", MuseumVenue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "Level 1", "West", "Gallery 1"),
                new MuseumLocation { Gallery = "Leavings", MuseumVenue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "Level 1", "West", "Gallery 2"),
                new MuseumLocation { Gallery = "Settings", MuseumVenue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "Level 1", "Central", "Gallery 3"),
                new MuseumLocation { Gallery = "Journeys (long room)", MuseumVenue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "Level 1", "East", "Gallery 4"),
                new MuseumLocation { Gallery = "Getting In", MuseumVenue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "Level 1", "East", "Gallery 5"),
                new MuseumLocation { Gallery = "Access Gallery", MuseumVenue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "Level 2", "West", "Gallery 6"),
                new MuseumLocation { Gallery = "West Wing Gallery", MuseumVenue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "Level 2", "West", "Gallery 7"),
                new MuseumLocation { Gallery = "West Wing Gallery", MuseumVenue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "Level 2", "West", "Gallery 7"),
                new MuseumLocation { Gallery = "West Wing Gallery", MuseumVenue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "Level 2", "East", "Gallery 8"),
                new MuseumLocation { Gallery = "East Wing Gallery", MuseumVenue = "Immigration Museum" }
            },
        };

        public static string[] TaxonomyTypeStatuses = { "holotype", "lectotype", "neotype", "paralectotype", "paratype", "syntype", "type" };
    }
}