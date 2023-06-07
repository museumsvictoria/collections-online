using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Models;
using Microsoft.Extensions.Options;
using Raven.Client;
using Raven.Client.Linq;
using Constants = CollectionsOnline.Core.Config.Constants;

namespace CollectionsOnline.Tasks.Tasks;

public class OnLoanDateCheckTask : ITask
{
    private readonly IDocumentStore _documentStore;

    public OnLoanDateCheckTask(
        IDocumentStore documentStore)
    {
        _documentStore = documentStore;
    }

    public async Task Run(CancellationToken stoppingToken)
    {
        await Task.Run(() =>
        {
            using (Log.Logger.BeginTimedOperation("On Loan Date Check task starting", "OnLoanDateCheckTask.Run"))
            {
                var count = 0;

                while (true)
                {
                    using var documentSession = _documentStore.OpenSession();

                    // Find on loan records
                    var documentBatch = documentSession
                        .Query<object, CombinedIndex>()
                        .Where(x => ((CombinedIndexResult)x).DisplayStatus == "On loan")
                        .Skip(count)
                        .Take(Constants.DataBatchSize)
                        .ToList();
                    
                    if (documentBatch.Count == 0)
                        break;

                    // Check each record to see if the start/end date is out of range and if it is, replace museum location with not on display
                    foreach (var document in documentBatch)
                    {
                        // Can only ever be Item or Specimen
                        switch (document)
                        {
                            case Item item:
                                if (DateIsOutOfRange(item.MuseumLocation))
                                {
                                    LogOutOfRangeRecord(item.Id, item.MuseumLocation.DisplayStartDate, item.MuseumLocation.DisplayEndDate);
                                    item.MuseumLocation = new MuseumLocation() { DisplayStatus = DisplayStatus.NotOnDisplay };
                                }
                                break;
                            case Specimen specimen:
                                if (DateIsOutOfRange(specimen.MuseumLocation))
                                {
                                    LogOutOfRangeRecord(specimen.Id, specimen.MuseumLocation.DisplayStartDate, specimen.MuseumLocation.DisplayEndDate);
                                    specimen.MuseumLocation = new MuseumLocation() { DisplayStatus = DisplayStatus.NotOnDisplay };
                                }
                                break;
                        }
                    }

                    // Save any changes
                    documentSession.SaveChanges();
                    count += documentBatch.Count;
                }
            }
        }, stoppingToken);
    }

    private void LogOutOfRangeRecord(string id, DateTime? startDate, DateTime? endDate)
    {
        Log.Logger.Information(
            "Found on loan record {Id} with out of range loan dates {StartDate}/{EndDate}, setting to not on display",
            id, startDate, endDate);
    }

    private bool DateIsOutOfRange(MuseumLocation location) => !(DateTime.Now >= location.DisplayStartDate && DateTime.Now <= location.DisplayEndDate);

    public int Order => 1;

    public bool Enabled => true;
}