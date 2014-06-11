using System.Linq;
using CollectionsOnline.Core.Models;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace CollectionsOnline.Core.Indexes
{
    public class CombinedSearch : AbstractMultiMapIndexCreationTask<CombinedSearchResult>
    {
        public CombinedSearch()
        {
            AddMap<Item>(items => 
                from item in items
                where item.IsHidden == false
                select new
                {
                    // Content fields
                    Id = item.Id,
                    Name = item.ObjectName,
                    Content = new object[] { item.ObjectName, item.Discipline, item.RegistrationNumber },
                    Summary = item.Summary,
                    ThumbUrl = item.Media.FirstOrDefault() != null ? item.Media.FirstOrDefault().Url : (string)null,

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
                    StoryTypes = new object[] { },
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
                    // Content fields
                    Id = species.Id,
                    Name = species.Taxonomy.CommonName ?? species.Taxonomy.Species ?? species.Taxonomy.Genus,
                    Content = new object[] { species.AnimalType, species.AnimalSubType, species.Taxonomy.CommonName, species.Taxonomy.Species, species.Taxonomy.Genus, species.Taxonomy.Family, species.Taxonomy.Order, species.Taxonomy.Class, species.Taxonomy.Phylum },
                    Summary = species.Summary,
                    ThumbUrl = species.Media.FirstOrDefault() != null ? species.Media.FirstOrDefault().Url : (string) null,

                    // Sort fields
                    Quality =
                        ((!string.IsNullOrWhiteSpace(species.IdentifyingCharacters) || !string.IsNullOrWhiteSpace(species.Biology) || !string.IsNullOrWhiteSpace(species.Habitat) || !string.IsNullOrWhiteSpace(species.Endemicity) || !string.IsNullOrWhiteSpace(species.Diet)) ? 1 : 0) +
                        ((!string.IsNullOrWhiteSpace(species.BriefId) || !string.IsNullOrWhiteSpace(species.Hazards)) ? 1 : 0) +
                        (species.Media.Count * 2) +
                        ((species.SpecimenIds.Any()) ? 1 : 0),

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
                    StoryTypes = new object[] {},
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
                    // Content fields
                    Id = specimen.Id,
                    Name = specimen.ObjectName ?? specimen.Taxonomy.ScientificName,
                    Content = new object[] { specimen.ScientificGroup, specimen.Type, specimen.RegistrationNumber, specimen.Discipline, specimen.Country },
                    Summary = specimen.Summary,
                    ThumbUrl = specimen.Media.FirstOrDefault() != null ? specimen.Media.FirstOrDefault().Url : (string) null,

                    // Sort fields
                    Quality =
                        ((!string.IsNullOrWhiteSpace(specimen.Year) || !string.IsNullOrWhiteSpace(specimen.RecordedBy) || !string.IsNullOrWhiteSpace(specimen.TypeStatus)) ? 1 : 0) +
                        ((!string.IsNullOrWhiteSpace(specimen.DecimalLatitude) || !string.IsNullOrWhiteSpace(specimen.DecimalLongitude)) ? 1 : 0) +
                        (specimen.Media.Count * 2) +
                        ((!string.IsNullOrWhiteSpace(specimen.Taxonomy.ScientificName)) ? 1 : 0),

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
                    StoryTypes = new object[] {},
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

            AddMap<Story>(stories =>
                from story in stories
                where story.IsHidden == false
                select new
                {
                    // Content fields
                    Id = story.Id,
                    Name = story.Title,
                    Content = new object[] {story.Content, story.ContentSummary},
                    Summary = story.Summary,
                    ThumbUrl = story.Media.FirstOrDefault() != null ? story.Media.FirstOrDefault().Url : (string) null,

                    // Sort fields
                    Quality =
                        ((story.RelatedItemIds.Count > 1) ? 1 : 0) +
                        ((story.Tags.Any()) ? 1 : 0) +
                        (story.Media.Count * 2) +
                        ((story.ChildStoryIds.Any()) ? 1 : 0),

                    // Facet fields
                    Type = "Story",
                    Category = "History & Technology",
                    HasImages = (story.Media.Any()) ? "Yes" : (string) null,
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
                    StoryTypes = story.Types,
                    Dates = new int[] { },

                    // Term fields
                    Tags = new object[] {story.Tags, story.GeographicTags},
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
            Index(x => x.ThumbUrl, FieldIndexing.No);
            
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
            Store(x => x.ThumbUrl, FieldStorage.Yes);
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
            TermVector(x => x.StoryTypes, FieldTermVector.Yes);
            TermVector(x => x.Dates, FieldTermVector.Yes);
        }
    }
}