using System;
using System.Collections.Generic;
using CollectionsOnline.Core.Models;

namespace CollectionsOnline.Import.Config
{
    public static class LocationDictionaries
    {
        public static Dictionary<Tuple<string, string, string, string>, MuseumLocation> Locations = new Dictionary
            <Tuple<string, string, string, string>, MuseumLocation>
        {
            {
                new Tuple<string, string, string, string>("EXTERNAL (MvCIS)", "EXHIBITION LOAN", "*", "*"),
                new MuseumLocation { DisplayStatus = DisplayStatus.OnLoan }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "LOWER GROUND", "CENTRAL WALKWAY", "*"),
                new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Lower Level Public Spaces", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "LOWER GROUND", "EAST WALKWAY", "*"),
                new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Lower Level Public Spaces", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "LOWER GROUND", "WEST WALKWAY", "*"),
                new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Lower Level Public Spaces", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "LOWER GROUND", "GALLERY 2", "*"),
                new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Touring Hall", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "GROUND LEVEL", "EAST CORRIDOR", "*"),
                new MuseumLocation { Venue = "Melbourne Museum", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "GROUND LEVEL", "WEST CORRIDOR", "*"),
                new MuseumLocation { Venue = "Melbourne Museum", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "GROUND LEVEL", "GALLERY 8", "*"),
                new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Bunjilaka - First Peoples", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.Bunjilaka }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "GROUND LEVEL", "GALLERY 9", "*"),
                new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Bunjilaka - Kalaya & Milarri Gardens", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.Bunjilaka }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "GROUND LEVEL", "GALLERY 3", "*"),
                new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Children's Museum", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "GROUND LEVEL", "CURIOUS", "*"),
                new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Curious?", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.Curious }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "GROUND LEVEL", "GALLERY 5", "*"),
                new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Evolution Gallery", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "GROUND LEVEL", "GALLERY 6", "*"),
                new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Forest Gallery", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "GROUND LEVEL", "CENTRAL WALKWAY", "*"),
                new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Ground Level Walkway", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "GROUND LEVEL", "EAST WALKWAY", "*"),
                new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Ground Level Walkway", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "GROUND LEVEL", "WEST WALKWAY", "*"),
                new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Ground Level Walkway", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "GROUND LEVEL", "CENTRAL FOYER", "*"),
                new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Main Entrance", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "GROUND LEVEL", "GALLERY 4", "*"),
                new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Science & Life Gallery", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "GROUND LEVEL", "GALLERY 7", "*"),
                new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Te Pasifika Gallery", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "LEVEL 1", "GALLERY 11", "*"),
                new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Evolution Gallery", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "LEVEL 1", "GALLERY 12", "*"),
                new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Melbourne Gallery", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "LEVEL 1", "GALLERY 10", "*"),
                new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Mind and Body Gallery", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "LEVEL 1", "CENTRAL BALCONY", "*"),
                new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Upper Level Balcony", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "LEVEL 1", "EAST BALCONY", "*"),
                new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Upper Level Balcony", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
            },
            {
                new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "LEVEL 1", "WEST BALCONY", "*"),
                new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Upper Level Balcony", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
            },
            {
                new Tuple<string, string, string, string>("ROYAL EXHIBITION BUILDING (MvCIS)", "GROUND LEVEL", "*", "*"),
                new MuseumLocation { Venue = "Royal Exhibition Building", Gallery = "Ground Level", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.RoyalExhibitionBuilding }
            },
            {
                new Tuple<string, string, string, string>("ROYAL EXHIBITION BUILDING (MvCIS)", "GROUND LEVEL", "WEST FORECOURT", "*"),
                new MuseumLocation { Venue = "Royal Exhibition Building", Gallery = "West Forecourt", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.RoyalExhibitionBuilding }
            },
            {
                new Tuple<string, string, string, string>("ROYAL EXHIBITION BUILDING (MvCIS)", "LEVEL 1", "EAST BALCONY", "*"),
                new MuseumLocation { Venue = "Royal Exhibition Building", Gallery = "Mezzanine Level", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.RoyalExhibitionBuilding }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "GROUND LEVEL", "CENTRAL CORRIDOR", "*"),
                new MuseumLocation { Venue = "Immigration Museum", Gallery = "Ground Floor Public Spaces", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.ImmigrationMuseum }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "GROUND LEVEL", "EAST CORRIDOR", "*"),
                new MuseumLocation { Venue = "Immigration Museum", Gallery = "Ground Floor Public Spaces", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.ImmigrationMuseum }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "GROUND LEVEL", "WEST CORRIDOR", "*"),
                new MuseumLocation { Venue = "Immigration Museum", Gallery = "Ground Floor Public Spaces", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.ImmigrationMuseum }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "GROUND LEVEL", "DISCOVERY CENTRE", "*"),
                new MuseumLocation { Venue = "Immigration Museum", Gallery = "Immigration Discovery Centre", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.ImmigrationDiscoveryCentre }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "GROUND LEVEL", "FOYER", "*"),
                new MuseumLocation { Venue = "Immigration Museum", Gallery = "Museum Entrance", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.ImmigrationMuseum }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "LEVEL 1", "NORTH EAST CORRIDOR", "*"),
                new MuseumLocation { Venue = "Immigration Museum", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.ImmigrationMuseum }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "LEVEL 1", "GALLERY 5", "*"),
                new MuseumLocation { Venue = "Immigration Museum", Gallery = "Community Gallery", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.ImmigrationMuseum }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "LEVEL 1", "NORTH WEST CORRIDOR", "*"),
                new MuseumLocation { Venue = "Immigration Museum", Gallery = "Customs Gallery", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.ImmigrationMuseum }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "LEVEL 1", "GALLERY 4", "*"),
                new MuseumLocation { Venue = "Immigration Museum", Gallery = "Getting In", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.ImmigrationMuseum }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "LEVEL 1", "EAST CORRIDOR", "*"),
                new MuseumLocation { Venue = "Immigration Museum", Gallery = "Getting In", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.ImmigrationMuseum }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "LEVEL 1", "GALLERY 2", "*"),
                new MuseumLocation { Venue = "Immigration Museum", Gallery = "Immigrant Stories and Timeline", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.ImmigrationMuseum }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "LEVEL 1", "GALLERY 1", "*"),
                new MuseumLocation { Venue = "Immigration Museum", Gallery = "Leaving Home", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.ImmigrationMuseum }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "LEVEL 1", "WEST CORRIDOR", "*"),
                new MuseumLocation { Venue = "Immigration Museum", Gallery = "Leaving Home", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.ImmigrationMuseum }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "LEVEL 1", "FOYER", "*"),
                new MuseumLocation { Venue = "Immigration Museum", Gallery = "Level 1 Public Spaces", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.ImmigrationMuseum }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "LEVEL 1", "CENTRAL VESTIBULE", "*"),
                new MuseumLocation { Venue = "Immigration Museum", Gallery = "Level 1 Public Spaces", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.ImmigrationMuseum }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "LEVEL 1", "GALLERY 3", "*"),
                new MuseumLocation { Venue = "Immigration Museum", Gallery = "Long Room", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.ImmigrationMuseum }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "LEVEL 2", "GALLERY 8", "*"),
                new MuseumLocation { Venue = "Immigration Museum", Gallery = "Level 2 East Wing Gallery", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.ImmigrationMuseum }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "LEVEL 2", "EAST CORRIDOR", "*"),
                new MuseumLocation { Venue = "Immigration Museum", Gallery = "Level 2 East Wing Gallery", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.ImmigrationMuseum }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "LEVEL 2", "GALLERY 9", "*"),
                new MuseumLocation { Venue = "Immigration Museum", Gallery = "Level 2 East Wing Gallery", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.ImmigrationMuseum }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "LEVEL 2", "GALLERY 10", "*"),
                new MuseumLocation { Venue = "Immigration Museum", Gallery = "Level 2 East Wing Gallery", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.ImmigrationMuseum }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "LEVEL 2", "FOYER", "*"),
                new MuseumLocation { Venue = "Immigration Museum", Gallery = "Level 2 Public Spaces", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.ImmigrationMuseum }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "LEVEL 2", "CENTRAL VESTIBULE", "*"),
                new MuseumLocation { Venue = "Immigration Museum", Gallery = "Level 2 Public Spaces", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.ImmigrationMuseum }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "LEVEL 2", "GALLERY 6", "*"),
                new MuseumLocation { Venue = "Immigration Museum", Gallery = "Level 2 West Wing Gallery", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.ImmigrationMuseum }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "LEVEL 2", "GALLERY 7", "*"),
                new MuseumLocation { Venue = "Immigration Museum", Gallery = "Level 2 West Wing Gallery", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.ImmigrationMuseum }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "LEVEL 2", "NORTH WEST CORRIDOR", "*"),
                new MuseumLocation { Venue = "Immigration Museum", Gallery = "Level 2 West Wing Gallery", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.ImmigrationMuseum }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "LEVEL 2", "WEST CORRIDOR", "*"),
                new MuseumLocation { Venue = "Immigration Museum", Gallery = "Level 2 West Wing Gallery", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.ImmigrationMuseum }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "LEVEL 2", "NORTH EAST CORRIDOR", "*"),
                new MuseumLocation { Venue = "Immigration Museum", Gallery = "Level 2 West Wing Gallery", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.ImmigrationMuseum }
            },
            {
                new Tuple<string, string, string, string>("SCIENCEWORKS (MvCIS)", "GROUND LEVEL", "GALLERY 2", "*"),
                new MuseumLocation { Venue = "Scienceworks", Gallery = "Ground Floor Exhibition Gallery", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.Scienceworks }
            },
            {
                new Tuple<string, string, string, string>("SCIENCEWORKS (MvCIS)", "GROUND LEVEL", "GARAGE", "*"),
                new MuseumLocation { Venue = "Scienceworks", Gallery = "Heritage Machines Garage", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.Scienceworks }
            },
            {
                new Tuple<string, string, string, string>("SCIENCEWORKS (MvCIS)", "GROUND LEVEL", "FOYER", "*"),
                new MuseumLocation { Venue = "Scienceworks", Gallery = "Museum Entrance", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.Scienceworks }
            },
            {
                new Tuple<string, string, string, string>("SCIENCEWORKS (MvCIS)", "GROUND LEVEL", "PLANETARIUM", "*"),
                new MuseumLocation { Venue = "Scienceworks", Gallery = "Planetarium", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.Scienceworks }
            },
            {
                new Tuple<string, string, string, string>("SCIENCEWORKS (MvCIS)", "GROUND LEVEL", "GALLERY 1", "*"),
                new MuseumLocation { Venue = "Scienceworks", Gallery = "Touring Hall", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.Scienceworks }
            },
            {
                new Tuple<string, string, string, string>("SCIENCEWORKS (MvCIS)", "LEVEL 1", "FOYER", "*"),
                new MuseumLocation { Venue = "Scienceworks", Gallery = "Foyer Level 1", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.Scienceworks }
            },
            {
                new Tuple<string, string, string, string>("SCIENCEWORKS (MvCIS)", "LEVEL 1", "GALLERY 3", "*"),
                new MuseumLocation { Venue = "Scienceworks", Gallery = "Level 1 Exhibition Gallery", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.Scienceworks }
            },
            {
                new Tuple<string, string, string, string>("PUMPING STATION (MvCIS)", "*", "*", "*"),
                new MuseumLocation { Venue = "Scienceworks", Gallery = "Spotswood Pumping Station", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.Scienceworks }
            },
        };
        
        public static IEnumerable<Tuple<string, string, string, string>> LocationsToExclude => new[]
        {
            new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "GROUND LEVEL", "GALLERY 8", "GRID F1"),
            new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "GROUND LEVEL", "GALLERY 8", "GRID F2"),
            new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "GROUND LEVEL", "GALLERY 8", "GRID F3"),
            new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "GROUND LEVEL", "GALLERY 8", "GRID F4"),
            new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "GROUND LEVEL", "GALLERY 8", "GRID F5")
        };

        public static Dictionary<string, MuseumLocation> Events = new Dictionary
            <string, MuseumLocation>
            {
                {
                    "4", new MuseumLocation { Venue = "Immigration Museum", Exhibition = "Customs Gallery" }
                },
            };
    }
}