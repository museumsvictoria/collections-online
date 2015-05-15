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

                if (higherClassification.SkipWhile(x => x.Key != "Subgenus").All(x => string.IsNullOrWhiteSpace(x.Value)))
                {
                    if (!string.IsNullOrWhiteSpace(taxonomy.TaxonRank))
                        taxonomy.TaxonName = higherClassification[taxonomy.TaxonRank];
                }
                else
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

        public string MakeScientificName(QualifierRankType qualifierRank, string qualifier, string genus,
            string subgenus, string species, string subspecies, string author)
        {
            string scientificName;

            if (qualifierRank == QualifierRankType.Species)
            {
                var taxonFirstPart = new[] {genus, subgenus}.Concatenate(" ");
                var taxonSecondPart = new[] {species, subspecies}.Concatenate(" ");
                
                scientificName = new[]
                {
                    string.IsNullOrWhiteSpace(taxonFirstPart) ? null : string.Format("<em>{0}</em>", taxonFirstPart),
                    qualifier,
                    string.IsNullOrWhiteSpace(taxonSecondPart) ? null : string.Format("<em>{0}</em>", taxonSecondPart),
                    author
                }.Concatenate(" ");
            }
            else
            {
                var taxonFirstPart = new[] { genus, subgenus, species, subspecies }.Concatenate(" ");
                
                scientificName = new[]
                {
                    qualifier,
                    string.IsNullOrWhiteSpace(taxonFirstPart) ? null : string.Format("<em>{0}</em>", taxonFirstPart),
                    author
                }.Concatenate(" ");
            }

            return string.IsNullOrWhiteSpace(scientificName) ? null : scientificName;
        }
    }
}