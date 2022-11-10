using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CollectionsOnline.Core.Models;
using Microsoft.Extensions.Options;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Indexes;
using Raven.Client.Connection;

namespace CollectionsOnline.Tasks.Tasks;

public class MediaReuseMigrationTask : ITask
{
    private readonly IDocumentStore _documentStore;
    private readonly AppSettings _appSettings;

    public MediaReuseMigrationTask(
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
            using (Log.Logger.BeginTimedOperation("MediaReuse Migration task starting", "MediaReuseMigrationTask.Run"))
            {
                UpgradeDeprecatedMediaReuses(stoppingToken);
            }
        }, stoppingToken);
    }

    private void UpgradeDeprecatedMediaReuses(CancellationToken stoppingToken)
    {
        var count = 0;
        var deserializableMediaResues = 0;
        var deprecatedMediaReuses = 0;

        Log.Logger.Information(
            "Attempting to upgrade any deprecated MediaReuses that cannot be deserialized from a RavenJObject");

        var enumerator = _documentStore
            .DatabaseCommands
            .StreamQuery(
                new RavenDocumentsByEntityName().IndexName,
                new IndexQuery
                {
                    Query = "__document_id:mediareuses*"
                },
                out var queryHeaderInformation);

        while (enumerator.MoveNext())
        {
            stoppingToken.ThrowIfCancellationRequested();

            var jObject = enumerator.Current;

            try
            {
                _ = jObject.Deserialize<MediaReuse>(_documentStore.Conventions);

                deserializableMediaResues++;
            }
            catch
            {
                using var mediaDocumentSession = _documentStore.OpenSession();

                var usage = jObject["Usage"].Value<string>();
                var usageMore = jObject["UsageMore"].Value<string>();
                var documentId = jObject["DocumentId"].Value<string>();
                var mediaIrn = long.Parse(jObject["Media"].Value<string>("Irn"));
                var mediaReuseId = jObject["@metadata"].Value<string>("@id");

                var document = mediaDocumentSession.Load<dynamic>(documentId);
                IList<Media> documentMedia = document?.Media;
                var media = documentMedia?.SingleOrDefault(x => x.Irn == mediaIrn);

                var mediaReuse = new MediaReuse
                {
                    Id = mediaReuseId,
                    Usage = usage,
                    UsageMore = usageMore,
                    DocumentId = documentId,
                    Media = media ?? new ImageMedia()
                    {
                        Irn = mediaIrn
                    }
                };

                mediaDocumentSession.Store(mediaReuse);
                mediaDocumentSession.SaveChanges();

                deprecatedMediaReuses++;
            }

            count++;

            if (count % 100 == 0)
                Log.Logger.Information(
                    "UpgradeDeprecatedMediaReuses progress deserializable... {Offset}/{TotalResults}", 
                    count, queryHeaderInformation.TotalResults);
        }
        
        Log.Logger.Information(
            "UpgradeDeprecatedMediaReuses complete, deserializable MediaResues:{deserializableMediaResues} deprecated MediaReuses:{deprecatedMediaReuses} deserializable",
            deserializableMediaResues, deprecatedMediaReuses);
    }

    public int Order => 100;

    public bool Enabled => true;
}