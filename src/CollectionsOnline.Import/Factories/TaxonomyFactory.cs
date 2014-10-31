using System.Collections.Generic;
using System.Linq;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Core.Extensions;
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

                taxonomy.Irn = long.Parse(map.GetString("irn"));
                taxonomy.Kingdom = map.GetString("ClaKingdom");
                taxonomy.Phylum = map.GetString("ClaPhylum");
                taxonomy.Subphylum = map.GetString("ClaSubphylum");
                taxonomy.Superclass = map.GetString("ClaSuperclass");
                taxonomy.Class = map.GetString("ClaClass");
                taxonomy.Subclass = map.GetString("ClaSubclass");
                taxonomy.Superorder = map.GetString("ClaSuperorder");
                taxonomy.Order = map.GetString("ClaOrder");
                taxonomy.Suborder = map.GetString("ClaSuborder");
                taxonomy.Infraorder = map.GetString("ClaInfraorder");
                taxonomy.Superfamily = map.GetString("ClaSuperfamily");
                taxonomy.Family = map.GetString("ClaFamily");
                taxonomy.Subfamily = map.GetString("ClaSubfamily");
                taxonomy.Genus = map.GetString("ClaGenus");
                taxonomy.Subgenus = map.GetString("ClaSubgenus");
                taxonomy.Species = map.GetString("ClaSpecies");
                taxonomy.Subspecies = map.GetString("ClaSubspecies");

                taxonomy.Author = map.GetString("AutAuthorString");
                taxonomy.Code = map.GetString("ClaApplicableCode");

                //higherClassification
                var higherClassification = new Dictionary<string, string>
                        {
                            { "Kingdom", map.GetString("ClaKingdom") },
                            { "Phylum", map.GetString("ClaPhylum") },
                            { "Subphylum", map.GetString("ClaSubphylum") },
                            { "Superclass", map.GetString("ClaSuperclass") },
                            { "Class", map.GetString("ClaClass") },
                            { "Subclass", map.GetString("ClaSubclass") },
                            { "Superorder", map.GetString("ClaSuperorder") },
                            { "Order", map.GetString("ClaOrder") },
                            { "Suborder", map.GetString("ClaSuborder") },
                            { "Infraorder", map.GetString("ClaInfraorder") },
                            { "Superfamily", map.GetString("ClaSuperfamily") },
                            { "Family", map.GetString("ClaFamily") },
                            { "Subfamily", map.GetString("ClaSubfamily") },
                            { "Genus", map.GetString("ClaGenus") },
                            { "Subgenus", map.GetString("ClaSubgenus") },
                            { "Species", map.GetString("ClaSpecies") },
                            { "Subspecies", map.GetString("ClaSubspecies") }
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
                    var status = name.GetString("ComStatus_tab");
                    var vernacularName = name.GetString("ComName_tab");

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