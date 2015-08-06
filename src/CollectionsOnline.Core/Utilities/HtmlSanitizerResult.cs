namespace CollectionsOnline.Import.Utilities
{
    public class HtmlSanitizerResult
    {
        public bool HasRemovedTag { get; set; }

        public bool HasRemovedStyle { get; set; }

        public bool HasRemovedAttribute { get; set; }

        public string Html { get; set; }
    }
}