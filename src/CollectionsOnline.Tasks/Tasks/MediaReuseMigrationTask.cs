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
using Raven.Client.Document;

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

                TransferMediaReuses(stoppingToken);
            }
        }, stoppingToken);
    }

    private void UpgradeDeprecatedMediaReuses(CancellationToken stoppingToken)
    {
        var count = 0;
        var deserializableMediaResues = 0;
        var deprecatedMediaReuses = 0;
        var recreatedMedia = 0;

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

                if (media == null)
                {
                    media = new ImageMedia
                    {
                        Irn = mediaIrn
                    };

                    Log.Logger.Debug(
                        "Could not find media with Irn {mediaIrn} attached to target document {documentId}, recreating Media with Irn only",
                        mediaIrn, documentId);

                    recreatedMedia++;
                }

                var mediaReuse = new MediaReuse
                {
                    Id = mediaReuseId,
                    Usage = usage,
                    UsageMore = usageMore,
                    DocumentId = documentId,
                    Media = media
                };

                mediaDocumentSession.Store(mediaReuse);
                mediaDocumentSession.SaveChanges();

                deprecatedMediaReuses++;
            }

            count++;

            if (count % 100 == 0)
                Log.Logger.Information(
                    "Upgrading of deprecated MediaReuses progress... {Offset}/{TotalResults}",
                    count, queryHeaderInformation.TotalResults);
        }

        Log.Logger.Information(
            "MediaReuse upgrading complete, deserializable MediaResues:{deserializableMediaResues}, deprecated MediaReuses:{deprecatedMediaReuses}, recreated Media with Irn only:{recreatedMedia}",
            deserializableMediaResues, deprecatedMediaReuses, recreatedMedia);
    }

    private void TransferMediaReuses(CancellationToken stoppingToken)
    {
        foreach (var sourceDatabase in _appSettings.MediaReuseMigrationTask.SourceDatabases)
        {
            stoppingToken.ThrowIfCancellationRequested();

            Log.Logger.Information("Fetching MediaReuses from Source Database {Url}{Name}",
                sourceDatabase.Url, sourceDatabase.Name);

            var sourceMediaReuses = new List<MediaReuse>();

            using var sourceDocumentStore = new DocumentStore
            {
                Url = sourceDatabase.Url,
                DefaultDatabase = sourceDatabase.Name
            }.Initialize();

            var enumerator = sourceDocumentStore
                .DatabaseCommands
                .StreamQuery(
                    new RavenDocumentsByEntityName().IndexName,
                    new IndexQuery
                    {
                        Query = "__document_id:mediareuses*"
                    },
                    out var queryHeaderInformation);

            var count = 0;

            while (enumerator.MoveNext())
            {
                stoppingToken.ThrowIfCancellationRequested();

                var sourceMediaReuse = enumerator.Current.Deserialize<MediaReuse>(sourceDocumentStore.Conventions);
                sourceMediaReuse.Id = "mediareuses/";

                sourceMediaReuses.Add(sourceMediaReuse);

                count++;

                if (count % 100 == 0)
                    Log.Logger.Information(
                        "Fetching of MediaReuses from Source Database {Url}{Name} progress... {Offset}/{TotalResults}",
                        sourceDatabase.Url, sourceDatabase.Name, count, queryHeaderInformation.TotalResults);
            }

            Log.Logger.Information("Saving {Count} MediaReuses to destination database {Url}{Name}",
                sourceMediaReuses.Count, _appSettings.DatabaseUrl, _appSettings.DatabaseName);

            using var bulkInsert = _documentStore.BulkInsert();

            foreach (var mediaReuse in sourceMediaReuses) bulkInsert.Store(mediaReuse);
        }
    }

    public int Order => 100;

    public bool Enabled => false;
}