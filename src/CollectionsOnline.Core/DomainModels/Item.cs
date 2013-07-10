using System;
using System.Collections.Generic;

namespace CollectionsOnline.Core.DomainModels
{
    public class Item : DomainModel
    {
        public DateTime DateModified { get; set; }

        public string Category { get; set; }

        public string Discipline { get; set; }

        public string Type { get; set; }

        public string RegistrationNumber { get; set; }

        public ICollection<string> CollectionNames { get; set; }

        public string PrimaryClassification { get; set; }

        public string SecondaryClassification { get; set; }

        public string TertiaryClassification { get; set; }

        public string Name { get; set; }

        public string Summary { get; set; }

        public string Description { get; set; }

        public string Inscription { get; set; }

        public ICollection<Association> Associations { get; set; }

        public ICollection<string> Tags { get; set; }

        public string Significance { get; set; }

        public string ModelScale { get; set; }

        public string Shape { get; set; }

        public ICollection<string> Dimensions { get; set; }

        public string References { get; set; }

        public ICollection<string> Bibliographies { get; set; }

        public Item(string irn)
        {
            Id = "Items/" + irn;
        }
    }
}