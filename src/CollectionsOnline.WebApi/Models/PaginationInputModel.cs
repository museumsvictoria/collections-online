namespace CollectionsOnline.WebApi.Models
{
    public class PaginationInputModel
    {
        public int Offset { get; set; }

        public int Limit { get; set; }

        public bool Envelope { get; set; }
    }
}