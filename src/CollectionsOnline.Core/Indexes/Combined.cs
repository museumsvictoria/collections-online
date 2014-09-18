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
                    SpeciesHabitats = new object[] { },
                    SpeciesDepths = new object[] { },
                    SpeciesWaterColumnLocations = new object[] { },
                    Phylum = (string)null,
                    Class = (string)null,
                    SpecimenScientificGroup = (string)null,
                    SpecimenDiscipline = (string)null,
                    ArticleTypes = new object[] { },
                    Dates = item.AssociatedDates.ToArray(),

                    // Term fields
                    Tags = item.Tags,
                    Country = item.Associations.Where(x => !string.IsNullOrWhiteSpace(x.Country)).Select(x => x.Country).ToArray(),
                    CollectionNames = item.CollectionNames,
                    CollectionPlans = item.CollectionPlans,
                    PrimaryClassification = item.PrimaryClassification,
                    SecondaryClassification = item.SecondaryClassification,
                    TertiaryClassification = item.TertiaryClassification,
                    AssociationNames = item.Associations.Where(x => !string.IsNullOrWhiteSpace(x.Name)).Select(x => x.Name).ToArray(),
                    ItemTradeLiteraturePrimarySubject = item.TradeLiteraturePrimarySubject,
                    ItemTradeLiteraturePublicationDate = item.TradeLiteraturePublicationDate,
                    ItemTradeLiteraturePrimaryRole = item.TradeLiteraturePrimaryRole,
                    ItemTradeLiteraturePrimaryName = item.TradeLiteraturePrimaryName,
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
                        ((species.RelatedItemSpecimenIds.Any()) ? 1 : 0),

                    // Facet fields
                    Type = "Species",
                    Category = "Natural Sciences",
                    HasImages = (species.Media.Any()) ? "Yes" : (string) null,
                    Discipline = (string) null,                    
                    ItemType = (string) null,
                    SpeciesType = species.AnimalType,
                    SpeciesSubType = species.AnimalSubType,
                    SpeciesHabitats = species.Habitats,
                    SpeciesDepths = species.Depths,
                    SpeciesWaterColumnLocations = species.WaterColumnLocations,
                    Phylum = species.Taxonomy.Phylum,
                    Class = species.Taxonomy.Class,
                    SpecimenScientificGroup = (string) null,
                    SpecimenDiscipline = (string) null,
                    ArticleTypes = new object[] {},
                    Dates = new object[] { },

                    // Term fields
                    Tags = new object[] {},
                    Country = new object[] {},
                    CollectionNames = new object[] {},
                    CollectionPlans = new object[] { },
                    PrimaryClassification = (string) null,
                    SecondaryClassification = (string) null,
                    TertiaryClassification = (string) null,
                    AssociationNames = new object[] {},
                    ItemTradeLiteraturePrimarySubject = (string) null,
                    ItemTradeLiteraturePublicationDate = (string) null,
                    ItemTradeLiteraturePrimaryRole = (string) null,
                    ItemTradeLiteraturePrimaryName = (string) null,
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
                    SpeciesHabitats = new object[] {},
                    SpeciesDepths = new object[] {},
                    SpeciesWaterColumnLocations = new object[] {},
                    Phylum = specimen.Taxonomy.Phylum,
                    Class = specimen.Taxonomy.Class,
                    SpecimenScientificGroup = specimen.ScientificGroup,
                    SpecimenDiscipline = specimen.Discipline,
                    ArticleTypes = new object[] {},
                    Dates = new object[] { specimen.AssociatedDate },

                    // Term fields
                    Tags = specimen.Tags,
                    Country = new object[] {specimen.Country},
                    CollectionNames = specimen.CollectionNames,
                    CollectionPlans = specimen.CollectionPlans,
                    PrimaryClassification = specimen.PrimaryClassification,
                    SecondaryClassification = specimen.SecondaryClassification,
                    TertiaryClassification = specimen.TertiaryClassification,
                    AssociationNames = specimen.Associations.Where(x => !string.IsNullOrWhiteSpace(x.Name)).Select(x => x.Name).ToArray(),
                    ItemTradeLiteraturePrimarySubject = (string) null,
                    ItemTradeLiteraturePublicationDate = (string) null,
                    ItemTradeLiteraturePrimaryRole = (string) null,
                    ItemTradeLiteraturePrimaryName = (string) null,
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
                        ((article.RelatedItemSpecimenIds.Count > 1) ? 1 : 0) +
                        ((article.Tags.Any()) ? 1 : 0) +
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
                    SpeciesHabitats = new object[] {},
                    SpeciesDepths = new object[] {},
                    SpeciesWaterColumnLocations = new object[] {},
                    Phylum = (string) null,
                    Class = (string) null,
                    SpecimenScientificGroup = (string) null,
                    SpecimenDiscipline = (string) null,
                    ArticleTypes = article.Types,
                    Dates = new int[] { },

                    // Term fields
                    Tags = new object[] {article.Tags, article.GeographicTags},
                    Country = new object[] {},
                    CollectionNames = new object[] {},
                    CollectionPlans = new object[] { },
                    PrimaryClassification = (string) null,
                    SecondaryClassification = (string) null,
                    TertiaryClassification = (string) null,
                    AssociationNames = new object[] {},
                    ItemTradeLiteraturePrimarySubject = (string) null,
                    ItemTradeLiteraturePublicationDate = (string) null,
                    ItemTradeLiteraturePrimaryRole = (string) null,
                    ItemTradeLiteraturePrimaryName = (string) null,
                });

            Index(x => x.Id, FieldIndexing.No);
            Index(x => x.Name, FieldIndexing.No);
            Index(x => x.Content, FieldIndexing.Analyzed);
            Index(x => x.Summary, FieldIndexing.No);
            Index(x => x.ThumbnailUri, FieldIndexing.No);

            Index(x => x.MediaIrns, FieldIndexing.NotAnalyzed);
            Index(x => x.TaxonomyIrn, FieldIndexing.NotAnalyzed);
            Index(x => x.Tags, FieldIndexing.NotAnalyzed);
            Index(x => x.Country, FieldIndexing.NotAnalyzed);
            Index(x => x.CollectionNames, FieldIndexing.NotAnalyzed);
            Index(x => x.PrimaryClassification, FieldIndexing.NotAnalyzed);
            Index(x => x.SecondaryClassification, FieldIndexing.NotAnalyzed);
            Index(x => x.TertiaryClassification, FieldIndexing.NotAnalyzed);
            Index(x => x.AssociationNames, FieldIndexing.NotAnalyzed);

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
            TermVector(x => x.SpeciesHabitats, FieldTermVector.Yes);
            TermVector(x => x.SpeciesDepths, FieldTermVector.Yes);
            TermVector(x => x.SpeciesWaterColumnLocations, FieldTermVector.Yes);
            TermVector(x => x.Phylum, FieldTermVector.Yes);
            TermVector(x => x.Class, FieldTermVector.Yes);
            TermVector(x => x.SpecimenScientificGroup, FieldTermVector.Yes);
            TermVector(x => x.SpecimenDiscipline, FieldTermVector.Yes);
            TermVector(x => x.ArticleTypes, FieldTermVector.Yes);
            TermVector(x => x.Dates, FieldTermVector.Yes);
        }
    }
}