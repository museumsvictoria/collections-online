using System;
using CollectionsOnline.Core.Extensions;

namespace CollectionsOnline.Core.Models
{
    public class MuseumLocation
    {
        public string Venue { get; set; }

        public string Gallery { get; set; }
        
        public string Exhibition { get; set; }

        public string DisplayLocation { get; set; }
        
        public string DisplayStatus { get; set; }
        
        public DateTime? DisplayStartDate { get; set; }
        
        public DateTime? DisplayEndDate { get; set; }
    }
    
    public static class DisplayLocation
    {
        public const string Bunjilaka = "Bunjilaka";
        
        public const string Curious = "Curious?";
    
        public const string MelbourneMuseum = "Melbourne Museum";
        
        public const string ImmigrationMuseum = "Immigration Museum";
        
        public const string RoyalExhibitionBuilding = "Royal Exhibition Building";
        
        public const string Scienceworks = "Scienceworks";
    }
    
    public static class DisplayStatus
    {
        public const string OnDisplay = "On display";
        
        public const string OnLoan = "On loan";
    
        public const string NotOnDisplay = "Not on display";
    }
}