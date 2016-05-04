namespace CollectionsOnline.Core.Models
{
    public class Licence
    {
        public LicenceType Type { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public string Uri { get; set; }

        public bool NonCommercialOpen { get; set; }

        public bool Open { get; set; }
    }
    
    public enum LicenceType
    {
        CcBy,
        CcByNc,
        CcByNcNd,
        CcByNcSa,
        CcByNd,
        CcBySa,
        Cc0,
        PublicDomainMark,
        AllRightsReserved
    }
}