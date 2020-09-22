namespace CollectionsOnline.Tasks
{
    public class AppSettings
    {
        public const string APP_SETTINGS = "AppSettings";
        
        public string DatabaseUrl { get; set; }
        
        public string DatabaseName { get; set; }
        
        public string WebSitePath { get; set; }
        
        public string WebSiteUser { get; set; }
        
        public string WebSitePassword { get; set; }
        
        public string WebSiteComputer { get; set; }
        
        public string WebSiteDomain { get; set; }
        
        public string CanonicalSiteBase { get; set; }
    }
}