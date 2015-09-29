namespace CollectionsOnline.WebSite.Models.Api
{
    public class AuthorApiViewModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName { get; set; }

        public string Biography { get; set; }

        public ImageMediaApiViewModel ProfileImage { get; set; }
    }
}