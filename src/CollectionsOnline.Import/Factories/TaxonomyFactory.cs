using System.Collections.Generic;
using System.Linq;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Import.Extensions;
using IMu;

namespace CollectionsOnline.Import.Factories
{
    public class TaxonomyFactory : ITaxonomyFactory
    {
        public Taxonomy Make(Map map)
        {
            if (map != null)
            {
                var taxonomy = new Taxonomy
                {
                    Irn = long.Parse(map.GetEncodedString("irn")),
                    Kingdom = map.GetEncodedString("ClaKingdom"),
                    Phylum = map.GetEncodedString("ClaPhylum"),
                    Subphylum = map.GetEncodedString("ClaSubphylum"),
                    Superclass = map.GetEncodedString("ClaSuperclass"),
                    Class = map.GetEncodedString("ClaClass"),
                    Subclass = map.GetEncodedString("ClaSubclass"),
                    Superorder = map.GetEncodedString("ClaSuperorder"),
                    Order = map.GetEncodedString("ClaOrder"),
                    Suborder = map.GetEncodedString("ClaSuborder"),
                    Infraorder = map.GetEncodedString("ClaInfraorder"),
                    Superfamily = map.GetEncodedString("ClaSuperfamily"),
                    Family = map.GetEncodedString("ClaFamily"),
                    Subfamily = map.GetEncodedString("ClaSubfamily"),
                    Genus = map.GetEncodedString("ClaGenus"),
                    Subgenus = map.GetEncodedString("ClaSubgenus"),
                    Species = map.GetEncodedString("ClaSpecies"),
                    Subspecies = map.GetEncodedString("ClaSubspecies"),
                    Author = map.GetEncodedString("AutAuthorString"),
                    Code = map.GetEncodedString("ClaApplicableCode")
                };

                //higherClassification
                var higherClassification = new Dictionary<string, string>
                        {
                            { "Kingdom", map.GetEncodedString("ClaKingdom") },
                            { "Phylum", map.GetEncodedString("ClaPhylum") },
                            { "Subphylum", map.GetEncodedString("ClaSubphylum") },
                            { "Superclass", map.GetEncodedString("ClaSuperclass") },
                            { "Class", map.GetEncodedString("ClaClass") },
                            { "Subclass", map.GetEncodedString("ClaSubclass") },
                            { "Superorder", map.GetEncodedString("ClaSuperorder") },
                            { "Order", map.GetEncodedString("ClaOrder") },
                            { "Suborder", map.GetEncodedString("ClaSuborder") },
                            { "Infraorder", map.GetEncodedString("ClaInfraorder") },
                            { "Superfamily", map.GetEncodedString("ClaSuperfamily") },
                            { "Family", map.GetEncodedString("ClaFamily") },
                            { "Subfamily", map.GetEncodedString("ClaSubfamily") },
                            { "Genus", map.GetEncodedString("ClaGenus") },
                            { "Subgenus", map.GetEncodedString("ClaSubgenus") },
                            { "Species", map.GetEncodedString("ClaSpecies") },
                            { "Subspecies", map.GetEncodedString("ClaSubspecies") }
                        };

                taxonomy.TaxonRank = higherClassification.Where(x => !string.IsNullOrWhiteSpace(x.Value)).Select(x => x.Key).LastOrDefault();

                if (!string.IsNullOrWhiteSpace(taxonomy.TaxonRank))
                {
                    taxonomy.TaxonName = higherClassification[taxonomy.TaxonRank];
                }
                
                // If we have any ranks below genus filled then construct a better taxon name
                if (higherClassification.SkipWhile(x => x.Key != "Subgenus").Any(x => !string.IsNullOrWhiteSpace(x.Value)))
                {
                    taxonomy.TaxonName = new[]
                    {
                        taxonomy.Genus,
                        string.IsNullOrWhiteSpace(taxonomy.Subgenus) ? null : string.Format("({0})", taxonomy.Subgenus),
                        taxonomy.Species,
                        taxonomy.Subspecies
                    }.Concatenate(" ");
                }

                var names = map.GetMaps("comname");
                foreach (var name in names)
                {
                    var status = name.GetEncodedString("ComStatus_tab");
                    var vernacularName = name.GetEncodedString("ComName_tab");

                    if (status != null && status.ToLower() == "preferred")
                    {
                        taxonomy.CommonName = vernacularName;
                    }
                    else if (status != null && status.ToLower() == "other")
                    {
                        taxonomy.OtherCommonNames.Add(vernacularName);
                    }
                }

                return taxonomy;
            }

            return null;
        }

        //TODO : Move to own factory
        public string MakeScientificName(QualifierRankType qualifierRank, string qualifier, Taxonomy taxonomy)
        {
            string scientificName;

            if (qualifierRank == QualifierRankType.Species)
            {
                var taxonFirstPart = new[] { taxonomy.Genus, string.IsNullOrWhiteSpace(taxonomy.Subgenus) ? null : string.Format("({0})", taxonomy.Subgenus) }.Concatenate(" ");
                var taxonSecondPart = new[] { taxonomy.Species, taxonomy.Subspecies }.Concatenate(" ");
                
                scientificName = new[]
                {
                    string.IsNullOrWhiteSpace(taxonFirstPart) ? null : string.Format("<em>{0}</em>", taxonFirstPart),
                    qualifier,
                    string.IsNullOrWhiteSpace(taxonSecondPart) ? null : string.Format("<em>{0}</em>", taxonSecondPart),
                    taxonomy.Author
                }.Concatenate(" ");
            }
            else if (qualifierRank == QualifierRankType.Genus)
            {
                var taxonFirstPart = new[] { taxonomy.Genus, string.IsNullOrWhiteSpace(taxonomy.Subgenus) ? null : string.Format("({0})", taxonomy.Subgenus), 
                    taxonomy.Species, taxonomy.Subspecies }.Concatenate(" ");
                
                scientificName = new[]
                {
                    qualifier,
                    string.IsNullOrWhiteSpace(taxonFirstPart) ? null : string.Format("<em>{0}</em>", taxonFirstPart),
                    taxonomy.Author
                }.Concatenate(" ");
            }
            else
            {
                // If there is no qualifier rank, try to concatenate taxon levels at and below genus, if that is empty instead use the taxon name
                var taxonFirstPart = new[] { taxonomy.Genus, string.IsNullOrWhiteSpace(taxonomy.Subgenus) ? null : string.Format("({0})", taxonomy.Subgenus), 
                    taxonomy.Species, taxonomy.Subspecies }.Concatenate(" ");

                scientificName = new[]
                {
                    string.IsNullOrWhiteSpace(taxonFirstPart) ? taxonomy.TaxonName : string.Format("<em>{0}</em>", taxonFirstPart),
                    taxonomy.Author
                }.Concatenate(" ");
            }

            return string.IsNullOrWhiteSpace(scientificName) ? null : scientificName;
        }
    }
}