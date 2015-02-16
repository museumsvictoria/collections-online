using System;
using System.Linq;
using CollectionsOnline.Core.Models;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace CollectionsOnline.Core.Indexes
{
    public class Combined : AbstractMultiMapIndexCreationTask<CombinedResult>
    {
        public Combined()
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
                    Content = new object[] { article.Content, article.ContentSummary },
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
                    Category = "History & Technology",
                    HasImages = (article.Media.Any()) ? "Yes" : (string)null,
                    OnDisplay = (string)null,
                    Discipline = (string)null,
                    ItemType = (string)null,
                    SpeciesType = (string)null,
                    SpeciesEndemicity = new object[] { },
                    SpecimenScientificGroup = (string)null,
                    ArticleTypes = article.Types,
                    OnDisplayLocation = (string)null,

                    // Term fields
                    Keywords = article.Keywords,
                    Localities = new object[] { },
                    Collections = new object[] { },
                    Dates = new object[] { },
                    CulturalGroups = new object[] { },
                    Classifications = new object[] { },
                    Names = new object[] { },
                    Technique = (string)null,
                    Denominations = new object[] { },
                    Habitats = new object[] { },
                    Taxon = new object[] { },
                    TypeStatus = (string)null,
                    GeoTypes = new object[] { },
                    MuseumLocations = new object[] { },
                    Articles = new object[] { }
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
                    Content = new object[] { item.ObjectName, item.Discipline, item.RegistrationNumber },
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
                    Discipline = item.Discipline,
                    ItemType = item.Type,
                    SpeciesType = (string)null,
                    SpeciesEndemicity = new object[] { },
                    SpecimenScientificGroup = (string)null,
                    ArticleTypes = new object[] { },
                    OnDisplayLocation = item.MuseumLocation.OnDisplayLocation,
                    
                    // Term fields
                    Keywords = new object[] { item.Keywords,
                        item.AudioVisualRecordingDetails, 
                        item.ModelNames, 
                        item.ArcheologyActivity, 
                        item.ArcheologySpecificActivity, 
                        item.ArcheologyDecoration,
                        item.NumismaticsSeries,
                        item.TradeLiteraturePrimarySubject,
                        item.TradeLiteraturePublicationTypes,
                        item.TradeLiteraturePrimaryRole },
                    Localities = new object[] { item.Associations.Where(x => !string.IsNullOrWhiteSpace(x.Locality)).Select(x => x.Locality), 
                        item.Associations.Where(x => !string.IsNullOrWhiteSpace(x.Region)).Select(x => x.Region), 
                        item.Associations.Where(x => !string.IsNullOrWhiteSpace(x.State)).Select(x => x.State), 
                        item.Associations.Where(x => !string.IsNullOrWhiteSpace(x.Country)).Select(x => x.Country), 
                        item.IndigenousCulturesLocality, 
                        item.IndigenousCulturesRegion, 
                        item.IndigenousCulturesState, 
                        item.IndigenousCulturesCountry },
                    Collections = new object[] { item.CollectionNames, item.CollectionPlans },
                    Dates = new object[] { item.Associations.Where(x => !string.IsNullOrWhiteSpace(x.Date)).Select(x => x.Date), 
                        item.IndigenousCulturesDate, 
                        item.IndigenousCulturesDateCollected,
                        item.ArcheologyManufactureDate,
                        item.PhilatelyDateIssued,
                        item.TradeLiteraturePublicationDate },
                    CulturalGroups = item.IndigenousCulturesCulturalGroups,
                    Classifications = new object[] { item.PrimaryClassification, item.SecondaryClassification, item.TertiaryClassification },
                    Names = new object[] { item.Associations.Where(x => !string.IsNullOrWhiteSpace(x.Name)).Select(x => x.Name), 
                        item.IndigenousCulturesPhotographer, 
                        item.IndigenousCulturesAuthor, 
                        item.IndigenousCulturesIllustrator, 
                        item.IndigenousCulturesMaker, 
                        item.IndigenousCulturesCollector,
                        item.IndigenousCulturesLetterTo,
                        item.IndigenousCulturesLetterFrom,
                        item.BrandNames,
                        item.ArcheologyManufactureName,
                        item.TradeLiteraturePrimaryName,
                        item.ArtworkPublisher },
                    Technique = item.ArcheologyTechnique,
                    Denominations = new object[] { item.NumismaticsDenomination, item.PhilatelyDenomination },
                    Habitats = new object[] { },
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
                    GeoTypes = new object[] { },
                    MuseumLocations = new object[] { item.MuseumLocation.Gallery, item.MuseumLocation.Venue },
                    Articles = LoadDocument<Article>(item.RelatedArticleIds).Select(x => x.Title)
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
                    Content = new object[] { species.AnimalType, species.AnimalSubType, species.Taxonomy.CommonName, species.Taxonomy.Species, species.Taxonomy.Genus, species.Taxonomy.Family, species.Taxonomy.Order, species.Taxonomy.Class, species.Taxonomy.Phylum },
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
                    Discipline = (string) null,
                    ItemType = (string) null,
                    SpeciesType = species.AnimalType,
                    SpeciesEndemicity = species.Endemicity,
                    SpecimenScientificGroup = (string) null,
                    ArticleTypes = new object[] { },
                    OnDisplayLocation = (string)null,

                    // Term fields
                    Keywords = new object[] { species.ConservationStatuses, species.AnimalSubType },
                    Localities = new object[] { species.NationalParks },
                    Collections = new object[] { },
                    Dates = new object[] {},
                    CulturalGroups = new object[] {},
                    Classifications = new object[] { },
                    Names = new object[] { },
                    Technique = (string)null,
                    Denominations = (string)null,
                    Habitats = new object[] { species.Habitats, species.WhereToLook },
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
                    GeoTypes = new object[] { },
                    MuseumLocations = new object[] { },
                    Articles = new object[] { }
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
                    Content = new object[] { specimen.ScientificGroup, specimen.Type, specimen.RegistrationNumber, specimen.Discipline, specimen.Country },
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
                    Discipline = specimen.Discipline,                    
                    ItemType = specimen.Type,
                    SpeciesType = (string) null,
                    SpeciesEndemicity = new object[] { },
                    SpecimenScientificGroup = specimen.ScientificGroup,
                    ArticleTypes = new object[] { },
                    OnDisplayLocation = specimen.MuseumLocation.OnDisplayLocation,

                    // Term fields
                    Keywords = new object[] { specimen.Keywords, specimen.ExpeditionName },
                    Localities = new object[] { specimen.Associations.Where(x => !string.IsNullOrWhiteSpace(x.Locality)).Select(x => x.Locality), 
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
                    Collections = new object[] { specimen.CollectionNames, specimen.CollectionPlans },
                    Dates = new object[] { specimen.Associations.Where(x => !string.IsNullOrWhiteSpace(x.Date)).Select(x => x.Date) },
                    CulturalGroups = new object[] { },
                    Classifications = new object[] { specimen.PrimaryClassification, specimen.SecondaryClassification, specimen.TertiaryClassification },
                    Names = new object[] { specimen.Associations.Where(x => !string.IsNullOrWhiteSpace(x.Name)).Select(x => x.Name) },
                    Technique = (string)null,
                    Denominations = new object[] { },
                    Habitats = new object[] { },
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
                    GeoTypes = new object[] { specimen.PetrologyRockClass, 
                        specimen.PetrologyRockGroup, 
                        specimen.MineralogyVariety, 
                        specimen.MineralogyGroup, 
                        specimen.MineralogyClass,
                        specimen.MeteoritesClass,
                        specimen.MeteoritesGroup,
                        specimen.TektitesClassification },
                    MuseumLocations = new object[] { specimen.MuseumLocation.Gallery, specimen.MuseumLocation.Venue },
                    Articles = LoadDocument<Article>(specimen.RelatedArticleIds).Select(x => x.Title)
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

            TermVector(x => x.Type, FieldTermVector.Yes);
            TermVector(x => x.Category, FieldTermVector.Yes);
            TermVector(x => x.HasImages, FieldTermVector.Yes);
            TermVector(x => x.OnDisplay, FieldTermVector.Yes);
            TermVector(x => x.Discipline, FieldTermVector.Yes);
            TermVector(x => x.ItemType, FieldTermVector.Yes);
            TermVector(x => x.SpeciesType, FieldTermVector.Yes);
            TermVector(x => x.SpeciesEndemicity, FieldTermVector.Yes);
            TermVector(x => x.SpecimenScientificGroup, FieldTermVector.Yes);
            TermVector(x => x.ArticleTypes, FieldTermVector.Yes);
            TermVector(x => x.OnDisplayLocation, FieldTermVector.Yes);
        }
    }
}