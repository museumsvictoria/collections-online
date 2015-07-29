using System;
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
                    SubDisplayTitle = (string)null,
                    Content = new object[] { article.DisplayTitle, article.ContentText, article.ContentSummary, article.Keywords },
                    Summary = article.Summary,
                    ThumbnailUri = article.ThumbnailUri,

                    // Sort fields
                    Quality =
                        ((article.RelatedItemIds.Any() || article.RelatedSpecimenIds.Any()) ? 1 : 0) +
                        ((article.Keywords.Any()) ? 1 : 0) +
                        Math.Log(article.Media.Count + 2, 2) +
                        ((article.ChildArticleIds.Any()) ? 1 : 0),
                    DateModified = article.DateModified.Date,

                    // Facet fields
                    RecordType = "Article",
                    Category = (string)null,
                    HasImages = (article.Media.OfType<ImageMedia>().Any()) ? "Yes" : "No",
                    OnDisplay = (string)null,
                    DisplayLocation = (string)null,
                    CollectingArea = new object[] { },
                    ItemType = (string)null,
                    SpeciesType = (string)null,
                    SpeciesEndemicity = (string)null,
                    SpecimenScientificGroup = (string)null,
                    ArticleType = article.Types,

                    // Term fields
                    Keyword = article.Keywords,
                    Locality = article.Localities,
                    Collection = new object[] { },
                    Date = new object[] { },
                    CulturalGroup = new object[] { },
                    Classification = new object[] { },
                    Name = new object[] { article.Authors.Select(x => x.FullName), article.Contributors.Select(x => x.FullName) },
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
                    SubDisplayTitle = item.RegistrationNumber,
                    Content = new object[] 
                    { 
                        item.ObjectName, item.Discipline, item.RegistrationNumber, item.RegistrationNumber.Replace(" ", ""),
                        item.ObjectSummary, item.PhysicalDescription, item.CollectionNames, item.Keywords, item.Significance,
                        item.Associations.Select(x => string.Format("{0} {1} {2} {3} {4} {5} {6}", x.Name, x.Country, x.Date, x.Locality, x.Region, x.State, x.StreetAddress)),
                        item.IndigenousCulturesMedium, item.IndigenousCulturesLocalName, item.IndigenousCulturesCulturalGroups, item.IndigenousCulturesLocalities,
                        item.IndigenousCulturesDate, item.IndigenousCulturesLocalities, item.IndigenousCulturesDescription, item.IndigenousCulturesPhotographer,
                        item.IndigenousCulturesAuthor, item.IndigenousCulturesIllustrator, item.IndigenousCulturesMaker, item.IndigenousCulturesDate, item.IndigenousCulturesCollector,
                        item.IndigenousCulturesDateCollected, item.IndigenousCulturesIndividualsIdentified, item.IndigenousCulturesLetterTo, item.IndigenousCulturesLetterFrom,
                        item.IsdDescriptionOfContent, item.ArcheologyDescription, item.ArcheologyManufactureName, item.ArcheologyManufactureDate, item.TradeLiteraturePrimaryName
                    },
                    Summary = item.Summary,
                    ThumbnailUri = item.ThumbnailUri,

                    // Sort fields
                    Quality = 
                        ((!string.IsNullOrWhiteSpace(item.PhysicalDescription) || !string.IsNullOrWhiteSpace(item.ObjectSummary) || !string.IsNullOrWhiteSpace(item.Significance) || !string.IsNullOrWhiteSpace(item.IsdDescriptionOfContent)) ? 1 : 0) + 
                        Math.Log(item.Media.Count + 2, 2) +
                        ((item.Associations.Any()) ? 1 : 0),
                    DateModified = item.DateModified.Date,

                    // Facet fields
                    RecordType = "Item",
                    Category = item.Category,
                    HasImages = (item.Media.OfType<ImageMedia>().Any()) ? "Yes" : "No",
                    OnDisplay = (item.MuseumLocation != null) ? "Yes" : "No",
                    DisplayLocation = item.MuseumLocation.DisplayLocation,
                    CollectingArea = item.CollectingAreas,
                    ItemType = item.Type,
                    SpeciesType = (string)null,
                    SpeciesEndemicity = (string)null,
                    SpecimenScientificGroup = (string)null,
                    ArticleType = new object[] { },
                    
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
                    Taxon = new object[] { item.Taxonomy.Kingdom,
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
                    SubDisplayTitle = (string)null,
                    Content = new object[]
                    {
                        (species.Taxonomy != null)
                        ? new object[]
                        {
                            species.Taxonomy.TaxonName, species.Taxonomy.Kingdom, species.Taxonomy.Phylum, species.Taxonomy.Subphylum,
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
                        Math.Log(species.Media.Count + 2, 2) +
                        ((species.RelatedItemIds.Any() || species.RelatedSpecimenIds.Any()) ? 1 : 0),
                    DateModified = species.DateModified.Date,

                    // Facet fields
                    RecordType = "Species",
                    Category = "Natural Sciences",
                    HasImages = (species.Media.OfType<ImageMedia>().Any()) ? "Yes" : "No",
                    OnDisplay = (string)null,
                    DisplayLocation = (string)null,
                    CollectingArea = new object[] { },
                    ItemType = (string) null,
                    SpeciesType = species.AnimalType,
                    SpeciesEndemicity = species.Endemicity,
                    SpecimenScientificGroup = (string) null,
                    ArticleType = new object[] { },

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
                    SubDisplayTitle = specimen.RegistrationNumber,
                    Content = new object[]
                    {
                        specimen.ObjectName, specimen.Discipline, specimen.RegistrationNumber, specimen.RegistrationNumber.Replace(" ", ""), specimen.ObjectSummary,
                        specimen.ScientificGroup, specimen.Category, specimen.CollectionNames, specimen.Discipline,
                        specimen.Sex, specimen.StageOrAge, specimen.Storages.Select(x => string.Format("{0} {1} {2} {3}", x.FixativeTreatment, x.Form, x.Medium, x.Nature)),
                        (specimen.Taxonomy != null)
                            ? new object[]
                            {
                                specimen.Taxonomy.TaxonName, specimen.Taxonomy.Kingdom, specimen.Taxonomy.Phylum, specimen.Taxonomy.Subphylum,
                                specimen.Taxonomy.Superclass, specimen.Taxonomy.Class, specimen.Taxonomy.Subclass,
                                specimen.Taxonomy.Superorder, specimen.Taxonomy.Order, specimen.Taxonomy.Suborder,
                                specimen.Taxonomy.Infraorder, specimen.Taxonomy.Superfamily, specimen.Taxonomy.Family,
                                specimen.Taxonomy.Subfamily, specimen.Taxonomy.CommonName, specimen.Taxonomy.OtherCommonNames
                            }
                            : null,
                        specimen.TypeStatus, specimen.ExpeditionName, specimen.CollectionEventCode,
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
                        ((specimen.Latitudes.Any() || specimen.Longitudes.Any()) ? 1 : 0) +
                        Math.Log(specimen.Media.Count + 2, 2) +
                        ((specimen.Taxonomy != null) ? 1 : 0),
                    DateModified = specimen.DateModified.Date,

                    // Facet fields
                    RecordType = "Specimen",
                    Category = specimen.Category,
                    HasImages = (specimen.Media.OfType<ImageMedia>().Any()) ? "Yes" : "No",
                    OnDisplay = (specimen.MuseumLocation != null) ? "Yes" : "No",
                    DisplayLocation = specimen.MuseumLocation.DisplayLocation,
                    CollectingArea = specimen.CollectingAreas,
                    ItemType = specimen.Type,
                    SpeciesType = (string) null,
                    SpeciesEndemicity = (string)null,
                    SpecimenScientificGroup = specimen.ScientificGroup,
                    ArticleType = new object[] { },

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
                    Taxon = new object[] { specimen.Taxonomy.Kingdom,
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
                        specimen.TektitesClassification,
                        specimen.MineralogySpecies },
                    MuseumLocation = new object[] { specimen.MuseumLocation.Gallery, specimen.MuseumLocation.Venue },
                    Article = LoadDocument<Article>(specimen.RelatedArticleIds).Select(x => x.Title)
                });
            
            Index(x => x.Id, FieldIndexing.No);
            Index(x => x.DisplayTitle, FieldIndexing.NotAnalyzed);
            Index(x => x.SubDisplayTitle, FieldIndexing.No);
            Index(x => x.Content, FieldIndexing.Analyzed);
            Index(x => x.Summary, FieldIndexing.No);
            Index(x => x.ThumbnailUri, FieldIndexing.No);

            Index(x => x.MediaIrns, FieldIndexing.NotAnalyzed);
            Index(x => x.TaxonomyIrn, FieldIndexing.NotAnalyzed);

            Store(x => x.Id, FieldStorage.Yes);
            Store(x => x.DisplayTitle, FieldStorage.Yes);
            Store(x => x.SubDisplayTitle, FieldStorage.Yes);
            Store(x => x.Summary, FieldStorage.Yes);
            Store(x => x.ThumbnailUri, FieldStorage.Yes);
            Store(x => x.RecordType, FieldStorage.Yes);
            Store(x => x.Quality, FieldStorage.Yes);

            Sort(x => x.Quality, SortOptions.Double);
            Sort(x => x.DateModified, SortOptions.String);

            Analyzers.Add(x => x.Content, "Lucene.Net.Analysis.Standard.StandardAnalyzer");

            //Suggestion(x => x.Content);

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

        public string SubDisplayTitle { get; set; }

        public object[] Content { get; set; }

        public string Summary { get; set; }

        public string ThumbnailUri { get; set; }

        public double Quality { get; set; }

        public DateTime DateModified { get; set; }

        /* facet fields */

        public string RecordType { get; set; }

        public string Category { get; set; }

        public string HasImages { get; set; }

        public string OnDisplay { get; set; }

        public string DisplayLocation { get; set; }

        public object[] CollectingArea { get; set; }

        public string ItemType { get; set; }

        public string SpeciesType { get; set; }

        public string SpeciesEndemicity { get; set; }

        public string SpecimenScientificGroup { get; set; }

        public object[] ArticleType { get; set; }

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