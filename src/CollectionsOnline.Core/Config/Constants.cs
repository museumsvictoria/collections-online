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

        public static int SummaryMaxChars = 230;

        public static int MetadataDescriptionMaxChars = 400;

        public static int CollectionSummaryMaxChars = 120;

        public static int HomeMaxRecentResults = 12;

        public static int MaxSitemapUrls = 50000;

        public static int PagingStreamSize = 4000;

        public static string ImuItemQueryString = "Collections Online: Humanities";

        public static string ImuSpeciesQueryString = "Collections Online: Species Profile";

        public static string ImuSpecimenQueryString = "Collections Online: Natural Sciences";

        public static string ImuArticleQueryString = "Collections Online: Article";

        public static string ImuCollectionQueryString = "Collections Online: Collection Overview";

        public static string ImuMultimediaQueryString = "Collections Online: MMR";

        public static string ImuVideoQueryString = "Video URL";

        public static string ImuUriQueryString = "Webpage Link URL";

        public static TimeSpan AggressiveCacheTimeSpan = TimeSpan.FromHours(24);

        public static TimeSpan ImuOfflineTimeSpanStart = new TimeSpan(19, 00, 0); // 7:00pm
        
        public static TimeSpan ImuOfflineTimeSpanEnd = new TimeSpan(23, 00, 0); // 11:00pm        

        public static string ApiPathBase = "/api";

        public static Dictionary<Tuple<string, string, string, string>, MuseumLocation> MuseumLocations = new Dictionary
            <Tuple<string, string, string, string>, MuseumLocation>(new OrdinalIgnoreCaseTupleComparer())
        {
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "GROUND LEVEL", "CENTRAL FOYER", string.Empty),
                new MuseumLocation { Gallery = "Main Entrance", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "GROUND LEVEL", "ADMINISTRATION OFFICE", string.Empty),
                new MuseumLocation { Gallery = "Administration", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "GROUND LEVEL", "WEST VESTIBULE 1", string.Empty),
                new MuseumLocation { Gallery = "Public Spaces Ground Floor", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "GROUND LEVEL", "WEST VESTIBULE 2", string.Empty),
                new MuseumLocation { Gallery = "Public Spaces Ground Floor", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "GROUND LEVEL", "CENTRAL WALKWAY", string.Empty),
                new MuseumLocation { Gallery = "Public Spaces Ground Floor", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "GROUND LEVEL", "EAST WALKWAY", string.Empty),
                new MuseumLocation { Gallery = "Public Spaces Ground Floor", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "GROUND LEVEL", "WEST WALKWAY", string.Empty),
                new MuseumLocation { Gallery = "Public Spaces Ground Floor", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "GROUND LEVEL", "WEST CORRIDOR", string.Empty),
                new MuseumLocation { Gallery = "Public Spaces Ground Floor", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "GROUND LEVEL", "GALLERY 3", string.Empty),
                new MuseumLocation { Gallery = "Children's Gallery", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "GROUND LEVEL", "GALLERY 4", string.Empty),
                new MuseumLocation { Gallery = "Science & Life Gallery", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "GROUND LEVEL", "GALLERY 5", string.Empty),
                new MuseumLocation { Gallery = "Wild", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "GROUND LEVEL", "GALLERY 5", "LINK"),
                new MuseumLocation { Gallery = "Wild", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "LEVEL 1", "GALLERY 11", string.Empty),
                new MuseumLocation { Gallery = "Darwin to DNA", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "GROUND LEVEL", "GALLERY 6", string.Empty),
                new MuseumLocation { Gallery = "Forest Gallery", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "GROUND LEVEL", "GALLERY 8", string.Empty),
                new MuseumLocation { Gallery = "Bunjilaka - First Peoples", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "GROUND LEVEL", "GALLERY 9", string.Empty),
                new MuseumLocation { Gallery = "Bunjilaka - Kalaya & Milarri Gardens", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "LEVEL 1", "GALLERY 7", string.Empty),
                new MuseumLocation { Gallery = "Te Pasifika Gallery", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "GROUND LEVEL", "GALLERY 7", string.Empty),
                new MuseumLocation { Gallery = "Te Pasifika Gallery", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "GROUND LEVEL", "GALLERY 7", "MEZZANINE"),
                new MuseumLocation { Gallery = "Te Pasifika Gallery", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "LEVEL 1", "GALLERY 12", string.Empty),
                new MuseumLocation { Gallery = "Melbourne Gallery", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "LEVEL 1", "CENTRAL BALCONY", string.Empty),
                new MuseumLocation { Gallery = "Upper Level Balcony", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "LEVEL 1", "EAST BALCONY", string.Empty),
                new MuseumLocation { Gallery = "Upper Level Balcony", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "LEVEL 1", "WEST BALCONY", string.Empty),
                new MuseumLocation { Gallery = "Upper Level Balcony", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "LEVEL 1", "GALLERY 10", string.Empty),
                new MuseumLocation { Gallery = "Mind and Body Gallery", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "LOWER GROUND", "GALLERY 1", string.Empty),
                new MuseumLocation { Gallery = "Discovery Centre", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "LOWER GROUND", "CENTRAL WALKWAY", string.Empty),
                new MuseumLocation { Gallery = "Public Spaces Lower Level", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "LOWER GROUND", "EAST WALKWAY", string.Empty),
                new MuseumLocation { Gallery = "Public Spaces Lower Level", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "LOWER GROUND", "WEST WALKWAY", string.Empty),
                new MuseumLocation { Gallery = "Public Spaces Lower Level", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "LOWER GROUND", "GALLERY 2", string.Empty),
                new MuseumLocation { Gallery = "Touring Hall", Venue = "Melbourne Museum" }
            },
            {
                new Tuple<string, string, string, string>("ROYAL EXHIBITION BUILDING (MvCIS)", "GROUND LEVEL", "CENTRAL HALL", string.Empty),
                new MuseumLocation { Gallery = "Ground Level", Venue = "Royal Exhibition Building" }
            },
            {
                new Tuple<string, string, string, string>("ROYAL EXHIBITION BUILDING (MvCIS)", "GROUND LEVEL", "EAST HALL", string.Empty),
                new MuseumLocation { Gallery = "Ground Level", Venue = "Royal Exhibition Building" }
            },
            {
                new Tuple<string, string, string, string>("ROYAL EXHIBITION BUILDING (MvCIS)", "GROUND LEVEL", "NORTH HALL", string.Empty),
                new MuseumLocation { Gallery = "Ground Level", Venue = "Royal Exhibition Building" }
            },
            {
                new Tuple<string, string, string, string>("ROYAL EXHIBITION BUILDING (MvCIS)", "GROUND LEVEL", "SOUTH HALL", string.Empty),
                new MuseumLocation { Gallery = "Ground Level", Venue = "Royal Exhibition Building" }
            },
            {
                new Tuple<string, string, string, string>("ROYAL EXHIBITION BUILDING (MvCIS)", "GROUND LEVEL", "WEST FORECOURT", string.Empty),
                new MuseumLocation { Gallery = "Ground Level", Venue = "Royal Exhibition Building" }
            },
            {
                new Tuple<string, string, string, string>("ROYAL EXHIBITION BUILDING (MvCIS)", "GROUND LEVEL", "WEST HALL", string.Empty),
                new MuseumLocation { Gallery = "Ground Level", Venue = "Royal Exhibition Building" }
            },
            {
                new Tuple<string, string, string, string>("ROYAL EXHIBITION BUILDING (MvCIS)", "LEVEL 1 ", "EAST BALCONY", string.Empty),
                new MuseumLocation { Gallery = "Mezzanine Level", Venue = "Royal Exhibition Building" }
            },
            {
                new Tuple<string, string, string, string>("ROYAL EXHIBITION BUILDING (MvCIS)", "LEVEL 1 ", "NORTH BALCONY", string.Empty),
                new MuseumLocation { Gallery = "Mezzanine Level", Venue = "Royal Exhibition Building" }
            },
            {
                new Tuple<string, string, string, string>("ROYAL EXHIBITION BUILDING (MvCIS)", "LEVEL 1 ", "SOUTH BALCONY", string.Empty),
                new MuseumLocation { Gallery = "Mezzanine Level", Venue = "Royal Exhibition Building" }
            },
            {
                new Tuple<string, string, string, string>("ROYAL EXHIBITION BUILDING (MvCIS)", "LEVEL 1 ", "WEST BALCONY", string.Empty),
                new MuseumLocation { Gallery = "Mezzanine Level", Venue = "Royal Exhibition Building" }
            },
            {
                new Tuple<string, string, string, string>("SCIENCEWORKS (MvCIS)", "GROUND LEVEL", "FOYER", string.Empty),
                new MuseumLocation { Gallery = "Museum Entrance", Venue = "Scienceworks" }
            },
            {
                new Tuple<string, string, string, string>("SCIENCEWORKS (MvCIS)", "GROUND LEVEL", "GALLERY 2", string.Empty),
                new MuseumLocation { Gallery = "Think Ahead", Venue = "Scienceworks" }
            },
            {
                new Tuple<string, string, string, string>("SCIENCEWORKS (MvCIS)", "LEVEL 1", "GALLERY 3", string.Empty),
                new MuseumLocation { Gallery = "Nitty Gritty Super City", Venue = "Scienceworks" }
            },
            {
                new Tuple<string, string, string, string>("SCIENCEWORKS (MvCIS)", "GROUND LEVEL", "PLANETARIUM", string.Empty),
                new MuseumLocation { Gallery = "Planetarium", Venue = "Scienceworks" }
            },
            {
                new Tuple<string, string, string, string>("SCIENCEWORKS (MvCIS)", "GROUND LEVEL", "GARAGE", string.Empty),
                new MuseumLocation { Gallery = "Heritage Machines Garage", Venue = "Scienceworks" }
            },
            {
                new Tuple<string, string, string, string>("SCIENCEWORKS (MvCIS)", "LEVEL 1", "FOYER", string.Empty),
                new MuseumLocation { Gallery = "Historical Displays", Venue = "Scienceworks" }
            },
            {
                new Tuple<string, string, string, string>("SCIENCEWORKS (MvCIS)", "GROUND LEVEL", "GALLERY 1", string.Empty),
                new MuseumLocation { Gallery = "Touring Hall", Venue = "Scienceworks" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "GROUND LEVEL", "FOYER", string.Empty),
                new MuseumLocation { Gallery = "Museum Entrance", Venue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "GROUND LEVEL", "CENTRAL CORRIDOR", string.Empty),
                new MuseumLocation { Gallery = "Museum Entrance", Venue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "GROUND LEVEL", "EAST CORRIDOR", string.Empty),
                new MuseumLocation { Gallery = "Museum Entrance", Venue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "GROUND LEVEL", "WEST CORRIDOR", string.Empty),
                new MuseumLocation { Gallery = "Museum Entrance", Venue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "GROUND LEVEL", "DISCOVERY CENTRE", string.Empty),
                new MuseumLocation { Gallery = "Immigration Discovery Centre", Venue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "LEVEL 1", "FOYER", string.Empty),
                new MuseumLocation { Gallery = "Public Spaces Level 1", Venue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "LEVEL 1", "CENTRAL VESTIBULE", string.Empty),
                new MuseumLocation { Gallery = "Public Spaces Level 1", Venue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "LEVEL 1", "GALLERY 1", string.Empty),
                new MuseumLocation { Gallery = "Leaving Home", Venue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "LEVEL 1", "WEST CORRIDOR", string.Empty),
                new MuseumLocation { Gallery = "Leaving Home", Venue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "LEVEL 1", "GALLERY 2", string.Empty),
                new MuseumLocation { Gallery = "Immigrant Stories and Timeline", Venue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "LEVEL 1", "NORTH WEST CORRIDOR", string.Empty),
                new MuseumLocation { Gallery = "Customs Gallery", Venue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "LEVEL 1", "GALLERY 3", string.Empty),
                new MuseumLocation { Gallery = "Journeys of a Lifetime (The Long Room)", Venue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "LEVEL 1", "GALLERY 4", string.Empty),
                new MuseumLocation { Gallery = "Getting In", Venue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "LEVEL 1", "EAST CORRIDOR", string.Empty),
                new MuseumLocation { Gallery = "Getting In", Venue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "LEVEL 1", "NORTH EAST CORRIDOR", string.Empty),
                new MuseumLocation { Gallery = "Getting In", Venue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "LEVEL 1", "GALLERY 5", string.Empty),
                new MuseumLocation { Gallery = "Community Gallery", Venue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "LEVEL 2", "GALLERY 6", string.Empty),
                new MuseumLocation { Gallery = "West Wing Gallery", Venue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "LEVEL 2", "GALLERY 7", string.Empty),
                new MuseumLocation { Gallery = "Identity - Yours, Mine, Ours", Venue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "LEVEL 2", "NORTH WEST CORRIDOR", string.Empty),
                new MuseumLocation { Gallery = "Identity - Yours, Mine, Ours", Venue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "LEVEL 2", "WEST CORRIDOR", string.Empty),
                new MuseumLocation { Gallery = "Identity - Yours, Mine, Ours", Venue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "LEVEL 2", "FOYER", string.Empty),
                new MuseumLocation { Gallery = "Public Spaces Level 2", Venue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "LEVEL 2", "CENTRAL VESTIBULE", string.Empty),
                new MuseumLocation { Gallery = "Public Spaces Level 2", Venue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "LEVEL 2", "GALLERY 8", string.Empty),
                new MuseumLocation { Gallery = "East Wing Gallery", Venue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "LEVEL 2", "EAST CORRIDOR", string.Empty),
                new MuseumLocation { Gallery = "East Wing Gallery", Venue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "LEVEL 2", "NORTH EAST CORRIDOR", string.Empty),
                new MuseumLocation { Gallery = "East Wing Gallery", Venue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "LEVEL 2", "GALLERY 9", string.Empty),
                new MuseumLocation { Gallery = "East Wing Gallery", Venue = "Immigration Museum" }
            },
            {
                new Tuple<string, string, string, string>("PUMPING STATION (MvCIS)", "SOUTH", "STRAINING HOUSE", string.Empty),
                new MuseumLocation { Gallery = "South Straining House", Venue = "Pumping Station" }
            },
            {
                new Tuple<string, string, string, string>("PUMPING STATION (MvCIS)", "SOUTH", "BOILER HOUSE", string.Empty),
                new MuseumLocation { Gallery = "South Boiler House", Venue = "Pumping Station" }
            },
            {
                new Tuple<string, string, string, string>("PUMPING STATION (MvCIS)", "SOUTH", "ENGINE ROOM", string.Empty),
                new MuseumLocation { Gallery = "South Engine Room", Venue = "Pumping Station" }
            },
            {
                new Tuple<string, string, string, string>("PUMPING STATION (MvCIS)", "SOUTH", "TOWER", string.Empty),
                new MuseumLocation { Gallery = "South Tower", Venue = "Pumping Station" }
            },
            {
                new Tuple<string, string, string, string>("PUMPING STATION (MvCIS)", "NORTH", "TOWER", string.Empty),
                new MuseumLocation { Gallery = "North Tower", Venue = "Pumping Station" }
            },
            {
                new Tuple<string, string, string, string>("PUMPING STATION (MvCIS)", "NORTH", "ENGINE ROOM", string.Empty),
                new MuseumLocation { Gallery = "North Engine Room", Venue = "Pumping Station" }
            },
            {
                new Tuple<string, string, string, string>("PUMPING STATION (MvCIS)", "NORTH", "BOILER HOUSE", string.Empty),
                new MuseumLocation { Gallery = "North Boiler House", Venue = "Pumping Station" }
            },
            {
                new Tuple<string, string, string, string>("PUMPING STATION (MvCIS)", "NORTH", "STRAINING HOUSE", string.Empty),
                new MuseumLocation { Gallery = "North Straining House", Venue = "Pumping Station" }
            },
            {
                new Tuple<string, string, string, string>("PUMPING STATION (MvCIS)", "NORTH", "COURTYARD", string.Empty),
                new MuseumLocation { Gallery = "Courtyard", Venue = "Pumping Station" }
            },
            {
                new Tuple<string, string, string, string>("PUMPING STATION (MvCIS)", "CENTRAL", "COURTYARD", string.Empty),
                new MuseumLocation { Gallery = "Courtyard", Venue = "Pumping Station" }
            },
            {
                new Tuple<string, string, string, string>("PUMPING STATION (MvCIS)", "SOUTH", "COURTYARD", string.Empty),
                new MuseumLocation { Gallery = "Courtyard", Venue = "Pumping Station" }
            },
        };

        public static string[] TaxonomyTypeStatuses = { "holotype", "lectotype", "neotype", "paralectotype", "paratype", "syntype", "type" };
    }
}