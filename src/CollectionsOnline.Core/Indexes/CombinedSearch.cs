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
            AddMap<Item>(items => from item in items
                                        where item.IsHidden == false
                                        select new
                                        {
                                            Id = item.Id,
                                            Name = item.Name,
                                            Content = new object[] { item.Name, item.Discipline, item.RegistrationNumber },
                                            Summary = item.Summary,
                                            ThumbUrl = item.Media.FirstOrDefault() != null ? item.Media.FirstOrDefault().Url : (string)null,

                                            Type = "Item",
                                            Category = item.Category,
                                            HasImages = (item.Media.Any()) ? "Yes" : (string)null,
                                            Discipline = item.Discipline,
                                            Dates = item.AssociatedDates.ToArray(),
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
                                            
                                            Tags = item.Tags,
                                            Country = item.Associations.Where(x => !string.IsNullOrWhiteSpace(x.Country)).Select(x => x.Country).ToArray(),
                                            ItemCollectionNames = item.CollectionNames,
                                            ItemPrimaryClassification = item.PrimaryClassification,
                                            ItemSecondaryClassification = item.SecondaryClassification,
                                            ItemTertiaryClassification = item.TertiaryClassification,
                                            ItemAssociationNames = item.Associations.Where(x => !string.IsNullOrWhiteSpace(x.Name)).Select(x => x.Name).ToArray(),
                                            ItemTradeLiteraturePrimarySubject = item.TradeLiteraturePrimarySubject,
                                            ItemTradeLiteraturePublicationDate = item.TradeLiteraturePublicationDate,
                                            ItemTradeLiteraturePrimaryRole = item.TradeLiteraturePrimaryRole,
                                            ItemTradeLiteraturePrimaryName = item.TradeLiteraturePrimaryName,
                                        });

            AddMap<Species>(speciesDocs => from species in speciesDocs
                                        where species.IsHidden == false
                                        select new
                                        {
                                            Id = species.Id,
                                            Name = species.CommonNames.FirstOrDefault() ?? species.SpeciesName ?? species.Genus,
                                            Content = new object[] { species.AnimalType, species.AnimalSubType, species.HigherClassification, species.CommonNames },
                                            Summary = species.Summary,
                                            ThumbUrl = species.Media.FirstOrDefault() != null ? species.Media.FirstOrDefault().Url : (string)null,

                                            Type = "Species",
                                            Category = "Natural Sciences",
                                            HasImages = (species.Media.Any()) ? "Yes" : (string)null,
                                            Discipline = (string)null,
                                            Dates = new object[] { },
                                            ItemType = (string)null,
                                            SpeciesType = species.AnimalType,
                                            SpeciesSubType = species.AnimalSubType,
                                            SpeciesHabitats = species.Habitats,
                                            SpeciesDepths = species.Depths,
                                            SpeciesWaterColumnLocations = species.WaterColumnLocations,
                                            Phylum = species.Phylum,
                                            Class = species.Class,
                                            SpecimenScientificGroup = (string)null,
                                            SpecimenDiscipline = (string)null,
                                            StoryTypes = new object[] { },

                                            Tags = new object[] { },
                                            Country = new object[] { },
                                            ItemCollectionNames = new object[] { },
                                            ItemPrimaryClassification = (string)null,
                                            ItemSecondaryClassification = (string)null,
                                            ItemTertiaryClassification = (string)null,
                                            ItemAssociationNames = new object[] { },
                                            ItemTradeLiteraturePrimarySubject = (string)null,
                                            ItemTradeLiteraturePublicationDate = (string)null,
                                            ItemTradeLiteraturePrimaryRole = (string)null,
                                            ItemTradeLiteraturePrimaryName = (string)null,
                                        });

            AddMap<Specimen>(specimens => from specimen in specimens
                                        where specimen.IsHidden == false
                                        select new
                                        {
                                            Id = specimen.Id,
                                            Name = specimen.ScientificName ?? specimen.AcceptedNameUsage,
                                            Content = new object[] { specimen.ScientificGroup, specimen.Type, specimen.RegistrationNumber, specimen.Discipline, specimen.Country },
                                            Summary = specimen.Summary,
                                            ThumbUrl = specimen.Media.FirstOrDefault() != null ? specimen.Media.FirstOrDefault().Url : (string)null,

                                            Type = "Specimen",
                                            Category = "Natural Sciences",
                                            HasImages = (specimen.Media.Any()) ? "Yes" : (string)null,
                                            Discipline = specimen.Discipline,
                                            Dates = new object[] { specimen.AssociatedDate },
                                            ItemType = (string)null,
                                            SpeciesType = (string)null,
                                            SpeciesSubType = (string)null,
                                            SpeciesHabitats = new object[] { },
                                            SpeciesDepths = new object[] { },
                                            SpeciesWaterColumnLocations = new object[] { },
                                            Phylum = specimen.Phylum,
                                            Class = specimen.Class,
                                            SpecimenScientificGroup = specimen.ScientificGroup,
                                            SpecimenDiscipline = specimen.Discipline,
                                            StoryTypes = new object[] { },

                                            Tags = new object[] { },
                                            Country = new object[] { specimen.Country },
                                            ItemCollectionNames = new object[] { },
                                            ItemPrimaryClassification = (string)null,
                                            ItemSecondaryClassification = (string)null,
                                            ItemTertiaryClassification = (string)null,
                                            ItemAssociationNames = new object[] { },
                                            ItemTradeLiteraturePrimarySubject = (string)null,
                                            ItemTradeLiteraturePublicationDate = (string)null,
                                            ItemTradeLiteraturePrimaryRole = (string)null,
                                            ItemTradeLiteraturePrimaryName = (string)null,
                                        });

            AddMap<Story>(stories =>    from story in stories
                                        where story.IsHidden == false
                                        select new
                                        {
                                            Id = story.Id,
                                            Name = story.Title,
                                            Content = new object[] { story.Content, story.ContentSummary },
                                            Summary = story.Summary,
                                            ThumbUrl = story.Media.FirstOrDefault() != null ? story.Media.FirstOrDefault().Url : (string)null,

                                            Type = "Story",
                                            Category = "History & Technology",
                                            HasImages = (story.Media.Any()) ? "Yes" : (string)null,
                                            Discipline = (string)null,
                                            Dates = new int[] { },
                                            ItemType = (string)null,
                                            SpeciesType = (string)null,
                                            SpeciesSubType = (string)null,
                                            SpeciesHabitats = new object[] { },
                                            SpeciesDepths = new object[] { },
                                            SpeciesWaterColumnLocations = new object[] { },
                                            Phylum = (string)null,
                                            Class = (string)null,
                                            SpecimenScientificGroup = (string)null,
                                            SpecimenDiscipline = (string)null,
                                            StoryTypes = story.Types,

                                            Tags = new object[] { story.Tags, story.GeographicTags },
                                            Country = new object[] { },
                                            ItemCollectionNames = new object[] { },
                                            ItemPrimaryClassification = (string)null,
                                            ItemSecondaryClassification = (string)null,
                                            ItemTertiaryClassification = (string)null,
                                            ItemAssociationNames = new object[] { },
                                            ItemTradeLiteraturePrimarySubject = (string)null,
                                            ItemTradeLiteraturePublicationDate = (string)null,
                                            ItemTradeLiteraturePrimaryRole = (string)null,
                                            ItemTradeLiteraturePrimaryName = (string)null,
                                        });


            Index(x => x.Id, FieldIndexing.No);
            Index(x => x.Name, FieldIndexing.No);
            Index(x => x.Summary, FieldIndexing.No);
            Index(x => x.ThumbUrl, FieldIndexing.No);
            Index(x => x.Type, FieldIndexing.NotAnalyzed);
            Index(x => x.Content, FieldIndexing.Analyzed);

            Index(x => x.Tags, FieldIndexing.NotAnalyzed);
            Index(x => x.Country, FieldIndexing.NotAnalyzed);
            Index(x => x.ItemCollectionNames, FieldIndexing.NotAnalyzed);
            Index(x => x.ItemPrimaryClassification, FieldIndexing.NotAnalyzed);
            Index(x => x.ItemSecondaryClassification, FieldIndexing.NotAnalyzed);
            Index(x => x.ItemTertiaryClassification, FieldIndexing.NotAnalyzed);
            Index(x => x.ItemAssociationNames, FieldIndexing.NotAnalyzed);

            Store(x => x.Id, FieldStorage.Yes);            
            Store(x => x.Name, FieldStorage.Yes);
            Store(x => x.Summary, FieldStorage.Yes);
            Store(x => x.ThumbUrl, FieldStorage.Yes);
            Store(x => x.Type, FieldStorage.Yes);
            Store(x => x.Content, FieldStorage.No);

            Store(x => x.Tags, FieldStorage.Yes);
            Store(x => x.Country, FieldStorage.Yes);
            Store(x => x.ItemCollectionNames, FieldStorage.Yes);
            Store(x => x.ItemPrimaryClassification, FieldStorage.Yes);
            Store(x => x.ItemSecondaryClassification, FieldStorage.Yes);
            Store(x => x.ItemTertiaryClassification, FieldStorage.Yes);
            Store(x => x.ItemAssociationNames, FieldStorage.Yes);

            Analyzers.Add(x => x.Content, "SimpleAnalyzer");
            Suggestion(x => x.Content);
        }
    }
}
