using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Factories;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Infrastructure;
using Microsoft.Extensions.Options;
using Raven.Client;

namespace CollectionsOnline.Tasks.Tasks
{
    public class SitemapGeneratorTask : ITask
    {
        private readonly IDocumentStore _documentStore;
        private readonly AppSettings _appSettings;

        public SitemapGeneratorTask(
            IOptions<AppSettings> appSettings,
            IDocumentStore documentStore)
        {
            _appSettings = appSettings.Value;
            _documentStore = documentStore;
        }

        public async Task Run(CancellationToken stoppingToken)
        {
            await Task.Run(() =>
            {
                using (Log.Logger.BeginTimedOperation("Sitemap creation task starting", "SitemapGeneratorTask.Run"))
                {
                    NetworkShareAccesser networkShareAccesser = null;
                    if (!string.IsNullOrWhiteSpace(_appSettings.WebSiteDomain))
                    {
                        networkShareAccesser =
                            NetworkShareAccesser.Access(_appSettings.WebSiteComputer,
                                _appSettings.WebSiteDomain,
                                _appSettings.WebSiteUser,
                                _appSettings.WebSitePassword);
                    }

                    try
                    {
                        var count = 0;

                        var sitemapNodes = new List<SitemapNode>();
                        var sitemapNodeIndexes = new List<List<SitemapNode>>();
                        var skip = 0;

                        while (!stoppingToken.IsCancellationRequested)
                        {
                            using (var documentSession = _documentStore.OpenSession())
                            {
                                var query = documentSession.Advanced
                                    .DocumentQuery<CombinedIndexResult, CombinedIndex>()
                                    .SelectFields<SitemapResult>()
                                    .Skip(skip)
                                    .Take(Constants.PagingStreamSize);

                                var enumerator = documentSession.Advanced.Stream(query, out var queryHeaderInformation);

                                // Create all sitemap nodes
                                while (enumerator.MoveNext() &&
                                       !stoppingToken.IsCancellationRequested)
                                {
                                    sitemapNodes.Add(new SitemapNode(new Uri(
                                            $"{_appSettings.CanonicalSiteBase}/{enumerator.Current.Document.Id}"),
                                        enumerator.Current.Document.DateModified));

                                    count++;

                                    // Add sitemap to urlset once we reach our maximum url limit and create a new list to hold the next batch
                                    if (count % Constants.MaxSitemapUrls == 0)
                                    {
                                        sitemapNodeIndexes.Add(sitemapNodes);
                                        sitemapNodes = new List<SitemapNode>();
                                    }
                                }

                                stoppingToken.ThrowIfCancellationRequested();

                                skip += Constants.PagingStreamSize;

                                Log.Logger.Information("Sitemap node creation progress... {Offset}/{TotalResults}",
                                    count,
                                    queryHeaderInformation.TotalResults);

                                if (count == queryHeaderInformation.TotalResults)
                                    break;
                            }
                        }

                        stoppingToken.ThrowIfCancellationRequested();

                        // Add the last sitemapNodes to our index
                        if (sitemapNodes.Any())
                            sitemapNodeIndexes.Add(sitemapNodes);

                        // Create destination folder
                        PathFactory.CreateDestPath($"{_appSettings.WebSitePath}\\sitemaps\\");

                        // Create then save sitemaps as xml
                        count = 0;
                        XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
                        var utf8WithoutBom = new System.Text.UTF8Encoding(false);
                        var sitemapIndex = new XElement(xmlns + "sitemapindex");

                        foreach (var sitemapNodeIndex in sitemapNodeIndexes)
                        {
                            stoppingToken.ThrowIfCancellationRequested();

                            count++;

                            sitemapIndex.Add(new XElement(xmlns + "sitemap",
                                new XElement(xmlns + "loc",
                                    Uri.EscapeDataString(string.Format("{0}/sitemap-set-{1}.xml.gz",
                                        _appSettings.CanonicalSiteBase,
                                        count))),
                                new XElement(xmlns + "lastmod",
                                    sitemapNodeIndex.OrderByDescending(x => x.LastModified).Select(x => x.LastModified)
                                        .First()
                                        .ToString("yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture))));

                            var sitemapUrlset = new XElement(xmlns + "urlset");

                            foreach (var sitemapNode in sitemapNodeIndex)
                            {
                                stoppingToken.ThrowIfCancellationRequested();

                                var sitemapUrl = new XElement(xmlns + "url",
                                    new XElement(xmlns + "loc", Uri.EscapeDataString(sitemapNode.Url.AbsoluteUri)),
                                    new XElement(xmlns + "lastmod",
                                        sitemapNode.LastModified.ToString("yyyy-MM-ddTHH:mm:sszzz",
                                            CultureInfo.InvariantCulture)),
                                    new XElement(xmlns + "changefreq",
                                        sitemapNode.Frequency.ToString().ToLowerInvariant()));

                                if (sitemapNode.Priority.HasValue)
                                {
                                    sitemapUrl.Add(new XElement(xmlns + "priority",
                                        sitemapNode.Priority.ToString().ToLowerInvariant()));
                                }

                                sitemapUrlset.Add(sitemapUrl);
                            }

                            stoppingToken.ThrowIfCancellationRequested();

                            // Save sitemap set with gzip compression
                            using (var file = File.Open(
                                $"{_appSettings.WebSitePath}\\sitemaps\\sitemap-set-{count}.xml.gz", FileMode.Create,
                                FileAccess.Write))
                            using (var gzip = new GZipStream(file, CompressionMode.Compress, false))
                            {
                                Log.Logger.Information(
                                    "Saving sitemap set to {websitepath}\\sitemaps\\sitemap-set-{count}.xml.gz",
                                    _appSettings.WebSitePath, count);
                                sitemapUrlset.Save(gzip);
                            }
                        }

                        stoppingToken.ThrowIfCancellationRequested();

                        // Save sitemap index
                        using (var fileWriter = new StreamWriter(
                            $"{_appSettings.WebSitePath}\\sitemaps\\sitemap.xml", false, utf8WithoutBom))
                        {
                            Log.Logger.Information("Saving sitemap index to {websitepath}\\sitemaps\\sitemap.xml",
                                _appSettings.WebSitePath);
                            sitemapIndex.Save(fileWriter);
                        }
                    }
                    finally
                    {
                        networkShareAccesser?.Dispose();
                    }
                }

                return Task.CompletedTask;
            }, stoppingToken);
        }

        public int Order => 100;

        public bool Enabled => true;
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