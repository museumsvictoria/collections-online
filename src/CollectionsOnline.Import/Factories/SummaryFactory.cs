using System;
using System.Linq;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Import.Builders;

namespace CollectionsOnline.Import.Factories
{
    public class SummaryFactory : ISummaryFactory
    {
        public string Make(Article article)
        {
            var summaryBuilder = new SummaryBuilder();

            return summaryBuilder
                .AddText(article.ContentText.RemoveLineBreaks())
                .ToString();
        }

        public string Make(Item item)
        {
            var summaryBuilder = new SummaryBuilder();

            return summaryBuilder
                .AddHeading(item.CollectingAreas.Concatenate(", "))
                .AddText(item.ObjectSummary)
                .ToString();
        }

        public string Make(Species species)
        {
            var summaryBuilder = new SummaryBuilder();

            return summaryBuilder
                .AddText(species.Biology)
                .ToString();
        }

        public string Make(Specimen specimen)
        {
            var summaryBuilder = new SummaryBuilder();

            summaryBuilder
                .AddHeading(specimen.CollectingAreas.Concatenate(", "));

            if (string.Equals(specimen.ScientificGroup, "Geology", StringComparison.OrdinalIgnoreCase))
            {
                summaryBuilder
                    .AddField("Type status", specimen.MineralogyTypeOfType)
                    .AddField("Specimen form", specimen.Storages.Select(x => x.Form).Concatenate(", "))
                    .AddField("Class", specimen.MineralogyClass)
                    .AddField("Class", specimen.MeteoritesClass)
                    .AddField("Group", specimen.MineralogyGroup)
                    .AddField("Group", specimen.MeteoritesGroup)
                    .AddField("Classification ", specimen.TektitesClassification)
                    .AddField("Shape ", specimen.TektitesShape);
            }
            else
            {
                summaryBuilder
                    .AddField("Common name", (specimen.Taxonomy == null) ? null : specimen.Taxonomy.CommonName)
                    .AddField("Type status", specimen.TypeStatus)
                    .AddField("Specimen nature", specimen.Storages.Select(x => x.Nature).Concatenate(", "))
                    .AddField("Higher taxonomy", (specimen.Taxonomy == null) ? null : new[]
                        {
                            specimen.Taxonomy.Phylum,
                            specimen.Taxonomy.Class,
                            specimen.Taxonomy.Order
                        }.Concatenate(", "))
                    .AddField("Collected", (specimen.CollectionSite == null) ? null : new[]
                        {
                            specimen.CollectionSite.Country,
                            specimen.CollectionSite.State
                        }.Concatenate(", "));
            }

            return summaryBuilder
                .ToString();
        }
    }
}