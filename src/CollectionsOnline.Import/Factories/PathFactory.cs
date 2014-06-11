using System.Configuration;
using CollectionsOnline.Core.Models;

namespace CollectionsOnline.Import.Factories
{
    public static class PathFactory
    {
        public static string MakeDestPath(long irn, FileFormatType fileFormat, string derivative = null)
        {
            return string.Format("{0}\\{1}\\{2}", ConfigurationManager.AppSettings["MediaPath"], MakeSubFolder(irn), MakeFileName(irn, fileFormat, derivative));
        }

        public static string MakeUrlPath(long irn, FileFormatType fileFormat, string derivative = null)
        {
            return string.Format("{0}/{1}/{2}", ConfigurationManager.AppSettings["MediaServerUrl"], MakeSubFolder(irn), MakeFileName(irn, fileFormat, derivative));
        }

        private static int MakeSubFolder(long id)
        {
            return (int)(id % 10);
        }

        private static string MakeFileName(long irn, FileFormatType fileFormat, string derivative)
        {
            return derivative == null ? string.Format("{0}.{1}", irn, fileFormat.ToString().ToLower()) : string.Format("{0}-{1}.{2}", irn, derivative, fileFormat.ToString().ToLower());
        }
    }
}
