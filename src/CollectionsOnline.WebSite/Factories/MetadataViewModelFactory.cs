using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Core.Utilities;
using CollectionsOnline.WebSite.Extensions;
using CollectionsOnline.WebSite.Models;
using Raven.Abstractions.Extensions;

namespace CollectionsOnline.WebSite.Factories
{
    // TODO: Eventually move to view model factory when each module has its own
    public class MetadataViewModelFactory : IMetadataViewModelFactory
    {
        public MetadataViewModel Make(Article article)
        {
            // General metadata
            var metadata = new MetadataViewModel
            {
                Description = article.ContentText
                    .RemoveLineBreaks()
                    .Truncate(Constants.MetadataDescriptionMaxChars, " ..."),
                Title = HtmlConverter.HtmlToText(article.DisplayTitle),
                MetaProperties = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("og:type", "article"),                        
                }
            };

            // Twitter
            if (article.Media.WithThumbnails().Any())
                metadata.MetaProperties.Add(new KeyValuePair<string, string>("twitter:card", "summary_large_image"));
            else
                metadata.MetaProperties.Add(new KeyValuePair<string, string>("twitter:card", "summary"));

            // Image metadata
            metadata.MetaProperties.AddRange(CreateImageMetadata(article.Media));           

            return metadata;
        }

        public MetadataViewModel Make(Collection collection)
        {
            // General metadata
            var metadata = new MetadataViewModel
            {
                Description = HtmlConverter
                    .HtmlToText(collection.CollectionSummary)
                    .RemoveLineBreaks()
                    .Truncate(Constants.MetadataDescriptionMaxChars, " ..."),
                Title = HtmlConverter.HtmlToText(collection.DisplayTitle),
                MetaProperties = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("og:type", "article"),
                }
            };

            // Twitter
            if (collection.Media.WithThumbnails().Any())
                metadata.MetaProperties.Add(new KeyValuePair<string, string>("twitter:card", "summary_large_image"));
            else
                metadata.MetaProperties.Add(new KeyValuePair<string, string>("twitter:card", "summary"));

            // Image metadata
            metadata.MetaProperties.AddRange(CreateImageMetadata(collection.Media));

