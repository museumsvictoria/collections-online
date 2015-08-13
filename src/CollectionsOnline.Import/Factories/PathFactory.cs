using System.Configuration;
using CollectionsOnline.Core.Models;

namespace CollectionsOnline.Import.Factories
{
    public static class PathFactory
    {
        public static string MakeDestPath(long irn, string fileExtension, FileDerivativeType fileDerivative)
        {
            return string.Format("{0}\\content\\media\\{1}\\{2}", ConfigurationManager.AppSettings["WebSitePath"], GetSubFolder(irn), GetFileName(irn, fileExtension, fileDerivative));
        }

        public static string MakeUriPath(long irn, string fileExtension, FileDerivativeType fileDerivative)
        {
            return string.Format("/content/media/{0}/{1}", GetSubFolder(irn), GetFileName(irn, fileExtension, fileDerivative));
        }

        private static int GetSubFolder(long id)
        {
            return (int)(id % 50);
        }

        private static string GetFileName(long irn, string fileExtension, FileDerivativeType fileDerivative)
        {
            return fileDerivative == FileDerivativeType.None ? string.Format("{0}{1}", irn, fileExtension.ToLower()) : string.Format("{0}-{1}{2}", irn, fileDerivative.ToString().ToLower(), fileExtension.ToLower());
        }
    }
}