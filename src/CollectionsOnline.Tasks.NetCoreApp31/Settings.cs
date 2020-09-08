namespace CollectionsOnline.Tasks.NetCoreApp31
{
    public class Settings
    {
        public const string SETTINGS = "Settings";
        
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