namespace CollectionsOnline.WebSite.Models.Api
{
    public class ApiInputModel
    {
        public int Offset { get; set; }

        public int Limit { get; set; }

        public bool Envelope { get; set; }
    }
}