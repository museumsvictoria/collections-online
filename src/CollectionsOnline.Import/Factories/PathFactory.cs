using System;
using System.Configuration;
using System.IO;
using CollectionsOnline.Core.Models;
using Serilog;

namespace CollectionsOnline.Import.Factories
{
    public static class PathFactory
    {
        /// <summary>
        /// Fetches the path that would result from the passed in paramaters, does not create the directory
        /// </summary>
        /// <param name="irn">Irn of the file</param>
        /// <param name="fileExtension">File extension</param>
        /// <param name="fileDerivative">Type of file derivative</param>
        /// <returns>Path to file</returns>
        public static string GetDestPath(long irn, string fileExtension, FileDerivativeType fileDerivative)
        {
            return BuildDestPath(irn, fileExtension, fileDerivative);
        }

        /// <summary>
        /// Firstly builds the destination path based on the paramaters passed in then creates the path on disk
        /// </summary>
        /// <param name="irn">Irn of the file</param>
        /// <param name="fileExtension">File extension</param>
        /// <param name="fileDerivative">Type of file derivative</param>
        /// <returns>Final path that was created</returns>
        public static string MakeDestPath(long irn, string fileExtension, FileDerivativeType fileDerivative)
        {
            var destPath = BuildDestPath(irn, fileExtension, fileDerivative);

            CreateDirectory(destPath);

            return destPath;
        }

        /// <summary>
        /// Ensures the directory passed in is created on disk
        /// </summary>
        /// <param name="destPath">Path to create</param>
        public static void CreateDestPath(string destPath)
        {
            CreateDirectory(destPath);
        }

        /// <summary>
        /// Simply builds the uri path that would result from the passed in parameters
        /// </summary>
        /// <param name="irn">Irn of the file</param>
        /// <param name="fileExtension">File extension</param>
        /// <param name="fileDerivative">Type of file derivative</param>
        /// <returns>Path to uri</returns>
        public static string BuildUriPath(long irn, string fileExtension, FileDerivativeType fileDerivative)
        {
            return string.Format("/content/media/{0}/{1}", GetSubFolder(irn), GetFileName(irn, fileExtension, fileDerivative));
        }

        private static string BuildDestPath(long irn, string fileExtension, FileDerivativeType fileDerivative)
        {
            return string.Format("{0}\\content\\media\\{1}\\{2}", ConfigurationManager.AppSettings["WebSitePath"],GetSubFolder(irn), GetFileName(irn, fileExtension, fileDerivative));
        }

        private static void CreateDirectory(string path)
        {            
            if (string.IsNullOrWhiteSpace(path)) 
                return;

            var directory = Path.GetDirectoryName(path);

            try
            {                
                Directory.CreateDirectory(directory);
            }
            catch (Exception ex)
            {
                Log.Logger.Fatal(ex, "Error creating {directory} directory", directory);
                throw;
            }
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