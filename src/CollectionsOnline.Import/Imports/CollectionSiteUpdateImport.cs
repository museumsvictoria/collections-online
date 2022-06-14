using System;
using System.Linq;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Import.Extensions;
using CollectionsOnline.Import.Factories;
using CollectionsOnline.Import.Infrastructure;
using IMu;
using Raven.Abstractions.Extensions;
using Raven.Client;
using Serilog;
using Constants = CollectionsOnline.Core.Config.Constants;

namespace CollectionsOnline.Import.Imports
{
    public class CollectionSiteUpdateImport : ImuImportBase
    {
        private readonly IDocumentStore _documentStore;
        private readonly IImuSessionProvider _imuSessionProvider;
        private readonly ICollectionSiteFactory _collectionSiteFactory;

        public CollectionSiteUpdateImport(
            IDocumentStore documentStore,
            IImuSessionProvider imuSessionProvider,
            ICollectionSiteFactory collectionSiteFactory)
        {
            _documentStore = documentStore;
            _imuSessionProvider = imuSessionProvider;
            _collectionSiteFactory = collectionSiteFactory;
        }

        public override void Run()
        {
            using (Log.Logger.BeginTimedOperation("Collection Site Update Import starting", "CollectionSiteUpdateImport.Run"))
            { 
                ImportCache importCache;
                var columns = new[] {
                                        "SitSiteCode",
                                        "SitSiteNumber",
                                        "EraEra",
                                        "EraAge1",
                                        "EraAge2",
                                        "EraMvStage",
                                        "EraMvGroup_tab",
                                        "EraMvRockUnit_tab",
                                        "EraMvMember_tab",
                                        "EraLithology_tab",
                                        "geo=[LocOcean_tab,LocContinent_tab,LocCountry_tab,LocProvinceStateTerritory_tab,LocDistrictCountyShire_tab,LocTownship_tab,LocNearestNamedPlace_tab]",
                                        "LocPreciseLocation",
                                        "LocElevationASLFromMt",
                                        "LocElevationASLToMt",
                                        "latlong=[LatLongitudeDecimal_nesttab,LatLatitudeDecimal_nesttab,LatDatum_tab,LatRadiusNumeric_tab,determinedBy=LatDeterminedByRef_tab.(NamPartyType,NamFullName,NamOrganisation,NamBranch,NamDepartment,NamOrganisation,NamOrganisationOtherNames_tab,NamSource,AddPhysStreet,AddPhysCity,AddPhysState,AddPhysCountry,ColCollaborationName),LatDetDate0,LatLatLongDetermination_tab,LatDetSource_tab,LatPreferred_tab]",
                                        "AdmPublishWebNoPassword"
                                    };

                using (var documentSession = _documentStore.OpenSession())
                using (var imuSession = _imuSessionProvider.CreateInstance("esites"))
                {
                    // Check to see whether we need to run import, so grab the earliest previous date run of any imports that utilize collection site.
                    var previousDateRun = documentSession
                        .Load<Application>(Constants.ApplicationId)
                        .ImportStatuses.Where(x => x.ImportType.Contains(typeof(Specimen).Name, StringComparison.OrdinalIgnoreCase))
                        .Select(x => x.PreviousDateRun)
                        .OrderBy(x => x)
                        .FirstOrDefault(x => x.HasValue);

                    // Exit current import if it has never run.
                    if (!previousDateRun.HasValue)
                    {
                        Log.Logger.Information("Dependant imports have never been run... skipping");
                        return;
                    }

                    // Check for existing import in case we need to resume.
                    var importStatus = documentSession.Load<Application>(Constants.ApplicationId).GetImportStatus(GetType().ToString());

                    // Exit current import if it had completed previous time it was run.
                    if (importStatus.IsFinished)
                    {
                        Log.Logger.Information("Import finished last time... skipping");
                        return;
                    }

                    // Check for existing cached results
                    importCache = documentSession.Load<ImportCache>("importCaches/collectionsite");
                    if (importCache == null)
                    {
                        Log.Logger.Information("Caching {Name} results", GetType().Name);
                        importCache = new ImportCache { Id = "importCaches/collectionsite" };

                        var terms = new Terms();
                        terms.Add("AdmDateModified", previousDateRun.Value.ToString("MMM dd yyyy"), ">=");

                        var hits = imuSession.FindTerms(terms);

                        Log.Logger.Information("Found {Hits} records where {@Terms}", hits, terms.List);

                        var cachedCurrentOffset = 0;
                        while (true)
                        {
                            if (ImportCanceled())
                                return;

                            var results = imuSession.Fetch("start", cachedCurrentOffset, Constants.CachedDataBatchSize, new[] { "irn" });

                            if (results.Count == 0)
                                break;

                            importCache.Irns.AddRange(results.Rows.Select(x => long.Parse(x.GetEncodedString("irn"))));

                            cachedCurrentOffset += results.Count;

                            Log.Logger.Information("{Name} cache progress... {Offset}/{TotalResults}", GetType().Name, cachedCurrentOffset, hits);
                        }

                        // Store cached result
                        documentSession.Store(importCache);
                        documentSession.SaveChanges();

                        Log.Logger.Information("Caching of {Name} results complete", GetType().Name);
                    }
                    else
                    {
                        Log.Logger.Information("Cached results found, resuming {Name} import.", GetType().Name);
                    }
                }

                // Perform import
                while (true)
                {
                    using (var documentSession = _documentStore.OpenSession())
                    using (var imuSession = _imuSessionProvider.CreateInstance("esites"))
                    {
                        if (ImportCanceled())
                            return;

                        var importStatus = documentSession.Load<Application>(Constants.ApplicationId).GetImportStatus(GetType().ToString());

                        var cachedResultBatch = importCache.Irns
                            .Skip(importStatus.CurrentImportCacheOffset)
                            .Take(Constants.DataBatchSize)
                            .ToList();

                        if (cachedResultBatch.Count == 0)
                            break;

                        imuSession.FindKeys(cachedResultBatch);

                        var results = imuSession.Fetch("start", 0, -1, columns);

                        Log.Logger.Debug("Fetched {RecordCount} collection site records from Imu", cachedResultBatch.Count);

                        foreach (var row in results.Rows)
                        {
                            var collectionSiteIrn = long.Parse(row.GetString("irn"));

                            var count = 0;
                            while (true)
                            {
                                using (var associatedDocumentSession = _documentStore.OpenSession())
                                {
                                    if (ImportCanceled())
                                        return;

                                    // Find associated documents that utilize the media we are updating in order to update any denormalized references
                                    var associatedDocumentBatch = associatedDocumentSession
                                        .Query<object, CombinedIndex>()
                                        .Where(x => ((CombinedIndexResult)x).CollectionSiteIrn == collectionSiteIrn)
                                        .Skip(count)
                                        .Take(Constants.DataBatchSize)
                                        .ToList();

                                    if (associatedDocumentBatch.Count == 0)
                                        break;

                                    foreach (var document in associatedDocumentBatch)
                                    {
                                        var specimen = document as Specimen;

                                        Log.Logger.Debug("Updating Collection Site on {DocumentId}", specimen.Id);
                                        specimen.CollectionSite = _collectionSiteFactory.Make(row, specimen.Type, specimen.Discipline, specimen.ScientificGroup);
                                    }

                                    // Save any changes
                                    associatedDocumentSession.SaveChanges();
                                    count += associatedDocumentBatch.Count;
                                }
                            }
                        }

                        importStatus.CurrentImportCacheOffset += results.Count;

                        Log.Logger.Information("{Name} import progress... {Offset}/{TotalResults}", GetType().Name, importStatus.CurrentImportCacheOffset, importCache.Irns.Count);
                        documentSession.SaveChanges();
                    }
                }              

                using (var documentSession = _documentStore.OpenSession())
                {
                    // Mark import status as finished
                    documentSession.Load<Application>(Constants.ApplicationId).ImportFinished(GetType().ToString(), importCache.DateCreated);
                    documentSession.SaveChanges();
                }
            }
        }

        public override int Order => 10;
    }
}