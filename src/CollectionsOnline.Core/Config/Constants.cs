using System;
using System.Collections.Generic;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Core.Models;

namespace CollectionsOnline.Core.Config
{
    public static class Constants
    {
        public static string ApplicationId = "applications/collectionsonline";

        public static int DataBatchSize = 50;

        public static int CachedDataBatchSize = 10000;

        public static int PagingPerPageDefault = 40;

        public static int PagingPerPageMax = 100;

        public static int SuggestionsMinResultsSize = 15;

        public static int SummaryMaxChars = 120;

        public static int HomeMaxRecentResults = 12;
        
        public static int HomeHeroBackgroundImages = 5;

        public static string ImuItemQueryString = "Collections Online: Humanities";

        public static string ImuSpeciesQueryString = "Collections Online: Species Profile";

        public static string ImuSpecimenQueryString = "Collections Online: Natural Sciences";

        public static string ImuArticleQueryString = "Collections Online: Article";

        public static string ImuCollectionQueryString = "Collections Online: Collection Overview";

        public static string ImuMultimediaQueryString = "Collections Online: MMR";

        public static TimeSpan AggressiveCacheTimeSpan = TimeSpan.FromHours(12);

        public static TimeSpan ImuOfflineTimeSpanStart = new TimeSpan(19, 00, 0); // 7:00pm
        
        public static TimeSpan ImuOfflineTimeSpanEnd = new TimeSpan(23, 00, 0); // 11:00pm

        public static string CurrentApiVersionHeader = "application/vnd.collections.v1+json";

        public static string CurrentApiVersionPathSegment = "/{v1?}";

        public static string CurrentApiVersionPath = "/v1";

        public static string ApiBasePath = "/api";        

