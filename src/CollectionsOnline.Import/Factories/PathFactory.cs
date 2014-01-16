using System.Configuration;
using CollectionsOnline.Core.Models;

namespace CollectionsOnline.Import.Factories
{
    public static class PathFactory
    {
        public static string GetDestPath(long irn, FileFormatType fileFormat, string derivative = null)
        {
            return string.Format("{0}\\{1}\\{2}", ConfigurationManager.AppSettings["MediaPath"], GetSubFolder(irn), GetFileName(irn, fileFormat, derivative));
        }

        public static string GetUrlPath(long irn, FileFormatType fileFormat, string derivative = null)
        {
            return string.Format("{0}/{1}/{2}", ConfigurationManager.AppSettings["MediaServerUrl"], GetSubFolder(irn), GetFileName(irn, fileFormat, derivative));
        }

        private static int GetSubFolder(long id)
        {
            return (int)(id % 10);
        }

        private static string GetFileName(long irn, FileFormatType fileFormat, string derivative)
        {
            return derivative == null ? string.Format("{0}.{1}", irn, fileFormat.ToString().ToLower()) : string.Format("{0}-{1}.{2}", irn, derivative, fileFormat.ToString().ToLower());
        }
    }
}
