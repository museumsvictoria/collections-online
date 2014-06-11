using System.Collections.Generic;

namespace CollectionsOnline.Core.Models
{
    public class Taxonomy
    {
        public Taxonomy()
        {
            InitializeCollections();
        }

        public long Irn { get; set; }

        public string ScientificName { get; set; }

        public string Kingdom { get; set; }

        public string Phylum { get; set; }

        public string Subphylum { get; set; }

        public string Superclass { get; set; }

        public string Class { get; set; }

        public string Subclass { get; set; }

        public string Superorder { get; set; }

        public string Order { get; set; }

        public string Suborder { get; set; }

        public string Infraorder { get; set; }

        public string Superfamily { get; set; }        

        public string Family { get; set; }

        public string Subfamily { get; set; }

        public string Genus { get; set; }

        public string Subgenus { get; set; }

        public string Species { get; set; }

        public string Subspecies { get; set; }

        public string Author { get; set; }        

        public string Code { get; set; }

        public string TaxonRank { get; set; }

        public string CommonName { get; set; }

        public IList<string> OtherCommonNames { get; set; }

        private void InitializeCollections()
        {
            OtherCommonNames = new List<string>();
        }
    }
}