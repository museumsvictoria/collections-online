﻿using System;
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
                    Name = item.ObjectName,
                    Content = new object[] { item.ObjectName, item.Discipline, item.RegistrationNumber },
                    Summary = item.Summary,
                    ThumbnailUri = item.ThumbnailUri,

                    // Sort fields
                    Quality = 
                        ((!string.IsNullOrWhiteSpace(item.Description) || !string.IsNullOrWhiteSpace(item.ObjectSummary) || !string.IsNullOrWhiteSpace(item.Significance) || !string.IsNullOrWhiteSpace(item.IsdDescriptionOfContent)) ? 1 : 0) + 
                        (item.Media.Count * 2) +
                        ((item.Associations.Any()) ? 1 : 0),

                    // Facet fields
                    Type = "Item",
                    Category = item.Category,
                    HasImages = (item.Media.Any()) ? "Yes" : (string)null,
                    Discipline = item.Discipline,                    
                    ItemType = item.Type,
                    SpeciesType = (string)null,
                    SpeciesSubType = (string)null,
                    SpeciesEndemicity = new object[] { },
                    SpecimenScientificGroup = (string)null,
                    ArticleTypes = new object[] { },
                    
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
                    Phylum = (string)null,
                    Class = (string)null,
                    Order = (string)null,
                    Family = (string)null,
                    TypeStatus = (string)null,
                    GeoTypes = new object[] { },
                    MuseumLocations = new object[] { item.MuseumLocation.Gallery, item.MuseumLocation.Venue },
                    Articles = item.RelatedArticleIds,
                    Species = new object[] { }
                });

            AddMap<Species>(speciesDocs =>
                from species in speciesDocs
                where species.IsHidden == false
                select new
                {
                    // Update fields
                    MediaIrns = new object[] { species.Media.Select(x => x.Irn), species.Authors.Select(x => x.Media.Irn) },
                    TaxonomyIrn = species.Taxonomy.Irn,

                    // Content fields
                    Id = species.Id,
                    Name = species.Taxonomy.CommonName ?? species.Taxonomy.Species ?? species.Taxonomy.Genus,
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
                    Discipline = (string) null,                    
                    ItemType = (string) null,
                    SpeciesType = species.AnimalType,
                    SpeciesSubType = species.AnimalSubType,
                    SpeciesEndemicity = species.Endemicity,
                    SpecimenScientificGroup = (string) null,
                    ArticleTypes = new object[] { },

                    // Term fields
                    Keywords = new object[] { species.ConservationStatuses },
                    Localities = new object[] { species.NationalParks },
                    Collections = new object[] { },
                    Dates = new object[] {},
                    CulturalGroups = new object[] {},
                    Classifications = new object[] { },
                    Names = new object[] { },
                    Technique = (string)null,
                    Denominations = (string)null,
                    Habitats = new object[] { species.Habitats, species.WhereToLook },
                    Phylum = species.Taxonomy.Phylum,
                    Class = species.Taxonomy.Class,
                    Order = species.Taxonomy.Order,
                    Family = species.Taxonomy.Family,
                    TypeStatus = (string)null,
                    GeoTypes = new object[] { },
                    MuseumLocations = new object[] { },
                    Articles = new object[] { },
                    Species = new object[] { }
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
                    Name = specimen.ObjectName ?? specimen.ScientificName,
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
                    Category = "Natural Sciences",
                    HasImages = (specimen.Media.Any()) ? "Yes" : (string) null,
                    Discipline = specimen.Discipline,                    
                    ItemType = (string) null,
                    SpeciesType = (string) null,
                    SpeciesSubType = (string) null,
                    SpeciesEndemicity = new object[] { },
                    SpecimenScientificGroup = specimen.ScientificGroup,
                    ArticleTypes = new object[] { },

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
                    Phylum = specimen.Taxonomy.Phylum,
                    Class = specimen.Taxonomy.Class,
                    Order = specimen.Taxonomy.Order,
                    Family = specimen.Taxonomy.Family,
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
                    Articles = specimen.RelatedArticleIds,
                    Species = specimen.RelatedSpeciesIds,
                });

            AddMap<Article>(articles =>
                from article in articles
                where article.IsHidden == false
                select new
                {
                    // Update fields
                    MediaIrns = new object[] { article.Media.Select(x => x.Irn), article.Authors.Select(x => x.Media.Irn) },
                    TaxonomyIrn = 0,

                    // Content fields
                    Id = article.Id,
                    Name = article.Title,
                    Content = new object[] {article.Content, article.ContentSummary},
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
                    HasImages = (article.Media.Any()) ? "Yes" : (string) null,
                    Discipline = (string) null,                    
                    ItemType = (string) null,
                    SpeciesType = (string) null,
                    SpeciesSubType = (string) null,
                    SpeciesEndemicity = new object[] { },
                    SpecimenScientificGroup = (string) null,
                    ArticleTypes = article.Types,

                    // Term fields
                    Keywords = article.Keywords,
                    Localities = new object[] { },
                    Collections = new object[] { },
                    Dates = new object[] { },
                    CulturalGroups = new object[] { },
                    Classifications = new object[] { },
                    Names = new object[] { },
                    Technique = (string) null,
                    Denominations = new object[] { },
                    Habitats = new object[] { },
                    Phylum = (string)null,
                    Class = (string)null,
                    Order = (string)null,
                    Family = (string)null,
                    TypeStatus = (string)null,
                    GeoTypes = new object[] { },
                    MuseumLocations = new object[] { },
                    Articles = new object[] { },
                    Species = new object[] { }
                });
            
            Index(x => x.Id, FieldIndexing.No);
            Index(x => x.Name, FieldIndexing.No);
            Index(x => x.Content, FieldIndexing.Analyzed);
            Index(x => x.Summary, FieldIndexing.No);
            Index(x => x.ThumbnailUri, FieldIndexing.No);

            Index(x => x.MediaIrns, FieldIndexing.NotAnalyzed);
            Index(x => x.TaxonomyIrn, FieldIndexing.NotAnalyzed);
            Index(x => x.Keywords, FieldIndexing.NotAnalyzed);
            Index(x => x.Localities, FieldIndexing.NotAnalyzed);
            Index(x => x.Collections, FieldIndexing.NotAnalyzed);
            Index(x => x.Dates, FieldIndexing.NotAnalyzed);
            Index(x => x.CulturalGroups, FieldIndexing.NotAnalyzed);
            Index(x => x.Classifications, FieldIndexing.NotAnalyzed);
            Index(x => x.Names, FieldIndexing.NotAnalyzed);
            Index(x => x.Technique, FieldIndexing.NotAnalyzed);
            Index(x => x.Denominations, FieldIndexing.NotAnalyzed);
            Index(x => x.Habitats, FieldIndexing.NotAnalyzed);
            Index(x => x.Phylum, FieldIndexing.NotAnalyzed);
            Index(x => x.Class, FieldIndexing.NotAnalyzed);
            Index(x => x.Order, FieldIndexing.NotAnalyzed);
            Index(x => x.Family, FieldIndexing.NotAnalyzed);
            Index(x => x.TypeStatus, FieldIndexing.NotAnalyzed);
            Index(x => x.GeoTypes, FieldIndexing.NotAnalyzed);
            Index(x => x.MuseumLocations, FieldIndexing.NotAnalyzed);
            Index(x => x.Articles, FieldIndexing.NotAnalyzed);
            Index(x => x.Species, FieldIndexing.NotAnalyzed);

            Store(x => x.Id, FieldStorage.Yes);
            Store(x => x.Name, FieldStorage.Yes);
            Store(x => x.Summary, FieldStorage.Yes);
            Store(x => x.ThumbnailUri, FieldStorage.Yes);
            Store(x => x.Type, FieldStorage.Yes);
            
            Sort(x => x.Quality, SortOptions.Int);

            Analyzers.Add(x => x.Content, "SimpleAnalyzer");

            Suggestion(x => x.Content);

            TermVector(x => x.Type, FieldTermVector.Yes);
            TermVector(x => x.Category, FieldTermVector.Yes);
            TermVector(x => x.HasImages, FieldTermVector.Yes);
            TermVector(x => x.Discipline, FieldTermVector.Yes);
            TermVector(x => x.ItemType, FieldTermVector.Yes);
            TermVector(x => x.SpeciesType, FieldTermVector.Yes);
            TermVector(x => x.SpeciesSubType, FieldTermVector.Yes);
            TermVector(x => x.SpeciesEndemicity, FieldTermVector.Yes);
            TermVector(x => x.SpecimenScientificGroup, FieldTermVector.Yes);
            TermVector(x => x.ArticleTypes, FieldTermVector.Yes);
        }
    }
}