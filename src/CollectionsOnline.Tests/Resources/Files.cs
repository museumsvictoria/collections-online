using System.IO;
using System.Reflection;

namespace CollectionsOnline.Tests.Resources
{
    public static class Files
    {
        private static string RootFolder => @"..\..\CollectionsOnline.Tests\Resources\Images\";
        
        public static string OutputFolder => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Output\";
        
        public static string Fish => RootFolder + @"parma-victoriae.tif";
        
        public static string Brochure => RootFolder + @"brochure.jpg";
    }
}