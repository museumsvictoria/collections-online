using System.Linq;
using CollectionsOnline.Core.Models;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace CollectionsOnline.Core.Indexes
{
    public class CombinedIndex : AbstractMultiMapIndexCreationTask<CombinedIndexResult>
    {
        public CombinedIndex()
        {
            AddMap<Article>(articles =>
                from article in articles
                where article.IsHidden == false
                select new
                {
                    // Update fields
                    MediaIrns = new object[] { article.Media.Select(x => x.Irn), article.Authors.Select(x => x.ProfileImage.Irn) },
                    TaxonomyIrn = 0,

                    // Content fields
                    Id = article.Id,
                    DisplayTitle = article.DisplayTitle,
                    Content = new object[] { article.DisplayTitle, article.ContentText, article.ContentSummary, article.Keywords },
                    Summary = article.Summary,
                    ThumbnailUri = article.ThumbnailUri,

                    // Sort fields
                    Quality =
                        ((article.RelatedItemIds.Any() || article.RelatedSpecimenIds.Any()) ? 1 : 0) +
                        ((article.Keywords.Any()) ? 1 : 0) +
                        (article.Media.Count * 2) +
                        ((article.ChildArticleIds.Any()) ? 1 : 0),

                    // Facet fields
                    Type = "Article",
                    Category = (string)null,
                    HasImages = (article.Media.Any()) ? "Yes" : (string)null,
                    OnDisplay = (string)null,
                    CollectionArea = new object[] { },
                    ItemType = (string)null,
                    SpeciesType = (string)null,
                    SpeciesEndemicity = (string)null,
                    SpecimenScientificGroup = (string)null,
                    ArticleType = article.Types,
                    OnDisplayLocation = (string)null,

                    // Term fields
                    Keyword = article.Keywords,
                    Locality = article.Localities,
                    Collection = new object[] { },
                    Date = new object[] { },
                    CulturalGroup = new object[] { },
                    Classification = new object[] { },
                    Name = new object[] { },
                    Technique = (string)null,
                    Denomination = new object[] { },
                    Habitat = new object[] { },
                    Taxon = new object[] { },
                    TypeStatus = (string)null,
                    GeoType = new object[] { },
                    MuseumLocation = new object[] { },
                    Article = new object[] { }
                });

            AddMap<Item>(items => 
                from item in items
                where item.IsHidden == false
                select new
                {
                    // Update fields
                    MediaIrns = item.Media.Select(x => x.Irn),
                    TaxonomyIrn = item.Taxonomy.Irn,

                    // Content fields
                    Id = item.Id,
                    DisplayTitle = item.DisplayTitle,
                    Content = new object[] { item.ObjectName, item.Discipline, item.RegistrationNumber, item.ObjectSummary, item.PhysicalDescription },
                    Summary = item.Summary,
                    ThumbnailUri = item.ThumbnailUri,

                    // Sort fields
                    Quality = 
                        ((!string.IsNullOrWhiteSpace(item.PhysicalDescription) || !string.IsNullOrWhiteSpace(item.ObjectSummary) || !string.IsNullOrWhiteSpace(item.Significance) || !string.IsNullOrWhiteSpace(item.IsdDescriptionOfContent)) ? 1 : 0) + 
                        (item.Media.Count * 2) +
                        ((item.Associations.Any()) ? 1 : 0),

                    // Facet fields
                    Type = "Item",
                    Category = item.Category,
                    HasImages = (item.Media.Any()) ? "Yes" : (string)null,
                    OnDisplay = item.MuseumLocation != null ? "Yes" : (string)null,
                    CollectionArea = item.CollectionAreas,
                    ItemType = item.Type,
                    SpeciesType = (string)null,
                    SpeciesEndemicity = (string)null,
                    SpecimenScientificGroup = (string)null,
                    ArticleType = new object[] { },
                    OnDisplayLocation = item.MuseumLocation.OnDisplayLocation,
                    
                    // Term fields
                    Keyword = new object[] { item.Keywords,
                        item.AudioVisualRecordingDetails, 
                        item.ModelNames, 
                        item.ArcheologyActivity, 
                        item.ArcheologySpecificActivity, 
                        item.ArcheologyDecoration,
                        item.NumismaticsSeries,
                        item.TradeLiteraturePrimarySubject,
                        item.TradeLiteraturePublicationTypes,
                        item.Brands.Select(x => x.ProductType) },
                    Locality = new object[] { item.Associations.Where(x => !string.IsNullOrWhiteSpace(x.Locality)).Select(x => x.Locality), 
                        item.Associations.Where(x => !string.IsNullOrWhiteSpace(x.Region)).Select(x => x.Region), 
                        item.Associations.Where(x => !string.IsNullOrWhiteSpace(x.State)).Select(x => x.State), 
                        item.Associations.Where(x => !string.IsNullOrWhiteSpace(x.Country)).Select(x => x.Country), 
                        item.IndigenousCulturesLocalities },
                    Collection = new object[] { item.CollectionNames, item.Discipline },
                    Date = new object[] { item.Associations.Where(x => !string.IsNullOrWhiteSpace(x.Date)).Select(x => x.Date), 
                        item.IndigenousCulturesDate, 
                        item.IndigenousCulturesDateCollected,
                        item.ArcheologyManufactureDate,
                        item.PhilatelyDateIssued,
                        item.TradeLiteraturePublicationDate },
                    CulturalGroup = item.IndigenousCulturesCulturalGroups,
                    Classification = item.Classifications,
                    Name = new object[] { item.Associations.Where(x => !string.IsNullOrWhiteSpace(x.Name)).Select(x => x.Name), 
                        item.IndigenousCulturesPhotographer, 
                        item.IndigenousCulturesAuthor, 
                        item.IndigenousCulturesIllustrator, 
                        item.IndigenousCulturesMaker, 
                        item.IndigenousCulturesCollector,
                        item.IndigenousCulturesLetterTo,
                        item.IndigenousCulturesLetterFrom,
                        item.Brands.Select(x => x.Name),
                        item.ArcheologyManufactureName,
                        item.TradeLiteraturePrimaryName,
                        item.ArtworkPublisher },
                    Technique = item.ArcheologyTechnique,
                    Denomination = new object[] { item.NumismaticsDenomination, item.PhilatelyDenomination },
                    Habitat = new object[] { },
                    Taxon = new object[] { item.ScientificName,
                        item.Taxonomy.Kingdom,
                        item.Taxonomy.Phylum,
                        item.Taxonomy.Subphylum,
                        item.Taxonomy.Superclass,
                        item.Taxonomy.Class,
                        item.Taxonomy.Subclass,
                        item.Taxonomy.Superorder,
                        item.Taxonomy.Order,
                        item.Taxonomy.Suborder,
                        item.Taxonomy.Infraorder,
                        item.Taxonomy.Superfamily,
                        item.Taxonomy.Family,
                        item.Taxonomy.Subfamily,
                        item.Taxonomy.Genus,
                        item.Taxonomy.Subgenus,
                        item.Taxonomy.TaxonName,
                        LoadDocument<Species>(item.RelatedSpeciesIds).Select(x => x.Taxonomy.TaxonName) },
                    TypeStatus = (string)null,
                    GeoType = new object[] { },
                    MuseumLocation = new object[] { item.MuseumLocation.Gallery, item.MuseumLocation.Venue },
                    Article = LoadDocument<Article>(item.RelatedArticleIds).Select(x => x.Title)
                });

            AddMap<Species>(speciesDocs =>
                from species in speciesDocs
                where species.IsHidden == false
                select new
                {
                    // Update fields
                    MediaIrns = new object[] { species.Media.Select(x => x.Irn), species.Authors.Select(x => x.ProfileImage.Irn) },
                    TaxonomyIrn = species.Taxonomy.Irn,

                    // Content fields
                    Id = species.Id,
                    DisplayTitle = species.DisplayTitle,
                    Content =
                        new object[]
                        {
                            (species.Taxonomy != null)
                            ? new object[]
                            {
                                species.Taxonomy.Kingdom, species.Taxonomy.Phylum, species.Taxonomy.Subphylum,
                                species.Taxonomy.Superclass, species.Taxonomy.Class, species.Taxonomy.Subclass,
                                species.Taxonomy.Superorder, species.Taxonomy.Order, species.Taxonomy.Suborder,
                                species.Taxonomy.Infraorder, species.Taxonomy.Superfamily, species.Taxonomy.Family,
                                species.Taxonomy.Subfamily, species.Taxonomy.CommonName, species.Taxonomy.OtherCommonNames
                            }
                            : null,
                            species.AnimalType, species.AnimalSubType
                        },
                    Summary = species.Summary,
                    ThumbnailUri = species.ThumbnailUri,

                    // Sort fields
                    Quality =
                        ((!string.IsNullOrWhiteSpace(species.GeneralDescription) || !string.IsNullOrWhiteSpace(species.Biology) || !string.IsNullOrWhiteSpace(species.Habitat) || !string.IsNullOrWhiteSpace(species.Endemicity) || !string.IsNullOrWhiteSpace(species.Diet)) ? 1 : 0) +
                        ((!string.IsNullOrWhiteSpace(species.BriefId) || !string.IsNullOrWhiteSpace(species.Hazards)) ? 1 : 0) +
                        (species.Media.Count * 2) +
                        ((species.RelatedItemIds.Any() || species.RelatedSpecimenIds.Any()) ? 1 : 0),

                    // Facet fields
                    Type = "Species",
                    Category = "Natural Sciences",
                    HasImages = (species.Media.Any()) ? "Yes" : (string) null,
                    OnDisplay = (string)null,
                    CollectionArea = new object[] { },
                    ItemType = (string) null,
                    SpeciesType = species.AnimalType,
                    SpeciesEndemicity = species.Endemicity,
                    SpecimenScientificGroup = (string) null,
                    ArticleType = new object[] { },
                    OnDisplayLocation = (string)null,

                    // Term fields
                    Keyword = new object[] { species.ConservationStatuses, species.AnimalSubType },
                    Locality = new object[] { species.NationalParks },
                    Collection = new object[] { },
                    Date = new object[] { },
                    CulturalGroup = new object[] { },
                    Classification = new object[] { },
                    Name = new object[] { },
                    Technique = (string)null,
                    Denomination = (string)null,
                    Habitat = new object[] { species.Habitats, species.WhereToLook },
                    Taxon = new object[] { species.Taxonomy.Kingdom,
                        species.Taxonomy.Phylum,
                        species.Taxonomy.Subphylum,
                        species.Taxonomy.Superclass,
                        species.Taxonomy.Class,
                        species.Taxonomy.Subclass,
                        species.Taxonomy.Superorder,
                        species.Taxonomy.Order,
                        species.Taxonomy.Suborder,
                        species.Taxonomy.Infraorder,
                        species.Taxonomy.Superfamily,
                        species.Taxonomy.Family,
                        species.Taxonomy.Subfamily,
                        species.Taxonomy.Genus,
                        species.Taxonomy.Subgenus,
                        species.Taxonomy.TaxonName },
                    TypeStatus = (string)null,
                    GeoType = new object[] { },
                    MuseumLocation = new object[] { },
                    Article = new object[] { }
                });

            AddMap<Specimen>(specimens =>
                from specimen in specimens
                where specimen.IsHidden == false
                select new
                {
                    // Update fields
                    MediaIrns = specimen.Media.Select(x => x.Irn),
                    TaxonomyIrn = specimen.Taxonomy.Irn,

                    // Content fields
                    Id = specimen.Id,
                    DisplayTitle = specimen.DisplayTitle,
                    Content = new object[]
                    {
                        specimen.RegistrationNumber, specimen.ScientificGroup, specimen.Category, specimen.CollectionNames, specimen.Discipline,
                        specimen.Sex, specimen.StageOrAge, specimen.Storages.Select(x => string.Format("{0} {1} {2} {3}", x.FixativeTreatment, x.Form, x.Medium, x.Nature)),
                        (specimen.Taxonomy != null)
                            ? new object[]
                            {
                                specimen.Taxonomy.Kingdom, specimen.Taxonomy.Phylum, specimen.Taxonomy.Subphylum,
                                specimen.Taxonomy.Superclass, specimen.Taxonomy.Class, specimen.Taxonomy.Subclass,
                                specimen.Taxonomy.Superorder, specimen.Taxonomy.Order, specimen.Taxonomy.Suborder,
                                specimen.Taxonomy.Infraorder, specimen.Taxonomy.Superfamily, specimen.Taxonomy.Family,
                                specimen.Taxonomy.Subfamily, specimen.Taxonomy.CommonName, specimen.Taxonomy.OtherCommonNames
                            }
                            : null,
                        specimen.ScientificName, specimen.TypeStatus, specimen.ExpeditionName, specimen.CollectionEventCode,
                        specimen.SamplingMethod, specimen.CollectedBy, specimen.SiteCode, specimen.Ocean, specimen.Continent,
                        specimen.Country, specimen.State, specimen.District, specimen.Town, specimen.NearestNamedPlace,
                        specimen.PreciseLocation, specimen.GeologyEra, specimen.GeologyPeriod, specimen.GeologyEpoch,
                        specimen.GeologyStage, specimen.GeologyGroup, specimen.GeologyFormation, specimen.GeologyMember,
                        specimen.GeologyRockType, specimen.PetrologyRockClass,  specimen.PetrologyRockGroup, specimen.PetrologyRockName,
                        specimen.PetrologyMineralsPresent, specimen.MineralogySpecies, specimen.MineralogyVariety, specimen.MineralogyGroup,
                        specimen.MineralogyClass, specimen.MineralogyAssociatedMatrix, specimen.MineralogyTypeOfType, specimen.MeteoritesName,
                        specimen.MeteoritesClass, specimen.MeteoritesGroup, specimen.MeteoritesType, specimen.MeteoritesMinerals, 
                        specimen.TektitesName, specimen.TektitesClassification, specimen.TektitesShape, specimen.TektitesLocalStrewnfield,
                        specimen.TektitesGlobalStrewnfield                        
                    },
                    Summary = specimen.Summary,
                    ThumbnailUri = specimen.ThumbnailUri,

                    // Sort fields
                    Quality =
                        ((specimen.DateVisitedFrom.HasValue || !string.IsNullOrWhiteSpace(specimen.CollectedBy) || !string.IsNullOrWhiteSpace(specimen.TypeStatus)) ? 1 : 0) +
                        ((!string.IsNullOrWhiteSpace(specimen.Latitude) || !string.IsNullOrWhiteSpace(specimen.Longitude)) ? 1 : 0) +
                        (specimen.Media.Count * 2) +
                        ((!string.IsNullOrWhiteSpace(specimen.ScientificName)) ? 1 : 0),

                    // Facet fields
                    Type = "Specimen",
                    Category = specimen.Category,
                    HasImages = (specimen.Media.Any()) ? "Yes" : (string) null,
                    OnDisplay = specimen.MuseumLocation != null ? "Yes" : (string)null,
                    CollectionArea = specimen.CollectionAreas,                    
                    ItemType = specimen.Type,
                    SpeciesType = (string) null,
                    SpeciesEndemicity = (string)null,
                    SpecimenScientificGroup = specimen.ScientificGroup,
                    ArticleType = new object[] { },
                    OnDisplayLocation = specimen.MuseumLocation.OnDisplayLocation,

                    // Term fields
                    Keyword = new object[] { specimen.Keywords, specimen.ExpeditionName },
                    Locality = new object[] { specimen.Associations.Where(x => !string.IsNullOrWhiteSpace(x.Locality)).Select(x => x.Locality), 
                        specimen.Associations.Where(x => !string.IsNullOrWhiteSpace(x.Region)).Select(x => x.Region), 
                        specimen.Associations.Where(x => !string.IsNullOrWhiteSpace(x.State)).Select(x => x.State), 
                        specimen.Associations.Where(x => !string.IsNullOrWhiteSpace(x.Country)).Select(x => x.Country),
                        specimen.Ocean,
                        specimen.Continent,
                        specimen.Country,
                        specimen.State,
                        specimen.District,
                        specimen.Town,
                        specimen.NearestNamedPlace,
                        specimen.TektitesLocalStrewnfield, 
                        specimen.TektitesGlobalStrewnfield },
                    Collection = new object[] { specimen.CollectionNames, specimen.Discipline },
                    Date = new object[] { specimen.Associations.Where(x => !string.IsNullOrWhiteSpace(x.Date)).Select(x => x.Date) },
                    CulturalGroup = new object[] { },
                    Classification = specimen.Classifications,
                    Name = new object[] { specimen.Associations.Where(x => !string.IsNullOrWhiteSpace(x.Name)).Select(x => x.Name) },
                    Technique = (string)null,
                    Denomination = new object[] { },
                    Habitat = new object[] { },
                    Taxon = new object[] { specimen.ScientificName,
                        specimen.Taxonomy.Kingdom,
                        specimen.Taxonomy.Phylum,
                        specimen.Taxonomy.Subphylum,
                        specimen.Taxonomy.Superclass,
                        specimen.Taxonomy.Class,
                        specimen.Taxonomy.Subclass,
                        specimen.Taxonomy.Superorder,
                        specimen.Taxonomy.Order,
                        specimen.Taxonomy.Suborder,
                        specimen.Taxonomy.Infraorder,
                        specimen.Taxonomy.Superfamily,
                        specimen.Taxonomy.Family,
                        specimen.Taxonomy.Subfamily,
                        specimen.Taxonomy.Genus,
                        specimen.Taxonomy.Subgenus,
                        specimen.Taxonomy.TaxonName,
                        LoadDocument<Species>(specimen.RelatedSpeciesIds).Select(x => x.Taxonomy.TaxonName) },
                    TypeStatus = specimen.TypeStatus,
                    GeoType = new object[] { specimen.PetrologyRockClass, 
                        specimen.PetrologyRockGroup, 
                        specimen.MineralogyVariety, 
                        specimen.MineralogyGroup, 
                        specimen.MineralogyClass,
                        specimen.MeteoritesClass,
                        specimen.MeteoritesGroup,
                        specimen.TektitesClassification },
                    MuseumLocation = new object[] { specimen.MuseumLocation.Gallery, specimen.MuseumLocation.Venue },
                    Article = LoadDocument<Article>(specimen.RelatedArticleIds).Select(x => x.Title)
                });
            
            Index(x => x.Id, FieldIndexing.No);
            Index(x => x.DisplayTitle, FieldIndexing.No);
            Index(x => x.Content, FieldIndexing.Analyzed);
            Index(x => x.Summary, FieldIndexing.No);
            Index(x => x.ThumbnailUri, FieldIndexing.No);

            Index(x => x.MediaIrns, FieldIndexing.NotAnalyzed);
            Index(x => x.TaxonomyIrn, FieldIndexing.NotAnalyzed);

            Store(x => x.Id, FieldStorage.Yes);
            Store(x => x.DisplayTitle, FieldStorage.Yes);
            Store(x => x.Summary, FieldStorage.Yes);
            Store(x => x.ThumbnailUri, FieldStorage.Yes);
            Store(x => x.Type, FieldStorage.Yes);

            Sort(x => x.Quality, SortOptions.Int);

            Analyzers.Add(x => x.Content, "Lucene.Net.Analysis.Standard.StandardAnalyzer");

            Suggestion(x => x.Content);

            TermVector(x => x.Content, FieldTermVector.Yes);
        }
    }

    public class CombinedIndexResult
    {
        /* Update fields */
        public long[] MediaIrns { get; set; }

        public long TaxonomyIrn { get; set; }

        /* Content/Order fields */
        public string Id { get; set; }

        public string DisplayTitle { get; set; }

        public object[] Content { get; set; }

        public string Summary { get; set; }

        public string ThumbnailUri { get; set; }

        public int Quality { get; set; }

        /* facet fields */

        public string Type { get; set; }

        public string Category { get; set; }

        public string HasImages { get; set; }

        public string OnDisplay { get; set; }

        public object[] CollectionArea { get; set; }

        public string ItemType { get; set; }

        public string SpeciesType { get; set; }

        public string SpeciesEndemicity { get; set; }

        public string SpecimenScientificGroup { get; set; }

        public object[] ArticleType { get; set; }

        public string OnDisplayLocation { get; set; }

        /* term fields */

        public object[] Keyword { get; set; }

        public object[] Locality { get; set; }

        public object[] Collection { get; set; }

        public object[] Date { get; set; }

        public object[] CulturalGroup { get; set; }

        public object[] Classification { get; set; }

        public object[] Name { get; set; }

        public string Technique { get; set; }

        public object[] Denomination { get; set; }

        public object[] Habitat { get; set; }

        public object[] Taxon { get; set; }

        public string TypeStatus { get; set; }

        public object[] GeoType { get; set; }

        public object[] MuseumLocation { get; set; }

        public object[] Article { get; set; }
    }
}