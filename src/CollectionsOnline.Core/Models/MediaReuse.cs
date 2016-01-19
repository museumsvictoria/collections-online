namespace CollectionsOnline.Core.Models
{
    public class MediaReuse : AggregateRoot
    {
        public MediaReuse()
        {
            Id = "mediareuses/";
        }

        public string Usage { get; set; }

        public string UsageMore { get; set; }

        public string DocumentId { get; set; }

        public long MediaId { get; set; }
    }
}