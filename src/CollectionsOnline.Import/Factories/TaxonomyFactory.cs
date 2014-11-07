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
                var taxonomy = new Taxonomy();

                taxonomy.Irn = long.Parse(map.GetEncodedString("irn"));
                taxonomy.Kingdom = map.GetEncodedString("ClaKingdom");
                taxonomy.Phylum = map.GetEncodedString("ClaPhylum");
                taxonomy.Subphylum = map.GetEncodedString("ClaSubphylum");
                taxonomy.Superclass = map.GetEncodedString("ClaSuperclass");
                taxonomy.Class = map.GetEncodedString("ClaClass");
                taxonomy.Subclass = map.GetEncodedString("ClaSubclass");
                taxonomy.Superorder = map.GetEncodedString("ClaSuperorder");
                taxonomy.Order = map.GetEncodedString("ClaOrder");
                taxonomy.Suborder = map.GetEncodedString("ClaSuborder");
                taxonomy.Infraorder = map.GetEncodedString("ClaInfraorder");
                taxonomy.Superfamily = map.GetEncodedString("ClaSuperfamily");
                taxonomy.Family = map.GetEncodedString("ClaFamily");
                taxonomy.Subfamily = map.GetEncodedString("ClaSubfamily");
                taxonomy.Genus = map.GetEncodedString("ClaGenus");
                taxonomy.Subgenus = map.GetEncodedString("ClaSubgenus");
                taxonomy.Species = map.GetEncodedString("ClaSpecies");
                taxonomy.Subspecies = map.GetEncodedString("ClaSubspecies");

                taxonomy.Author = map.GetEncodedString("AutAuthorString");
                taxonomy.Code = map.GetEncodedString("ClaApplicableCode");

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
                        string.IsNullOrWhiteSpace(taxonomy.Subgenus)
                            ? null
                            : string.Format("({0})", taxonomy.Subgenus),
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
    }
}