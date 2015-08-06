using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Core.Utilities;
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
            metadata.MetaProperties.Add(new KeyValuePair<string, string>("twitter:card", "summary_large_image"));

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
            metadata.MetaProperties.Add(new KeyValuePair<string, string>("twitter:card", "summary_large_image"));

            // Image metadata
            metadata.MetaProperties.AddRange(CreateImageMetadata(collection.Media));

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

        private IEnumerable<KeyValuePair<string, string>> CreateImageMetadata(IList<Media> media)
        {
            var metaProperties = new List<KeyValuePair<string, string>>();

            var image = media.OfType<ImageMedia>().FirstOrDefault();
            var video = media.OfType<VideoMedia>().FirstOrDefault();
            if (image != null)
            {
                metaProperties.Add(new KeyValuePair<string, string>("og:image", string.Format("{0}{1}", ConfigurationManager.AppSettings["CanonicalSiteBase"], image.Large.Uri)));
                metaProperties.Add(new KeyValuePair<string, string>("og:image:type", "image/jpeg"));
                metaProperties.Add(new KeyValuePair<string, string>("og:image:width", image.Large.Width.ToString()));
                metaProperties.Add(new KeyValuePair<string, string>("og:image:height", image.Large.Height.ToString()));
                metaProperties.Add(new KeyValuePair<string, string>("twitter:image", string.Format("{0}{1}", ConfigurationManager.AppSettings["CanonicalSiteBase"], image.Medium.Uri)));
            }
            if (video != null)
            {
                if (image == null)
                    metaProperties.Add(new KeyValuePair<string, string>("og:image", string.Format("{0}{1}", ConfigurationManager.AppSettings["CanonicalSiteBase"], video.Medium.Uri)));

                metaProperties.Add(new KeyValuePair<string, string>("og:video", string.Format("https://www.youtube.com/embed/{0}", video.VideoId)));
                metaProperties.Add(new KeyValuePair<string, string>("og:video:type", "application/x-shockwave-flash"));
            }

            return metaProperties;
        }
    }
}