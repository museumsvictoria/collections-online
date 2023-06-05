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
                new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Curious?", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
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
                new Tuple<string, string, string, string>("ROYAL EXHIBITION BUILDING (MvCIS)", "BASEMENT", "GALLERY 1", "*"),
                new MuseumLocation { Venue = "Royal Exhibition Building", Gallery = "Basement", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.RoyalExhibitionBuilding }
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
                new MuseumLocation { Venue = "Immigration Museum", Gallery = "Immigration Discovery Centre", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.ImmigrationMuseum }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "GROUND LEVEL", "FOYER", "*"),
                new MuseumLocation { Venue = "Immigration Museum", Gallery = "Museum Entrance", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.ImmigrationMuseum }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "LEVEL 1", "NORTH EAST CORRIDOR", "*"),
                new MuseumLocation { Venue = "Immigration Museum", Gallery = "Level 1 East Wing Gallery", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.ImmigrationMuseum }
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
                new MuseumLocation { Venue = "Immigration Museum", Gallery = "Level 1 East Wing Gallery", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.ImmigrationMuseum }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "LEVEL 1", "GALLERY 2", "*"),
                new MuseumLocation { Venue = "Immigration Museum", Gallery = "Immigrant Stories and Timeline", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.ImmigrationMuseum }
            },
            {
                new Tuple<string, string, string, string>("IMMIGRATION MUSEUM (MvCIS)", "LEVEL 1", "GALLERY 1", "*"),
                new MuseumLocation { Venue = "Immigration Museum", Gallery = "Level 1 West Wing Gallery", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.ImmigrationMuseum }
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
            new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "GROUND LEVEL", "GALLERY 8", "GRID F5"),
            new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "LEVEL 1", "GALLERY 12", "GRID C1"),
            new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "LEVEL 1", "GALLERY 12", "GRID C2"),
            new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "LEVEL 1", "GALLERY 12", "GRID D1"),
            new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "LEVEL 1", "GALLERY 12", "GRID D2"),
            new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "LEVEL 1", "GALLERY 12", "GRID D3"),
            new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "LEVEL 1", "GALLERY 12", "GRID E1"),
            new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "LEVEL 1", "GALLERY 12", "GRID E2"),
            new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "LEVEL 1", "GALLERY 12", "GRID E3"),
            new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "LEVEL 1", "GALLERY 12", "GRID F1"),
            new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "LEVEL 1", "GALLERY 12", "GRID F2"),
            new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "LEVEL 1", "GALLERY 12", "GRID Y1"),
            new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "LEVEL 1", "GALLERY 12", "GRID Y2"),
            new Tuple<string, string, string, string>("MELBOURNE (MvCIS)", "LEVEL 1", "GALLERY 12", "GRID X2"),
        };

        public static Dictionary<string, MuseumLocation> Events = new Dictionary
            <string, MuseumLocation>
            {
                {
                    "4", new MuseumLocation { Venue = "Immigration Museum", Exhibition = "Customs Gallery", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.ImmigrationMuseum }
                },
                {
                    "94", new MuseumLocation { Venue = "Immigration Museum", Exhibition = "Getting In", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.ImmigrationMuseum }
                },
                {
                    "294", new MuseumLocation { Venue = "Immigration Museum", Exhibition = "Leaving Home", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.ImmigrationMuseum }
                },
                {
                    "517", new MuseumLocation { Venue = "Immigration Museum", Exhibition = "Identity: Yours, Mine, Ours", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.ImmigrationMuseum }
                },
                {
                    "30", new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Forest Secrets", Exhibition = "Forest Gallery", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
                },
                {
                    "46", new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Science & Life Gallery", Exhibition = "Bugs Alive!", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
                },
                {
                    "56", new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Te Pasifika Gallery", Exhibition = "Te Vainui O Pasifika", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
                },
                {
                    "306", new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Melbourne Gallery", Exhibition = "Melbourne Story", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
                },
                {
                    "307", new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Melbourne Gallery", Exhibition = "Melbourne Story", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
                },
                {
                    "308", new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Melbourne Gallery", Exhibition = "Melbourne Story", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
                },
                {
                    "309", new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Melbourne Gallery", Exhibition = "Melbourne Story", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
                },
                {
                    "310", new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Melbourne Gallery", Exhibition = "Melbourne Story", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
                },
                {
                    "311", new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Melbourne Gallery", Exhibition = "Melbourne Story", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
                },
                {
                    "312", new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Melbourne Gallery", Exhibition = "Melbourne Story", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
                },
                {
                    "313", new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Melbourne Gallery", Exhibition = "Melbourne Story", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
                },
                {
                    "314", new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Melbourne Gallery", Exhibition = "Melbourne Story", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
                },
                {
                    "355", new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Upper Level Balcony", Exhibition = "West Balcony Display", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
                },
                {
                    "363", new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Upper Level Balcony", Exhibition = "West Balcony Display", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
                },
                {
                    "365", new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Upper Level Balcony", Exhibition = "West Balcony Display", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
                },
                {
                    "366", new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Upper Level Balcony", Exhibition = "Federation Tapastries", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
                },
                {
                    "367", new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Upper Level Balcony", Exhibition = "West Balcony Display", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
                },
                {
                    "368", new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Upper Level Balcony", Exhibition = "West Balcony Display", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
                },
                {
                    "373", new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Upper Level Balcony", Exhibition = "East Balcony Display", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
                },
                {
                    "376", new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Upper Level Balcony", Exhibition = "East Balcony Display", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
                },                
                {
                    "412", new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Upper Level Balcony", Exhibition = "West Balcony Display", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
                },
                {
                    "443", new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Main Entrance", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
                },
                {
                    "483", new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Science & Life Gallery", Exhibition = "Dinosaur Walk", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
                },
                {
                    "484", new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Science & Life Gallery", Exhibition = "600 Million Years: Victoria Evolves", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
                },
                {
                    "485", new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Science & Life Gallery", Exhibition = "Dynamic Earth", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
                },
                {
                    "525", new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Upper Level Balcony", Exhibition = "East Balcony Display", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
                },
                {
                    "601", new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Upper Level Balcony", Exhibition = "East Balcony Display", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
                },
                {
                    "621", new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Bunjilaka Cultural Centre", Exhibition = "First Peoples", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.Bunjilaka }
                },
                {
                    "1064", new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Melbourne Gallery", Exhibition = "Mini Mega Model Museum", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.Bunjilaka }
                },
                {
                    "700", new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Bunjilaka Cultural Centre", Exhibition = "First Peoples", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.Bunjilaka }
                },
                {
                    "701", new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Bunjilaka Cultural Centre", Exhibition = "First Peoples", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.Bunjilaka }
                },
                {
                    "914", new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Children's Gallery", Exhibition = "Children's Museum", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
                },
                {
                    "916", new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Ground Level Concourse", Exhibition = "East Concourse Displays", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
                },
                {
                    "1157", new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Te Pasifika Gallery", Exhibition = "Tok stori Vikitolia Pasifiki | Victorian Pacific Community Stories", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
                },
                {
                    "1184", new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Evolution Gallery", Exhibition = "Triceratops: Age of the Dinosaurs", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
                },
                {
                    "1229", new MuseumLocation { Venue = "Melbourne Museum", Gallery = "Science & Life Gallery", Exhibition = "Museums Victoria Research Institute Gallery", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.MelbourneMuseum }
                },
                {
                    "1115", new MuseumLocation { Venue = "Royal Exhibition Building", Gallery = "Basement", Exhibition = "Basement Gallery Display", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.RoyalExhibitionBuilding }
                },
                {
                    "180", new MuseumLocation { Venue = "Scienceworks", Gallery = "Ground Floor, Main Exhibition Space", Exhibition = "Sportsworks", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.Scienceworks }
                },
                {
                    "495", new MuseumLocation { Venue = "Scienceworks", Gallery = "Garage", Exhibition = "Heritage Machines", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.Scienceworks }
                },
                {
                    "496", new MuseumLocation { Venue = "Scienceworks", Gallery = "Spotswood Pumping Station", Exhibition = "Pumping Station", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.Scienceworks }
                },
                {
                    "611", new MuseumLocation { Venue = "Scienceworks", Gallery = "Ground Floor, Main Exhibition Space", Exhibition = "Think Ahead", DisplayStatus = DisplayStatus.OnDisplay, DisplayLocation = DisplayLocation.Scienceworks }
                },                
            };
    }
}