using System.Collections.Generic;

namespace CollectionsOnline.Tasks
{
    public class AppSettings
    {
        public const string SECTION_NAME = "AppSettings";
        
        public string DatabaseUrl { get; set; }
        
        public string DatabaseName { get; set; }
        
        public string DatabaseUserName { get; set; }
        
        public string DatabasePassword { get; set; }
        
        public string DatabaseDomain { get; set; }
        
        public string WebSitePath { get; set; }
        
        public string WebSiteUser { get; set; }
        
        public string WebSitePassword { get; set; }
        
        public string WebSiteComputer { get; set; }
        
        public string WebSiteDomain { get; set; }
        
        public string CanonicalSiteBase { get; set; }
        
        public MediaReuseMigrationTask MediaReuseMigrationTask { get; set; }
    }

    public class MediaReuseMigrationTask
    {
        public IList<RavenDatabase> SourceDatabases { get; set; }
    }

    public class RavenDatabase
    {
        public string Url { get; set; }
        
        public string Name { get; set; }
    }
}