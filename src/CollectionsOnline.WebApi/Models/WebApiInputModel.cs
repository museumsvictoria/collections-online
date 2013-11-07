namespace CollectionsOnline.WebApi.Models
{
    public class WebApiInputModel
    {
        public int Offset { get; set; }

        public int Limit { get; set; }

        public bool Envelope { get; set; }
    }
}