        public static Dictionary<Tuple<string, string, string, string>, MuseumLocation> MuseumLocations = new Dictionary
            <Tuple<string, string, string, string>, MuseumLocation>(new OrdinalIgnoreCaseTupleComparer())
        {
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Ground Level", "Central", "Foyer"),
                new MuseumLocation { Gallery = "Main Entrance", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Ground Level", "West", "Administration"),
                new MuseumLocation { Gallery = "Administration", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Ground Level", "West", "Vestibule"),
                new MuseumLocation { Gallery = "Public Spaces Ground Floor", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Ground Level", "West", "Walk"),
                new MuseumLocation { Gallery = "Public Spaces Ground Floor", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Ground Level", "Central", "Walk"),
                new MuseumLocation { Gallery = "Public Spaces Ground Floor", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Ground Level", "East", "Walk"),
                new MuseumLocation { Gallery = "Public Spaces Ground Floor", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Ground Level", "East", "Corridor"),
                new MuseumLocation { Gallery = "Public Spaces Ground Floor", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Ground Level", "West", "Corridor"),
                new MuseumLocation { Gallery = "Public Spaces Ground Floor", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Ground Level", "West", "Gallery 3"),
                new MuseumLocation { Gallery = "Children's Gallery", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Ground Level", "West", "Gallery 4"),
                new MuseumLocation { Gallery = "Science & Life Gallery", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Ground Level", "West", "Gallery 5"),
                new MuseumLocation { Gallery = "Wild", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Level 1", "West", "Gallery 11"),
                new MuseumLocation { Gallery = "Darwin to DNA", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Ground Level", "Central", "Gallery 6"),
                new MuseumLocation { Gallery = "Forest Gallery", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Ground Level", "East", "Gallery 8"),
                new MuseumLocation { Gallery = "Bunjilaka - First Peoples", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Ground Level", "East", "Gallery 9"),
                new MuseumLocation { Gallery = "Bunjilaka - Kalaya & Milarri Gardens", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Level 1", "East", "Gallery 7"),
                new MuseumLocation { Gallery = "Te Pasifika Gallery", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Ground Level", "East", "Gallery 7"),
                new MuseumLocation { Gallery = "Te Pasifika Gallery", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Level 1", "East", "Gallery 12"),
                new MuseumLocation { Gallery = "Melbourne Gallery", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Level 1", "West", "Balcony"),
                new MuseumLocation { Gallery = "Upper Level Balcony", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Level 1", "Central", "Balcony"),
                new MuseumLocation { Gallery = "Upper Level Balcony", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Level 1", "East", "Balcony"),
                new MuseumLocation { Gallery = "Upper Level Balcony", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Level 1", "West", "Gallery 10"),
                new MuseumLocation { Gallery = "Mind and Body Gallery", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Lower Ground", "West", "Gallery 1"),
                new MuseumLocation { Gallery = "Discovery Centre", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Lower Ground", "Central", string.Empty),
                new MuseumLocation { Gallery = "Public Spaces Lower Level", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Lower Ground", "West", string.Empty),
                new MuseumLocation { Gallery = "Public Spaces Lower Level", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Lower Ground", "East", string.Empty),
                new MuseumLocation { Gallery = "Public Spaces Lower Level", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "Lower Ground", "East", "Gallery 2"),
                new MuseumLocation { Gallery = "Touring Hall", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("ROYAL EXHIBITION BUILDING (MvCIS)", "North", "Ground", string.Empty),
                new MuseumLocation { Gallery = "Ground Level", Venue = "Royal Exhibition Building" }
            },
            {
                new Tuple<string, string, string, string>("ROYAL EXHIBITION BUILDING (MvCIS)", "South", "Ground", string.Empty),
                new MuseumLocation { Gallery = "Ground Level", Venue = "Royal Exhibition Building" }
            },
            {
                new Tuple<string, string, string, string>("ROYAL EXHIBITION BUILDING (MvCIS)", "East", "Ground", string.Empty),
                new MuseumLocation { Gallery = "Ground Level", Venue = "Royal Exhibition Building" }
            },
            {
                new Tuple<string, string, string, string>("ROYAL EXHIBITION BUILDING (MvCIS)", "West", "Ground", string.Empty),
                new MuseumLocation { Gallery = "Ground Level", Venue = "Royal Exhibition Building" }
            },
            {
                new Tuple<string, string, string, string>("ROYAL EXHIBITION BUILDING (MvCIS)", "Central", "Ground", string.Empty),
                new MuseumLocation { Gallery = "Ground Level", Venue = "Royal Exhibition Building" }
            },
            {
                new Tuple<string, string, string, string>("ROYAL EXHIBITION BUILDING (MvCIS)", "North", "Balcony", string.Empty),
                new MuseumLocation { Gallery = "Mezzanine Level", Venue = "Royal Exhibition Building" }
            },
            {
                new Tuple<string, string, string, string>("ROYAL EXHIBITION BUILDING (MvCIS)", "South", "Balcony", string.Empty),
                new MuseumLocation { Gallery = "Mezzanine Level", Venue = "Royal Exhibition Building" }
            },
            {
                new Tuple<string, string, string, string>("ROYAL EXHIBITION BUILDING (MvCIS)", "East", "Balcony", string.Empty),
                new MuseumLocation { Gallery = "Mezzanine Level", Venue = "Royal Exhibition Building" }
            },
            {
                new Tuple<string, string, string, string>("ROYAL EXHIBITION BUILDING (MvCIS)", "West", "Balcony", string.Empty),
                new MuseumLocation { Gallery = "Mezzanine Level", Venue = "Royal Exhibition Building" }
            },
            {
                new Tuple<string, string, string, string>("SCIENCEWORKS (MvCIS)", "Ground Level", "Central", string.Empty),
                new MuseumLocation { Gallery = "Drum", Venue = "Scienceworks" }
            },
            {
                new Tuple<string, string, string, string>("SCIENCEWORKS (MvCIS)", "Level 1", "Central", string.Empty),
                new MuseumLocation { Gallery = "Drum", Venue = "Scienceworks" }
            },
            {
                new Tuple<string, string, string, string>("SCIENCEWORKS (MvCIS)", "Ground Level", "South", "Gallery"),
                new MuseumLocation { Gallery = "Think Ahead", Venue = "Scienceworks" }
            },
            {
                new Tuple<string, string, string, string>("SCIENCEWORKS (MvCIS)", "Level 1", "South", "Gallery"),
                new MuseumLocation { Gallery = "Nitty Gritty Super City", Venue = "Scienceworks" }
            },
            {
                new Tuple<string, string, string, string>("SCIENCEWORKS (MvCIS)", "Planetarium", string.Empty, string.Empty),
                new MuseumLocation { Gallery = "Planetarium", Venue = "Scienceworks" }
            },
            {
                new Tuple<string, string, string, string>("SCIENCEWORKS (MvCIS)", "Ground Level", "North", "Gallery"),
                new MuseumLocation { Gallery = "Touring Hall", Venue = "Scienceworks" }
            },
            {
                new Tuple<string, string, string, string>("SCIENCEWORKS (MvCIS)", "Ground Level", "South", "Turret"),
                new MuseumLocation { Gallery = "Turret", Venue = "Scienceworks" }
            },
            {
                new Tuple<string, string, string, string>("SCIENCEWORKS (MvCIS)", "Level 1", "South", "Turret"),
                new MuseumLocation { Gallery = "Turret", Venue = "Scienceworks" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "Ground Level", "Foyer", string.Empty),
                new MuseumLocation { Gallery = "Museum Entrance", Venue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "Ground Level", "West", "Corridor"),
                new MuseumLocation { Gallery = "Museum Entrance", Venue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "Ground Level", "East", "Corridor"),
                new MuseumLocation { Gallery = "Museum Entrance", Venue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "Ground Level", "Central", "Corridor"),
                new MuseumLocation { Gallery = "Museum Entrance", Venue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "Ground Level", "Discovery Centre", string.Empty),
                new MuseumLocation { Gallery = "Discovery Centre", Venue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "Level 1", "Foyer", string.Empty),
                new MuseumLocation { Gallery = "Discovery Centre", Venue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "Level 1", "South West", "Vestibule"),
                new MuseumLocation { Gallery = "Discovery Centre", Venue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "Level 1", "West", "Gallery 1"),
                new MuseumLocation { Gallery = "Leaving Home", Venue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "Level 1", "West", "Corridor"),
                new MuseumLocation { Gallery = "Leaving Home", Venue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "Level 1", "West", "Gallery 2"),
                new MuseumLocation { Gallery = "Immigrant Stories and Timeline", Venue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "Level 1", "North West", "Corridor"),
                new MuseumLocation { Gallery = "Immigrant Stories and Timeline", Venue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "Level 1", "Central", "Gallery 3"),
                new MuseumLocation { Gallery = "Journeys of a Lifetime (The Long Room)", Venue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "Level 1", "North East", "Corridor"),
                new MuseumLocation { Gallery = "Journeys of a Lifetime (The Long Room)", Venue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "Level 1", "East", "Gallery 4"),
                new MuseumLocation { Gallery = "Getting In", Venue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "Level 1", "East", "Corridor"),
                new MuseumLocation { Gallery = "Getting In", Venue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "Level 1", "East", "Gallery 5"),
                new MuseumLocation { Gallery = "Community Gallery", Venue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "Level 2", "West", "Gallery 6"),
                new MuseumLocation { Gallery = "West Wing Gallery", Venue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "Level 2", "West", "Gallery 7"),
                new MuseumLocation { Gallery = "Identity: Yours, Mine, Ours", Venue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "Level 2", "West", "Corridor"),
                new MuseumLocation { Gallery = "Public Spaces Level 2", Venue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "Level 2", "South West", "Vestibule"),
                new MuseumLocation { Gallery = "Public Spaces Level 2", Venue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "Level 2", "Foyer", string.Empty),
                new MuseumLocation { Gallery = "Public Spaces Level 2", Venue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "Level 2", "North West", "Corridor"),
                new MuseumLocation { Gallery = "Public Spaces Level 2", Venue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "Level 2", "North East", "Corridor"),
                new MuseumLocation { Gallery = "Public Spaces Level 2", Venue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "Level 2", "East", "Corridor"),
                new MuseumLocation { Gallery = "Public Spaces Level 2", Venue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "Level 2", "East", "Gallery 8"),
                new MuseumLocation { Gallery = "East Wing Gallery", Venue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "Level 2", "East", "Gallery 9"),
                new MuseumLocation { Gallery = "East Wing Gallery", Venue = "Immigration Museum" }
            }
        };

        public static string[] TaxonomyTypeStatuses = { "holotype", "lectotype", "neotype", "paralectotype", "paratype", "syntype", "type" };
    }
}