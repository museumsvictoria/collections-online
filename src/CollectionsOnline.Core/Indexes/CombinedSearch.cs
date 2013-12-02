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
                                        select new
                                        {
                                            Content = new object[] { item.Name, item.Discipline, item.RegistrationNumber },
                                            Type = "Item",
                                            Id = item.Id,
                                            Name = item.Name,
                                            Category = item.Category,
                                            Tags = item.Tags,

                                            ItemType = item.Type,
                                            ItemCollectionNames = item.CollectionNames,
                                            ItemPrimaryClassification = item.PrimaryClassification,
                                            ItemSecondaryClassification = item.SecondaryClassification,
                                            ItemTertiaryClassification = item.TertiaryClassification,
                                            ItemAssociationNames = item.Associations.Select(x => x.Name).ToArray(),

                                            StoryTypes = new object[] { },
                                            SpeciesType = (string)null,
                                            SpeciesSubType = (string)null,
                                        });

            AddMap<Species>(speciesDocs => from species in speciesDocs
                                        select new
                                        {
                                            Content = new object[] { species.AnimalType, species.AnimalSubType, species.HigherClassification, species.CommonNames },
                                            Type = "Species",
                                            Id = species.Id,
                                            Name = species.CommonNames.FirstOrDefault() ?? species.SpeciesName,
                                            Category = "Natural Sciences",
                                            Tags = new object[] { },

                                            ItemType = (string)null,
                                            ItemCollectionNames = new object[] { },
                                            ItemPrimaryClassification = (string)null,
                                            ItemSecondaryClassification = (string)null,
                                            ItemTertiaryClassification = (string)null,
                                            ItemAssociationNames = new object[] { },

                                            StoryTypes = new object[] { },
                                            SpeciesType = species.AnimalType,
                                            SpeciesSubType = species.AnimalSubType,
                                        });

            AddMap<Specimen>(specimens => from specimen in specimens
                                        select new
                                        {
                                            Content = new object[] { specimen.ScientificGroup, specimen.Type, specimen.RegistrationNumber, specimen.Discipline, specimen.Country },
                                            Type = "Specimen",
                                            Id = specimen.Id,
                                            Name = specimen.ScientificName ?? specimen.AcceptedNameUsage,
                                            Category = "Natural Sciences",
                                            Tags = new object[] { },

                                            ItemType = (string)null,
                                            ItemCollectionNames = new object[] { },
                                            ItemPrimaryClassification = (string)null,
                                            ItemSecondaryClassification = (string)null,
                                            ItemTertiaryClassification = (string)null,
                                            ItemAssociationNames = new object[] { },

                                            StoryTypes = new object[] { },
                                            SpeciesType = (string)null,
                                            SpeciesSubType = (string)null,
                                        });

            AddMap<Story>(stories => from story in stories
                                        select new
                                        {
                                            Content = new object[] { story.Content, story.ContentSummary },
                                            Type = "Story",
                                            Id = story.Id,
                                            Name = story.Title,
                                            Category = "History & Technology",
                                            Tags = new object[] { story.Tags, story.GeographicTags },

                                            ItemType = (string)null,
                                            ItemCollectionNames = new object[] { },
                                            ItemPrimaryClassification = (string)null,
                                            ItemSecondaryClassification = (string)null,
                                            ItemTertiaryClassification = (string)null,
                                            ItemAssociationNames = new object[] { },

                                            StoryTypes = story.Types,
                                            SpeciesType = (string)null,
                                            SpeciesSubType = (string)null,
                                        });


            Index(x => x.Content, FieldIndexing.Analyzed);
            Index(x => x.ItemCollectionNames, FieldIndexing.NotAnalyzed);
            Index(x => x.ItemPrimaryClassification, FieldIndexing.NotAnalyzed);
            Index(x => x.ItemSecondaryClassification, FieldIndexing.NotAnalyzed);
            Index(x => x.ItemTertiaryClassification, FieldIndexing.NotAnalyzed);
            Index(x => x.ItemAssociationNames, FieldIndexing.NotAnalyzed);
            Index(x => x.Tags, FieldIndexing.NotAnalyzed);

            Store(x => x.Content, FieldStorage.No);
            Store(x => x.ItemCollectionNames, FieldStorage.Yes);
            Store(x => x.ItemPrimaryClassification, FieldStorage.Yes);
            Store(x => x.ItemSecondaryClassification, FieldStorage.Yes);
            Store(x => x.ItemTertiaryClassification, FieldStorage.Yes);
            Store(x => x.ItemAssociationNames, FieldStorage.Yes);
            Store(x => x.Tags, FieldStorage.Yes);
        }
    }
}
