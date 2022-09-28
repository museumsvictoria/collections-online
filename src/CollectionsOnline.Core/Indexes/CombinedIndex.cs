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
                    TaxonomyIrn = 0,
                    CollectionEventIrn = 0,
                    CollectionSiteIrn = 0,

                    // Content fields
                    article.Id,
                    article.DisplayTitle,
                    SubDisplayTitle = (string)null,
                    Content = new object[] { article.DisplayTitle, article.ContentText, article.ContentSummary, article.Keywords },
                    article.Summary,
                    article.ThumbnailUri,

                    // Sort fields
                    Quality =
                        (article.RelatedItemIds.Any() || article.RelatedSpecimenIds.Any() ? 1 : 0) +
                        (article.ChildArticleIds.Any() ? 1 : 0) +
                        (article.Media.Any(x => !string.Equals(AsDocument(x)["$type"].ToString(), "CollectionsOnline.Core.Models.ImageMedia, CollectionsOnline.Core", StringComparison.Ordinal)) ? 1 : 0) +
                        (article.Media.OfType<ImageMedia>().Any() ? Math.Log(article.Media.OfType<ImageMedia>().Average(x => Math.Max(x.Large.Height, x.Large.Width) / 100) + article.Media.Count, 2) : 0),
                    article.DateModified,

                    // Facet fields
                    RecordType = "Article",
                    Category = (string)null,
                    HasMedia = new object[]
                    {
                        article.Media.OfType<ImageMedia>().Any() ? "Images" : null,
                        article.Media.OfType<VideoMedia>().Any() ? "Videos" : null,
                        article.Media.OfType<AudioMedia>().Any() ? "Audio" : null,
                        article.Media.OfType<FileMedia>().Any() ? "Files" : null,
                    },
                    HasImages = article.Media.OfType<ImageMedia>().Any() ? "Yes" : "No",
                    OnDisplay = (string)null,
                    DisplayLocation = (string)null,
                    CollectingArea = new object[] { },
                    ItemType = (string)null,
                    SpeciesType = (string)null,                    
                    SpecimenScientificGroup = (string)null,
                    ArticleType = article.Types,
                    ImageLicence = article.Media.Where(x => string.Equals(AsDocument(x)["$type"].ToString(), "CollectionsOnline.Core.Models.ImageMedia, CollectionsOnline.Core", StringComparison.Ordinal)).Select(x => x.Licence.ShortName),

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
                    Article = new object[] { },
                    SpeciesEndemicity = (string)null,
                });

            AddMap<Item>(items => 
                from item in items
                where item.IsHidden == false
                select new
                {
                    // Update fields
                    TaxonomyIrn = item.Taxonomy.Irn,
                    CollectionEventIrn = 0,
                    CollectionSiteIrn = 0,

                    // Content fields
                    item.Id,
                    item.DisplayTitle,
                    SubDisplayTitle = item.RegistrationNumber,
                    Content = new object[] 
                    { 
                        item.ObjectName, item.Discipline, item.RegistrationNumber, item.RegistrationNumber.Replace(" ", ""),
                        item.ObjectSummary, item.PhysicalDescription, item.CollectionNames, item.Keywords, item.Significance,
                        item.Associations.Select(x => $"{x.Name} {x.Country} {x.Date} {x.Locality} {x.Region} {x.State} {x.StreetAddress}"),
                        item.IndigenousCulturesMedium, item.IndigenousCulturesLocalName, item.IndigenousCulturesCulturalGroups, item.IndigenousCulturesLocalities,
                        item.IndigenousCulturesDate, item.IndigenousCulturesLocalities, item.IndigenousCulturesDescription, item.IndigenousCulturesPhotographer,
                        item.IndigenousCulturesAuthor, item.IndigenousCulturesIllustrator, item.IndigenousCulturesMaker, item.IndigenousCulturesDate, item.IndigenousCulturesCollector,
                        item.IndigenousCulturesDateCollected, item.IndigenousCulturesIndividualsIdentified, item.IndigenousCulturesLetterTo, item.IndigenousCulturesLetterFrom,
                        item.IsdDescriptionOfContent, item.ArcheologyDescription, item.ArcheologyManufactureName, item.ArcheologyManufactureDate, item.TradeLiteraturePrimaryName
                    },
                    item.Summary,
                    item.ThumbnailUri,

                    // Sort fields
                    Quality = 
                        (!string.IsNullOrWhiteSpace(item.PhysicalDescription) || !string.IsNullOrWhiteSpace(item.ObjectSummary) || !string.IsNullOrWhiteSpace(item.Significance) || !string.IsNullOrWhiteSpace(item.IsdDescriptionOfContent) ? 1 : 0) + 
                        (item.Associations.Any() ? 1 : 0) +
                        (item.Media.Any(x => !string.Equals(AsDocument(x)["$type"].ToString(), "CollectionsOnline.Core.Models.ImageMedia, CollectionsOnline.Core", StringComparison.Ordinal)) ? 1 : 0) +
                        (item.Media.OfType<ImageMedia>().Any() ? Math.Log(item.Media.OfType<ImageMedia>().Average(x => Math.Max(x.Large.Height, x.Large.Width) / 100) + item.Media.Count, 2) : 0),
                    item.DateModified,

                    // Facet fields
                    RecordType = "Item",
                    item.Category,
                    HasMedia = new object[]
                    {
                        item.Media.OfType<ImageMedia>().Any() ? "Images" : null,
                        item.Media.OfType<VideoMedia>().Any() ? "Videos" : null,
                        item.Media.OfType<AudioMedia>().Any() ? "Audio" : null,
                        item.Media.OfType<FileMedia>().Any() ? "Files" : null,
                    },
                    HasImages = item.Media.OfType<ImageMedia>().Any() ? "Yes" : "No",
                    OnDisplay = item.MuseumLocation != null ? "Yes" : "No",
                    item.MuseumLocation.DisplayLocation,
                    CollectingArea = item.CollectingAreas,
                    ItemType = item.Type,
                    SpeciesType = (string)null,
                    SpecimenScientificGroup = (string)null,
                    ArticleType = new object[] { },
                    ImageLicence = item.Media.Where(x => string.Equals(AsDocument(x)["$type"].ToString(), "CollectionsOnline.Core.Models.ImageMedia, CollectionsOnline.Core", StringComparison.Ordinal)).Select(x => x.Licence.ShortName),
                    
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
                    Article = LoadDocument<Article>(item.RelatedArticleIds).Select(x => x.Title),
                    SpeciesEndemicity = (string)null,
                });

            AddMap<Species>(speciesDocs =>
                from species in speciesDocs
                where species.IsHidden == false
                select new
                {
                    // Update fields
                    TaxonomyIrn = species.Taxonomy.Irn,
                    CollectionEventIrn = 0,
                    CollectionSiteIrn = 0,

                    // Content fields
                    species.Id,
                    species.DisplayTitle,
                    SubDisplayTitle = (string)null,
                    Content = new object[]
                    {
                        species.Taxonomy != null
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
                    species.Summary,
                    species.ThumbnailUri,

                    // Sort fields
                    Quality =
                        (!string.IsNullOrWhiteSpace(species.GeneralDescription) || !string.IsNullOrWhiteSpace(species.Biology) || !string.IsNullOrWhiteSpace(species.Habitat) || !string.IsNullOrWhiteSpace(species.Endemicity) || !string.IsNullOrWhiteSpace(species.Diet) || !string.IsNullOrWhiteSpace(species.BriefId) || !string.IsNullOrWhiteSpace(species.Hazards) ? 1 : 0) +
                        (species.RelatedItemIds.Any() || species.RelatedSpecimenIds.Any() ? 1 : 0) +
                        (species.Media.Any(x => !string.Equals(AsDocument(x)["$type"].ToString(), "CollectionsOnline.Core.Models.ImageMedia, CollectionsOnline.Core", StringComparison.Ordinal)) ? 1 : 0) +
                        (species.Media.OfType<ImageMedia>().Any() ? Math.Log(species.Media.OfType<ImageMedia>().Average(x => Math.Max(x.Large.Height, x.Large.Width) / 100) + species.Media.Count, 2) : 0),
                    species.DateModified,

                    // Facet fields
                    RecordType = "Species",
                    Category = "Natural Sciences",
                    HasMedia = new object[]
                    {
                        species.Media.OfType<ImageMedia>().Any() ? "Images" : null,
                        species.Media.OfType<VideoMedia>().Any() ? "Videos" : null,
                        species.Media.OfType<AudioMedia>().Any() ? "Audio" : null,
                        species.Media.OfType<FileMedia>().Any() ? "Files" : null,
                    },
                    HasImages = species.Media.OfType<ImageMedia>().Any() ? "Yes" : "No",
                    OnDisplay = (string)null,
                    DisplayLocation = (string)null,
                    CollectingArea = new object[] { },
                    ItemType = (string) null,
                    SpeciesType = species.AnimalType,                    
                    SpecimenScientificGroup = (string) null,
                    ArticleType = new object[] { },
                    ImageLicence = species.Media.Where(x => string.Equals(AsDocument(x)["$type"].ToString(), "CollectionsOnline.Core.Models.ImageMedia, CollectionsOnline.Core", StringComparison.Ordinal)).Select(x => x.Licence.ShortName),

                    // Term fields
                    Keyword = new object[] { species.ConservationStatuses, species.AnimalSubType },
                    Locality = new object[] { },
                    Collection = new object[] { },
                    Date = new object[] { },
                    CulturalGroup = new object[] { },
                    Classification = new object[] { },
                    Name = new object[] { species.Authors.Select(x => x.FullName) },
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
                    Article = new object[] { },
                    SpeciesEndemicity = species.Endemicity,
                });

            AddMap<Specimen>(specimens =>
                from specimen in specimens
                where specimen.IsHidden == false
                select new
                {
                    // Update fields
                    TaxonomyIrn = specimen.Taxonomy.Irn,
                    CollectionEventIrn = specimen.CollectionEvent.Irn,
                    CollectionSiteIrn = specimen.CollectionSite.Irn,

                    // Content fields
                    specimen.Id,
                    specimen.DisplayTitle,
                    SubDisplayTitle = specimen.RegistrationNumber,
                    Content = new object[]
                    {
                        specimen.ObjectName, specimen.Discipline, specimen.RegistrationNumber, specimen.RegistrationNumber.Replace(" ", ""), specimen.ObjectSummary,
                        specimen.ScientificGroup, specimen.Category, specimen.CollectionNames, specimen.Discipline,
                        specimen.Sex, specimen.StageOrAge, specimen.Storages.Select(x => string.Format("{0} {1} {2} {3}", x.FixativeTreatment, x.Form, x.Medium, x.Nature)),
                        specimen.Taxonomy != null
                            ? new object[]
                            {
                                specimen.Taxonomy.TaxonName, specimen.Taxonomy.Kingdom, specimen.Taxonomy.Phylum, specimen.Taxonomy.Subphylum,
                                specimen.Taxonomy.Superclass, specimen.Taxonomy.Class, specimen.Taxonomy.Subclass,
                                specimen.Taxonomy.Superorder, specimen.Taxonomy.Order, specimen.Taxonomy.Suborder,
                                specimen.Taxonomy.Infraorder, specimen.Taxonomy.Superfamily, specimen.Taxonomy.Family,
                                specimen.Taxonomy.Subfamily, specimen.Taxonomy.CommonName, specimen.Taxonomy.OtherCommonNames
                            }
                            : null,
                        specimen.TypeStatus, 
                        specimen.CollectionEvent != null
                            ? new object[]
                            {
                                specimen.CollectionEvent.ExpeditionName, specimen.CollectionEvent.CollectionEventCode,
                                specimen.CollectionEvent.SamplingMethod, specimen.CollectionEvent.CollectedBy
                            }
                            : null,
                        specimen.CollectionSite != null
                            ? new object[]
                            {
                                specimen.CollectionSite.SiteCode, specimen.CollectionSite.Ocean, specimen.CollectionSite.Continent,
                                specimen.CollectionSite.Country, specimen.CollectionSite.State, specimen.CollectionSite.District, 
                                specimen.CollectionSite.Town, specimen.CollectionSite.NearestNamedPlace, specimen.CollectionSite.PreciseLocation, 
                                specimen.CollectionSite.GeologyEra, specimen.CollectionSite.GeologyPeriod, specimen.CollectionSite.GeologyEpoch,
                                specimen.CollectionSite.GeologyStage, specimen.CollectionSite.GeologyGroup, specimen.CollectionSite.GeologyFormation, 
                                specimen.CollectionSite.GeologyMember, specimen.CollectionSite.GeologyRockType,
                            }
                            : null,
                        specimen.PetrologyRockClass,  specimen.PetrologyRockGroup, specimen.PetrologyRockName,
                        specimen.PetrologyMineralsPresent, specimen.MineralogySpecies, specimen.MineralogyVariety, specimen.MineralogyGroup,
                        specimen.MineralogyClass, specimen.MineralogyAssociatedMatrix, specimen.MineralogyTypeOfType, specimen.MeteoritesName,
                        specimen.MeteoritesClass, specimen.MeteoritesGroup, specimen.MeteoritesType, specimen.MeteoritesMinerals, 
                        specimen.TektitesName, specimen.TektitesClassification, specimen.TektitesShape, specimen.TektitesLocalStrewnfield,
                        specimen.TektitesGlobalStrewnfield                        
                    },
                    specimen.Summary,
                    specimen.ThumbnailUri,

                    // Sort fields
                    Quality =
                        (specimen.CollectionEvent != null || !string.IsNullOrWhiteSpace(specimen.TypeStatus) || specimen.Taxonomy != null || specimen.CollectionSite != null ? 1 : 0) +
                        (!string.IsNullOrWhiteSpace(specimen.ObjectSummary) ? 1 : 0) +
                        (specimen.Media.Any(x => !string.Equals(AsDocument(x)["$type"].ToString(), "CollectionsOnline.Core.Models.ImageMedia, CollectionsOnline.Core", StringComparison.Ordinal)) ? 1 : 0) +
                        (specimen.Media.OfType<ImageMedia>().Any() ? Math.Log(specimen.Media.OfType<ImageMedia>().Average(x => Math.Max(x.Large.Height, x.Large.Width) / 100) + specimen.Media.Count, 2) : 0),
                    specimen.DateModified,

                    // Facet fields
                    RecordType = "Specimen",
                    specimen.Category,
                    HasMedia = new object[]
                    {
                        specimen.Media.OfType<ImageMedia>().Any() ? "Images" : null,
                        specimen.Media.OfType<VideoMedia>().Any() ? "Videos" : null,
                        specimen.Media.OfType<AudioMedia>().Any() ? "Audio" : null,
                        specimen.Media.OfType<FileMedia>().Any() ? "Files" : null,
                    },
                    HasImages = specimen.Media.OfType<ImageMedia>().Any() ? "Yes" : "No",
                    OnDisplay = specimen.MuseumLocation != null ? "Yes" : "No",
                    specimen.MuseumLocation.DisplayLocation,
                    CollectingArea = specimen.CollectingAreas,
                    ItemType = specimen.Type,
                    SpeciesType = (string) null,                    
                    SpecimenScientificGroup = specimen.ScientificGroup,
                    ArticleType = new object[] { },
                    ImageLicence = specimen.Media.Where(x => string.Equals(AsDocument(x)["$type"].ToString(), "CollectionsOnline.Core.Models.ImageMedia, CollectionsOnline.Core", StringComparison.Ordinal)).Select(x => x.Licence.ShortName),

                    // Term fields
                    Keyword = new object[] { specimen.Keywords, specimen.CollectionEvent != null ? specimen.CollectionEvent.ExpeditionName : null },
                    Locality = new object[] { specimen.Associations.Where(x => !string.IsNullOrWhiteSpace(x.Locality)).Select(x => x.Locality), 
                        specimen.Associations.Where(x => !string.IsNullOrWhiteSpace(x.Region)).Select(x => x.Region), 
                        specimen.Associations.Where(x => !string.IsNullOrWhiteSpace(x.State)).Select(x => x.State), 
                        specimen.Associations.Where(x => !string.IsNullOrWhiteSpace(x.Country)).Select(x => x.Country),
                        specimen.CollectionSite != null
                            ? new object[]
                            {
                                specimen.CollectionSite.Ocean, specimen.CollectionSite.Continent, specimen.CollectionSite.Country,
                                specimen.CollectionSite.State, specimen.CollectionSite.District, specimen.CollectionSite.Town, 
                                specimen.CollectionSite.NearestNamedPlace
                            }
                            : null,
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
                    specimen.TypeStatus,
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
                    Article = LoadDocument<Article>(specimen.RelatedArticleIds).Select(x => x.Title),
                    SpeciesEndemicity = (string)null,
                });
            
            Index(x => x.Id, FieldIndexing.No);
            Index(x => x.DisplayTitle, FieldIndexing.NotAnalyzed);
            Index(x => x.SubDisplayTitle, FieldIndexing.No);
            Index(x => x.Content, FieldIndexing.Analyzed);
            Index(x => x.Summary, FieldIndexing.No);
            Index(x => x.ThumbnailUri, FieldIndexing.No);

            Index(x => x.TaxonomyIrn, FieldIndexing.NotAnalyzed);

            Store(x => x.Id, FieldStorage.Yes);
            Store(x => x.DisplayTitle, FieldStorage.Yes);
            Store(x => x.SubDisplayTitle, FieldStorage.Yes);
            Store(x => x.Summary, FieldStorage.Yes);
            Store(x => x.ThumbnailUri, FieldStorage.Yes);
            Store(x => x.RecordType, FieldStorage.Yes);
            Store(x => x.Quality, FieldStorage.Yes);

            Sort(x => x.Quality, SortOptions.Double);

            Analyzers.Add(x => x.Content, "Lucene.Net.Analysis.Standard.StandardAnalyzer");

            TermVector(x => x.Content, FieldTermVector.Yes);
        }
    }

    public class CombinedIndexResult
    {
        /* Update fields */
        public long TaxonomyIrn { get; set; }

        public long CollectionEventIrn { get; set; }

        public long CollectionSiteIrn { get; set; }

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

        public string HasMedia { get; set; }
        
        public string HasImages { get; set; }

        public string OnDisplay { get; set; }

        public string DisplayLocation { get; set; }

        public object[] CollectingArea { get; set; }

        public string ItemType { get; set; }

        public string SpeciesType { get; set; }        

        public string SpecimenScientificGroup { get; set; }

        public object[] ArticleType { get; set; }

        public object[] ImageLicence { get; set; }        

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

        public string SpeciesEndemicity { get; set; }
    }
}