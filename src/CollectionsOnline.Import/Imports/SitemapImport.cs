using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;
using CollectionsOnline.Core.Indexes;
using NLog;
using Raven.Abstractions.Data;
using Raven.Client;
using Constants = CollectionsOnline.Core.Config.Constants;

namespace CollectionsOnline.Import.Imports
{
    public class SitemapImport : IImport
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly IDocumentStore _documentStore;

        public SitemapImport(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public void Run()
        {            
            _log.Debug("Beginning sitemap creation");

            var count = 0;

            var sitemapNodes = new List<SitemapNode>();
            var sitemapNodeIndexes = new List<List<SitemapNode>>();
            var skip = 0;
            
            while (true)
            {
                using (var documentSession = _documentStore.OpenSession())
                {
                    var query = documentSession.Advanced
                        .DocumentQuery<CombinedIndexResult, CombinedIndex>()
                        .SelectFields<SitemapResult>()
                        .Skip(skip)
                        .Take(Constants.PagingStreamSize);

                    QueryHeaderInformation queryHeaderInformation;
                    var enumerator = documentSession.Advanced.Stream(query, out queryHeaderInformation);

                    // Create all sitemap nodes
                    while (enumerator.MoveNext())
                    {
                        if (ImportCanceled())
                            return;

                        sitemapNodes.Add(new SitemapNode(new Uri(string.Format("{0}/{1}",
                            ConfigurationManager.AppSettings["CanonicalSiteBase"], enumerator.Current.Document.Id)),
                            enumerator.Current.Document.DateModified));

                        count++;

                        // Add sitemap to urlset once we reach our maximum url limit and create a new list to hold the next batch
                        if (count % Constants.MaxSitemapUrls == 0)
                        {
                            sitemapNodeIndexes.Add(sitemapNodes);
                            sitemapNodes = new List<SitemapNode>();
                        }
                    }

                    _log.Trace("Sitemap caching progress... {0}/{1}", count, queryHeaderInformation.TotalResults);

                    skip += Constants.PagingStreamSize;

                    if (count == queryHeaderInformation.TotalResults)
                        break;
                }
            }

            // Add the last sitemapNodes to our index
            if(sitemapNodes.Any())
                sitemapNodeIndexes.Add(sitemapNodes);

            // Create destination folder
            var directoryInfo = new DirectoryInfo(string.Format("{0}\\sitemaps", ConfigurationManager.AppSettings["WebSitePath"]));
            if (!directoryInfo.Exists)
                directoryInfo.Create();

            // Create then save sitemaps as xml
            count = 0;
            XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var utf8WithoutBom = new System.Text.UTF8Encoding(false);
            var sitemapIndex = new XElement(xmlns + "sitemapindex");

            foreach (var sitemapNodeIndex in sitemapNodeIndexes)
            {
                count++;

                sitemapIndex.Add(new XElement(xmlns + "sitemap",
                    new XElement(xmlns + "loc", Uri.EscapeUriString(string.Format("{0}/sitemap-set-{1}.xml.gz", ConfigurationManager.AppSettings["CanonicalSiteBase"], count))),
                    new XElement(xmlns + "lastmod", sitemapNodeIndex.OrderByDescending(x => x.LastModified).Select(x => x.LastModified).First().ToString("yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture))));

                var sitemapUrlset = new XElement(xmlns + "urlset");
                foreach (var sitemapNode in sitemapNodeIndex)
                {
                    var sitemapUrl = new XElement(xmlns + "url",
                        new XElement(xmlns + "loc", Uri.EscapeUriString(sitemapNode.Url.AbsoluteUri)),
                        new XElement(xmlns + "lastmod", sitemapNode.LastModified.ToString("yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture)),
                        new XElement(xmlns + "changefreq", sitemapNode.Frequency.ToString().ToLowerInvariant()));

                    if (sitemapNode.Priority.HasValue)
                    {
                        sitemapUrl.Add(new XElement(xmlns + "priority", sitemapNode.Priority.ToString().ToLowerInvariant()));
                    }

                    sitemapUrlset.Add(sitemapUrl);
                }

                // Save sitemap set with gzip compression
                using (var file = File.Create(string.Format("{0}\\sitemaps\\sitemap-set-{1}.xml.gz", ConfigurationManager.AppSettings["WebSitePath"], count)))
                using (var gzip = new GZipStream(file, CompressionMode.Compress, false))
                {
                    sitemapUrlset.Save(gzip);
                }
            }

            // Save sitemap index
            using (var fileWriter = new StreamWriter(string.Format("{0}\\sitemaps\\sitemap.xml", ConfigurationManager.AppSettings["WebSitePath"]), false, utf8WithoutBom))
            {
                sitemapIndex.Save(fileWriter);
            }

            _log.Debug("Sitemap creation complete");
        }

        public int Order
        {
            get { return 300; }
        }

        private bool ImportCanceled()
        {
            return Program.ImportCanceled;
        }
    }

    public class SitemapResult
    {
        public string Id { get; set; }

        public DateTime DateModified { get; set; }
    }

    public class SitemapNode
    {
        public SitemapNode(
            Uri url,
            DateTime lastModified)
        {
            Url = url;
            Frequency = SitemapFrequency.Daily;
            LastModified = lastModified;
        }

        public Uri Url { get; set; }

        public DateTimeOffset LastModified { get; set; }

        public SitemapFrequency Frequency { get; set; }

        public double? Priority { get; set; }
    }

    public enum SitemapFrequency
    {
        Never,
        Yearly,
        Monthly,
        Weekly,
        Daily,
        Hourly,
        Always
    }
}