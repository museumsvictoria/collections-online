using LiteDB;

namespace CollectionsOnline.Import.Models
{
    public class MediaChecksum
    {
        public ObjectId Id { get; set; }
        
        public long Irn { get; set; }
        
        public string Md5Checksum { get; set; }
    }
}