            return metadata;
        }

        public MetadataViewModel Make(Item item)
        {
            // General metadata
            var metadata = new MetadataViewModel
            {
                Title = HtmlConverter.HtmlToText(item.DisplayTitle)
            };

            if (!string.IsNullOrWhiteSpace(item.ObjectSummary))
                metadata.Description = item.ObjectSummary;
            else if (!string.IsNullOrWhiteSpace(item.IsdDescriptionOfContent))
                metadata.Description = item.IsdDescriptionOfContent;
            else if (!string.IsNullOrWhiteSpace(item.PhysicalDescription))
                metadata.Description = item.PhysicalDescription;
            else if (!string.IsNullOrWhiteSpace(item.IndigenousCulturesDescription))
                metadata.Description = item.IndigenousCulturesDescription;
            else if (!string.IsNullOrWhiteSpace(item.ArcheologyDescription))
                metadata.Description = item.ArcheologyDescription;
            else if (!string.IsNullOrWhiteSpace(item.Significance))
                metadata.Description = item.Significance;

            if (!string.IsNullOrWhiteSpace(metadata.Description))
            {
                metadata.Description = metadata.Description
                    .Truncate(Constants.MetadataDescriptionMaxChars, " ...")
                    .RemoveLineBreaks();
            }

            // Twitter
            if (item.Media.WithThumbnails().Any())
                metadata.MetaProperties.Add(new KeyValuePair<string, string>("twitter:card", "summary_large_image"));
            else
                metadata.MetaProperties.Add(new KeyValuePair<string, string>("twitter:card", "summary"));


            // Image metadata
            metadata.MetaProperties.AddRange(CreateImageMetadata(item.Media));

            return metadata;
        }

        public MetadataViewModel Make(Species species)
        {
            // General metadata
            var metadata = new MetadataViewModel
            {
                Title = HtmlConverter.HtmlToText(species.DisplayTitle),
                MetaProperties = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("og:type", "article"),                        
                }
            };

            if (!string.IsNullOrWhiteSpace(species.Biology))
                metadata.Description = species.Biology;
            else if (!string.IsNullOrWhiteSpace(species.GeneralDescription))
                metadata.Description = species.GeneralDescription;

            if (!string.IsNullOrWhiteSpace(metadata.Description))
            {
                metadata.Description = metadata.Description
                    .Truncate(Constants.MetadataDescriptionMaxChars, " ...")
                    .RemoveLineBreaks();
            }

            if (species.Media.WithThumbnails().Any())
                metadata.MetaProperties.Add(new KeyValuePair<string, string>("twitter:card", "summary_large_image"));
            else
                metadata.MetaProperties.Add(new KeyValuePair<string, string>("twitter:card", "summary"));

            // Image metadata
            metadata.MetaProperties.AddRange(CreateImageMetadata(species.Media));

            return metadata;
        }

        public MetadataViewModel Make(Specimen specimen)
        {
            // General metadata
            var metadata = new MetadataViewModel
            {
                Title = HtmlConverter.HtmlToText(specimen.DisplayTitle),
            };

            if (!string.IsNullOrWhiteSpace(specimen.ObjectSummary))
                metadata.Description = specimen.ObjectSummary;
            else if (!string.IsNullOrWhiteSpace(specimen.IsdDescriptionOfContent))
                metadata.Description = specimen.IsdDescriptionOfContent;
            else if (!string.IsNullOrWhiteSpace(specimen.Significance))
                metadata.Description = specimen.Significance;
            else if (string.IsNullOrWhiteSpace(metadata.Description))
                metadata.Description = string.Equals(specimen.ScientificGroup, "Geology", StringComparison.OrdinalIgnoreCase)
                    ? new[]
                    {
                        specimen.MineralogyTypeOfType,
                        specimen.MineralogyClass,
                        specimen.MeteoritesClass,
                        specimen.MineralogyGroup,
                        specimen.MeteoritesGroup,
                        specimen.TektitesClassification,
                        specimen.TektitesShape
                    }.Concatenate(", ")
                    : new[]
                    {
                        (specimen.Taxonomy == null)
                            ? null
                            : new[]
                            {
                                specimen.Taxonomy.CommonName,
                                specimen.Taxonomy.TaxonName
                            }.Concatenate(", "),
                        (specimen.CollectionSite == null)
                            ? null
                            : new[]
                            {
                                specimen.CollectionSite.Country,
                                specimen.CollectionSite.State
                            }.Concatenate(", "),
                    }.Concatenate(", ");

            if (!string.IsNullOrWhiteSpace(metadata.Description))
            {
                metadata.Description = metadata.Description
                    .Truncate(Constants.MetadataDescriptionMaxChars, " ...")
                    .RemoveLineBreaks();
            }

            // Twitter
            if(specimen.Media.WithThumbnails().Any())
                metadata.MetaProperties.Add(new KeyValuePair<string, string>("twitter:card", "summary_large_image"));
            else
                metadata.MetaProperties.Add(new KeyValuePair<string, string>("twitter:card", "summary"));

            // Image metadata
            metadata.MetaProperties.AddRange(CreateImageMetadata(specimen.Media));

            return metadata;
        }

        public MetadataViewModel MakeHomeIndex()
        {
            var metadata = new MetadataViewModel
            {
                Description = "Explore the fascinating collections of Museums Victoria in Melbourne, Australia, featuring over 1.15 million records of zoology, geology, palaeontology, history, indigenous cultures and technology.",
                Title = "Explore Museums Victoria's humanities and natural sciences collections",
                CanonicalUri = ConfigurationManager.AppSettings["CanonicalSiteBase"]
            };

            return metadata;
        }

        public MetadataViewModel MakeAboutIndex()
        {
            var metadata = new MetadataViewModel
            {
                Description = "This site allows users to explore the natural sciences and humanities collections of Museums Victoria in Australia, featuring collections of zoology, geology, palaeontology, history, indigenous cultures and technology. Over 1.15 million records were presented at launch in 2015, accompanied by over 150,000 images.",
                Title = "About Museums Victoria Collections",
            };

            return metadata;
        }

        public MetadataViewModel MakeCollectionIndex()
        {
            var metadata = new MetadataViewModel
            {
                Description = "Descriptions of the types of collections held at Museums Victoria in Melbourne, Australia.",
                Title = "Descriptions of the collections held at Museums Victoria"
            };

            return metadata;
        }

        public MetadataViewModel MakeDevelopersIndex()
        {
            var metadata = new MetadataViewModel
            {
                Description = "Use our public API to extract data from over 1.15 million humanities and natural sciences records that document Museums Victoria's collections.",
                Title = "Developer tools for Museums Victoria Collections"
            };

            return metadata;
        }

        public MetadataViewModel MakeSearchIndex()
        {
            var metadata = new MetadataViewModel
            {
                Description = "Search a database of over 1.15 million records of zoology, geology, palaeontology, history, indigenous cultures and technology from Museums Victoria's collections.",
                Title = "Search Museums Victoria's collections"
            };

            return metadata;
        }

        private IEnumerable<KeyValuePair<string, string>> CreateImageMetadata(IList<Media> media)
        {
            var metaProperties = new List<KeyValuePair<string, string>>();

            // Remove trailing slash for building our image uri's
            var canonicalSiteBase = ConfigurationManager.AppSettings["CanonicalSiteBase"];

            var image = media.OfType<ImageMedia>().FirstOrDefault();
            var video = media.OfType<VideoMedia>().FirstOrDefault();
            if (image != null)
            {
                metaProperties.Add(new KeyValuePair<string, string>("og:image", string.Format("{0}{1}", canonicalSiteBase, image.Medium.Uri)));
                metaProperties.Add(new KeyValuePair<string, string>("og:image:type", "image/jpeg"));
                metaProperties.Add(new KeyValuePair<string, string>("og:image:width", image.Medium.Width.ToString()));
                metaProperties.Add(new KeyValuePair<string, string>("og:image:height", image.Medium.Height.ToString()));
                metaProperties.Add(new KeyValuePair<string, string>("twitter:image", string.Format("{0}{1}", canonicalSiteBase, image.Small.Uri)));
            }
            if (video != null)
            {
                if (image == null)
                    metaProperties.Add(new KeyValuePair<string, string>("og:image", string.Format("{0}{1}", canonicalSiteBase, video.Small.Uri)));

                metaProperties.Add(new KeyValuePair<string, string>("og:video", string.Format("https://www.youtube.com/embed/{0}", video.VideoId)));
                metaProperties.Add(new KeyValuePair<string, string>("og:video:type", "application/x-shockwave-flash"));
            }

            return metaProperties;
        }
    }
}