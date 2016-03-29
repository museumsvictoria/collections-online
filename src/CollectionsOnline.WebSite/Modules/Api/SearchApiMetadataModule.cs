using System.Collections.Generic;
using CollectionsOnline.Core.Config;
using CollectionsOnline.WebSite.Models.Api;
using Nancy;
using Nancy.Metadata.Modules;
using Serilog;

namespace CollectionsOnline.WebSite.Modules.Api
{
    public class SearchApiMetadataModule : MetadataModule<ApiMetadata>
    {
        public SearchApiMetadataModule()
        {
            Log.Logger.Debug("Creating Search Api Metadata");

            Describe["search-api"] = description =>
            {
                return new ApiMetadata
                {
                    Name = description.Name,
                    Method = description.Method,
                    Path = description.Path,
                    Description = "Search the collection. All parameters used should be UTF-8 URL-Encoded strings.",
                    Parameters = new[]
                    {
                        new ApiParameter
                        {
                            Parameter = "query",
                            Necessity = "optional",
                            Description = "Specify a search term. Can contain one or more values. In order to add more terms repeat the query parameter as follows \"query=coin&query=india\"",
                            ExampleValues = new[] { "coin", "india" }
                        },
                        new ApiParameter
                        {
                            Parameter = "sort",
                            Necessity = "optional",
                            Description = "The order in which to sort return results. Regardless of the value used the direction of the order is always descending. Default value is relevance if the query parameter is present, otherwise quality is used as the default value.",
                            ValidValues = new []
                            {
                                new ApiParameterValidValue
                                {
                                    Name = "quality",
                                    Description = "The quality of the record based on how many fields and images a record may have."
                                },
                                new ApiParameterValidValue
                                {
                                    Name = "relevance",
                                    Description = "The relevance of the record based on the search query. Only valid if the query parameter is used."
                                },
                                new ApiParameterValidValue
                                {
                                    Name = "date",
                                    Description = "The date the record was last modified."
                                }
                            },
                            ExampleValues = new[] { "quality", "relevance", "date" }
                        },
                        new ApiParameter
                        {
                            Parameter = "mindatemodified",
                            Necessity = "optional",
                            Description = "Minimum date a record was modified in ISO 8601 UTC format. Records with a modified date greater than or equal to this value will be returned.",
                            ExampleValues = new[] { "2014-06-21T12:00:00Z" }
                        },
                        new ApiParameter
                        {
                            Parameter = "maxdatemodified",
                            Necessity = "optional",
                            Description = "Maximum date a record was modified in ISO 8601 UTC format. Records with a modified date less than or equal to this value will be returned.",
                            ExampleValues = new[] { "2015-11-06T00:00:00Z" }
                        },
                        new ApiParameter
                        {
                            Parameter = "hasimages",
                            Necessity = "optional",
                            Description = "Specify whether or not the returned records contain images. Parameter is considered a facet.",
                            ExampleValues = new[] { "yes", "no" }
                        },
                        new ApiParameter
                        {
                            Parameter = "ondisplay",
                            Necessity = "optional",
                            Description = "Specify whether or not the returned records are on display. Parameter is considered a facet.",
                            ExampleValues = new[] { "yes", "no" }
                        },
                        new ApiParameter
                        {
                            Parameter = "recordtype",
                            Necessity = "optional",
                            Description = "Type of record to return. Parameter is considered a facet.",
                            ExampleValues = new[] { "article", "item", "species", "specimen" }
                        },
                        new ApiParameter
                        {
                            Parameter = "category",
                            Necessity = "optional",
                            Description = "Collection category of a record. Parameter is considered a facet.",
                            ExampleValues = new[] { "natural sciences", "indigenous collections" }
                        },
                        new ApiParameter
                        {
                            Parameter = "collectingarea",
                            Necessity = "optional",
                            Description = "Collecting area of a record. Can contain one or more values. In order to add more terms repeat the query parameter as follows \"collectingarea=ornithology&collectingarea=mammalogy\". Parameter is considered a facet.",
                            ExampleValues = new[] { "ornithology", "mammalogy" }
                        },
                        new ApiParameter
                        {
                            Parameter = "specimenscientificgroup",
                            Necessity = "optional",
                            Description = "Specimen scientific group of a record, will only return specimen records. Parameter is considered a facet.",
                            ExampleValues = new[] { "geology", "invertebrate zoology" }
                        },
                        new ApiParameter
                        {
                            Parameter = "specimenscientificgroup",
                            Necessity = "optional",
                            Description = "Specimen scientific group of a record, will only return specimen records. Parameter is considered a facet.",
                            ExampleValues = new[] { "geology", "invertebrate zoology" }
                        },
                        new ApiParameter
                        {
                            Parameter = "itemtype",
                            Necessity = "optional",
                            Description = "The general type of specimen or item, will only return specimen or item records. Parameter is considered a facet.",
                            ExampleValues = new[] { "object", "specimen" }
                        },
                        new ApiParameter
                        {
                            Parameter = "speciestype",
                            Necessity = "optional",
                            Description = "The broad category a species may be considered to belong to, will only return species records. Parameter is considered a facet.",
                            ExampleValues = new[] { "snakes", "birds" }
                        },
                        new ApiParameter
                        {
                            Parameter = "imagelicence",
                            Necessity = "optional",
                            Description = "The open access licences that are attributed to a records images. Can contain one or more values. In order to add more terms repeat the query parameter as follows \"imagelicence=public+domain&imagelicence=cc+by\". Parameter is considered a facet.",
                            ExampleValues = new[] { "public domain", "cc by" }
                        },
                        new ApiParameter
                        {
                            Parameter = "articletype",
                            Necessity = "optional",
                            Description = "The type of article, will only return article records. Can contain one or more values. In order to add more terms repeat the query parameter as follows \"articletype=party&articletype=physical+object\". Parameter is considered a facet.",
                            ExampleValues = new[] { "party", "physical object" }
                        },
                        new ApiParameter
                        {
                            Parameter = "displaylocation",
                            Necessity = "optional",
                            Description = "The display location at Museum Victoria venues for the record, will only return item or specimen records. Parameter is considered a facet.",
                            ExampleValues = new[] { "melbourne museum", "scienceworks" }
                        },
                        new ApiParameter
                        {
                            Parameter = "keyword",
                            Necessity = "optional",
                            Description = "The keywords generally used to describe the record, sorta like tags. Keywords can come from a variety of other fields on records such as...<br/>articles:<br/><code>keywords</code><br/>items:<br/><code>keywords, audioVisualRecordingDetails, modelNames, archeologyActivity, archeologySpecificActivity, archeologyDecoration, numismaticsSeries, tradeLiteraturePrimarySubject, tradeLiteraturePublicationTypes, brands</code><br/>species:<br/><code>conservationStatuses, animalSubType</code><br/>specimens:<br/><code>keywords, expeditionNameExpeditionName</code>.",
                            ExampleValues = new[] { "handcrafts", "australian art" }
                        },
                        new ApiParameter
                        {
                            Parameter = "locality",
                            Necessity = "optional",
                            Description = "The general locality a record may be associated with. Locality can come from a variety of other fields on records such as...<br/>articles:<br/><code>localities</code><br/>items:<br/><code>locality, region, state, country, indigenousCulturesLocalities</code><br/>species:<br/><code>nationalParks</code><br/>specimens:<br/><code>locality, region, state, country, ocean, continent, district, town, nearestNamedPlace, tektitesLocalStrewnfield, tektitesGlobalStrewnfield</code>.",
                            ExampleValues = new[] { "barossa valley", "germany" }
                        },
                        new ApiParameter
                        {
                            Parameter = "collection",
                            Necessity = "optional",
                            Description = "The museum collection a record belongs to, will only return item or specimen records. Collection can come from a variety of other fields on records such as...<br/>items:<br/><code>collectionNames, discipline</code><br/>specimens:<br/><code>collectionNames, discipline</code>.",
                            ExampleValues = new[] { "spencer collection", "royal exhibition building collection" }
                        },
                        new ApiParameter
                        {
                            Parameter = "date",
                            Necessity = "optional",
                            Description = "The general date a record may be associated with, due to the variation in the way date is cataloged the string must be an exact match and is not parsed into a standardised date format, will only return item or specimen records. Collection can come from a variety of other fields on records such as...<br/>items:<br/><code>date, indigenousCulturesDate, indigenousCulturesDateCollected, archeologyManufactureDate, philatelyDateIssued, tradeLiteraturePublicationDate</code><br/>specimens:<br/><code>date</code>.",
                            ExampleValues = new[] { "02 nov 1956", "05/11/1908" }
                        },
                        new ApiParameter
                        {
                            Parameter = "culturalgroup",
                            Necessity = "optional",
                            Description = "The indigenous cultures cultural group a record may be associated with, will only return item records.",
                            ExampleValues = new[] { "woi wurrung", "barapa barapa" }
                        },
                        new ApiParameter
                        {
                            Parameter = "classification",
                            Necessity = "optional",
                            Description = "The museum classification a record may belong to, will only return item and specimen records.",
                            ExampleValues = new[] { "boring apparatus", "dust collecting equipment" }
                        },
                        new ApiParameter
                        {
                            Parameter = "name",
                            Necessity = "optional",
                            Description = "A person or companies name a record may be associated with. Name can come from a variety of other fields on records such as...<br/>articles:<br/><code>author.fullName</code><br/>items:<br/><code>association.name, indigenousCulturesPhotographer, indigenousCulturesAuthor, indigenousCulturesIllustrator, indigenousCulturesMaker, indigenousCulturesCollector, indigenousCulturesLetterTo, indigenousCulturesLetterFrom, brand.name, archeologyManufactureName, tradeLiteraturePrimaryName, artworkPublisher</code><br/>species:<br/><code>author.fullName</code><br/>specimens:<br/><code>association.name</code>.",
                            ExampleValues = new[] { "Mrs Jane Burn", "Senator George F. Pearce - Australian Government" }
                        },
                        new ApiParameter
                        {
                            Parameter = "technique",
                            Necessity = "optional",
                            Description = "The archeology technique used to create an object, will only return item records.",
                            ExampleValues = new[] { "caved bone", "machine-made" }
                        },
                        new ApiParameter
                        {
                            Parameter = "denomination",
                            Necessity = "optional",
                            Description = "The numismatics denomination of a record, will only return item records.",
                            ExampleValues = new[] { "half crown", "obol" }
                        },
                        new ApiParameter
                        {
                            Parameter = "habitat",
                            Necessity = "optional",
                            Description = "The <code>habitat</code> or <code>whereToLook</code> a species lives in, will only return species records.",
                            ExampleValues = new[] { "woodland", "seagrass meadows" }
                        },
                        new ApiParameter
                        {
                            Parameter = "taxon",
                            Necessity = "optional",
                            Description = "The <code>taxonName, kingdom, phylum, subphylum, superclass, class, subclass, superorder, order, suborder, infraorder, superfamily, family, subfamily, commonName, otherCommonNames</code> an item, species or specimen may belong to.",
                            ExampleValues = new[] { "mammalia", "Ornithorhynchus anatinus" }
                        },
                        new ApiParameter
                        {
                            Parameter = "typestatus",
                            Necessity = "optional",
                            Description = "The type status of a specimen, will only return specimen records.",
                            ExampleValues = new[] { "holotype", "paratype" }
                        },
                        new ApiParameter
                        {
                            Parameter = "geotype",
                            Necessity = "optional",
                            Description = "The <code>petrologyRockClass, petrologyRockGroup, mineralogyVariety, mineralogyGroup, mineralogyClass, meteoritesClass, meteoritesGroup, tektitesClassification, mineralogySpecies</code> a specimen may belong to.",
                            ExampleValues = new[] { "agate", "copper" }
                        },
                        new ApiParameter
                        {
                            Parameter = "museumlocation",
                            Necessity = "optional",
                            Description = "The <code>gallery</code> or <code>venue</code> a record may be on display at one of Museum Victoria's venues, will only return item and specimen records.",
                            ExampleValues = new[] { "immigration museum", "bunjilaka - first peoples" }
                        },
                        new ApiParameter
                        {
                            Parameter = "article",
                            Necessity = "optional",
                            Description = "The title of an article a record may be associated with, will only return article, item and specimen records.",
                            ExampleValues = new[] { "immigration museum", "bunjilaka - first peoples" }
                        },
                        new ApiParameter
                        {
                            Parameter = "article",
                            Necessity = "optional",
                            Description = "The title of an article a record may be associated with, will only return article, item and specimen records.",
                            ExampleValues = new[] { "arms collection", "csirac games - a summary" }
                        },
                        new ApiParameter
                        {
                            Parameter = "speciesendemicity",
                            Necessity = "optional",
                            Description = "The endemicity of a species, will only return species records.",
                            ExampleValues = new[] { "introduced pest", "native to australia" }
                        }
                    },
                    StatusCodes = new Dictionary<HttpStatusCode, string>
                    {
                        { HttpStatusCode.OK, "A bunch of things were able to be searched ok." }
                    },
                    ExampleUrl = description.Path,
                };
            };
        }
    }
}