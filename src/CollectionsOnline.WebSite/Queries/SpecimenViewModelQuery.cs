using System.Collections.Generic;
using System.Linq;
using CollectionsOnline.Core.Indexes;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Transformers;
using Newtonsoft.Json;
using Raven.Client;

namespace CollectionsOnline.WebSite.Queries
{
    public class SpecimenViewModelQuery : ISpecimenViewModelQuery
    {
        private readonly IDocumentSession _documentSession;

        public SpecimenViewModelQuery(
            IDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }

        public SpecimenViewTransformerResult BuildSpecimen(string specimenId)
        {
            var result = _documentSession.Load<SpecimenViewTransformer, SpecimenViewTransformerResult>(specimenId);

            if (result.Specimen.Taxonomy != null)
            {
                var query = _documentSession.Advanced
                    .DocumentQuery<CombinedIndexResult, CombinedIndex>()
                    .WhereEquals("Taxon", result.Specimen.Taxonomy.TaxonName)
                    .Take(1);

                // Dont allow a link to search page if the current specimen is the only result
                if (query.SelectFields<CombinedIndexResult>("Id").Select(x => x.Id).Except(new[] { specimenId }).Any())
                {
                    result.RelatedSpeciesSpecimenItemCount = query.QueryResult.TotalResults;
                }
            }

            // Set Media            
            result.SpecimenImages = result.Specimen.Media.Where(x => x is ImageMedia).Cast<ImageMedia>().ToList();
            result.SpecimenFiles = result.Specimen.Media.Where(x => x is FileMedia).Cast<FileMedia>().ToList();
            result.JsonSpecimenImages = JsonConvert.SerializeObject(result.SpecimenImages);

            // Map
            var latlongs = new List<double[]>();
            if (result.Specimen.Latitudes.Any(x => !string.IsNullOrWhiteSpace(x)) && result.Specimen.Longitudes.Any(x => !string.IsNullOrWhiteSpace(x)))
            {
                latlongs = result.Specimen.Latitudes.Zip(result.Specimen.Longitudes, (lat, lon) => new[] { double.Parse(lat), double.Parse(lon) }).ToList();
            }
            result.JsonSpecimenLatLongs = JsonConvert.SerializeObject(latlongs); 

            return result;
        }
    }
}