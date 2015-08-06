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
                    .Truncate(Constants.MetadataDescriptionMaxChars),
                Title = article.DisplayTitle,
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
                    .Truncate(Constants.MetadataDescriptionMaxChars),
                Title = collection.DisplayTitle,
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
                Title = item.ObjectName
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

            metadata.Description = metadata.Description
                .Truncate(Constants.MetadataDescriptionMaxChars)
                .RemoveLineBreaks();

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
                Title = species.DisplayTitle,
                MetaProperties = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("og:type", "article"),                        
                }
            };

            if (!string.IsNullOrWhiteSpace(species.Biology))
                metadata.Description = species.Biology;
            else if (!string.IsNullOrWhiteSpace(species.GeneralDescription))
                metadata.Description = species.GeneralDescription;

            metadata.Description = metadata.Description
                .Truncate(Constants.MetadataDescriptionMaxChars)
                .RemoveLineBreaks();

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
                Description = HtmlConverter.HtmlToText(specimen.Summary),
            };

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
                Description = "Museum Victoria Collections home page description goes here",
                Title = "Museum Victoria Collections",
                CanonicalUri = ConfigurationManager.AppSettings["CanonicalSiteBase"]
            };

            return metadata;
        }

        public MetadataViewModel MakeCollectionIndex()
        {
            var metadata = new MetadataViewModel
            {
                Description = "At Museum Victoria, we have been building and researching our collections since 1854.  Our priceless collections record Australia's environmental and cultural history. They are an irreplaceable resource for understanding the past, reflecting on the present and looking into the future.",
                Title = "Our Collections at Museum Victoria"
            };

            return metadata;
        }

        public MetadataViewModel MakeDevelopersIndex()
        {
            var metadata = new MetadataViewModel
            {
                Description = "Get access to our collections data via our handy web based API.",
                Title = "Our API"
            };

            return metadata;
        }

        public MetadataViewModel MakeSearchIndex()
        {
            var metadata = new MetadataViewModel
            {
                Description = "Search our massive online catalog containing over one million objects and articles.",
                Title = "Search our Collections"
            };

            return metadata;
        }

        private IEnumerable<KeyValuePair<string, string>> CreateImageMetadata(IList<Media> media)
        {
            var metaProperties = new List<KeyValuePair<string, string>>();

            // Remove trailing slash for building our image uri's
            var canonicalSiteBase = ConfigurationManager.AppSettings["CanonicalSiteBase"].Substring(0, ConfigurationManager.AppSettings["CanonicalSiteBase"].LastIndexOf('/')); ;

            var image = media.OfType<ImageMedia>().FirstOrDefault();
            var video = media.OfType<VideoMedia>().FirstOrDefault();
            if (image != null)
            {
                metaProperties.Add(new KeyValuePair<string, string>("og:image", string.Format("{0}{1}", canonicalSiteBase, image.Large.Uri)));
                metaProperties.Add(new KeyValuePair<string, string>("og:image:type", "image/jpeg"));
                metaProperties.Add(new KeyValuePair<string, string>("og:image:width", image.Large.Width.ToString()));
                metaProperties.Add(new KeyValuePair<string, string>("og:image:height", image.Large.Height.ToString()));
                metaProperties.Add(new KeyValuePair<string, string>("twitter:image", string.Format("{0}{1}", canonicalSiteBase, image.Medium.Uri)));
            }
            if (video != null)
            {
                if (image == null)
                    metaProperties.Add(new KeyValuePair<string, string>("og:image", string.Format("{0}{1}", canonicalSiteBase, video.Medium.Uri)));

                metaProperties.Add(new KeyValuePair<string, string>("og:video", string.Format("https://www.youtube.com/embed/{0}", video.VideoId)));
                metaProperties.Add(new KeyValuePair<string, string>("og:video:type", "application/x-shockwave-flash"));
            }

            return metaProperties;
        }
    }
}