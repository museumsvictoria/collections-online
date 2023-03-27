using System.IO;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.WebSite.Models;
using CsvHelper;
using Nancy;
using Nancy.Responses;

namespace CollectionsOnline.WebSite.Queries
{
    public class CsvResponseQuery : ICsvResponseQuery
    {
        public Response BuildCsvResponse(SearchIndexCsvModel searchIndexCsvModel)
        {
            var writer = new StreamWriter(new MemoryStream()) { AutoFlush = true };
            var csv = new CsvWriter(writer);

            csv.WriteRecords(searchIndexCsvModel.Results);
            writer.BaseStream.Position = 0;

            var response = new StreamResponse(() => writer.BaseStream, "text/csv");

            response.WithHeader("Content-Disposition", $"attachment; filename=\"{searchIndexCsvModel.SearchInputModel.ToString().Truncate(Constants.FileMaxChars, "...")}.csv\"");

            return response;
        }
    }
}