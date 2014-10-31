namespace CollectionsOnline.WebSite.Features.Search
{
    public class SearchInputModel
    {
        public int Offset { get; set; }

        public int Limit { get; set; }

        public string Query { get; set; }

        /* facet fields */
        public string Type { get; set; }

        public string Category { get; set; }

        public string HasImages { get; set; }

        public string OnDisplay { get; set; }

        public string Discipline { get; set; }

        public string ItemType { get; set; }

        public string SpeciesType { get; set; }

        public string SpeciesEndemicity { get; set; }

        public string SpecimenScientificGroup { get; set; }

        public string SpecimenDiscipline { get; set; }

        public string[] ArticleTypes { get; set; }

        /* term fields */
        public string Keyword { get; set; }

        public string Locality { get; set; }

        public string Collection { get; set; }

        public string CulturalGroup { get; set; }

        public string Classification { get; set; }

        public string Name { get; set; }

        public string Technique { get; set; }

        public string Denomination { get; set; }

        public string Habitat { get; set; }

        public string Taxon { get; set; }

        public string TypeStatus { get; set; }

        public string GeoType { get; set; }

        public string MuseumLocation { get; set; }

        public string Article { get; set; }
    }
}