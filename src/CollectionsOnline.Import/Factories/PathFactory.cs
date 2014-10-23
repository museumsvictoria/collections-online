using System.Configuration;
using CollectionsOnline.Core.Models;

namespace CollectionsOnline.Import.Factories
{
    public static class PathFactory
    {
        public static string MakeDestPath(long irn, FileFormatType fileFormat, FileDerivativeType fileDerivative)
        {
            return string.Format("{0}\\{1}\\{2}", ConfigurationManager.AppSettings["MediaPath"], GetSubFolder(irn), GetFileName(irn, fileFormat, fileDerivative));
        }

        public static string MakeUriPath(long irn, FileFormatType fileFormat, FileDerivativeType fileDerivative)
        {
            return string.Format("{0}/{1}/{2}", ConfigurationManager.AppSettings["MediaServerUri"], GetSubFolder(irn), GetFileName(irn, fileFormat, fileDerivative));
        }

        private static int GetSubFolder(long id)
        {
            return (int)(id % 10);
        }

        private static string GetFileName(long irn, FileFormatType fileFormat, FileDerivativeType fileDerivative)
        {
            return fileDerivative == FileDerivativeType.None ? string.Format("{0}.{1}", irn, fileFormat.ToString().ToLower()) : string.Format("{0}-{1}.{2}", irn, fileDerivative.ToString().ToLower(), fileFormat.ToString().ToLower());
        }
    }
}