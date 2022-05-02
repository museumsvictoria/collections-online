using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AutoMapper;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Import.Extensions;
using IMu;
using Raven.Abstractions.Commands;
using Raven.Abstractions.Data;
using Raven.Abstractions.Extensions;
using Raven.Client;
using Serilog;
using Constants = CollectionsOnline.Core.Config.Constants;

namespace CollectionsOnline.Import.Factories
{
    public class SpeciesFactory : IEmuAggregateRootFactory<Species>
    {
        private readonly ITaxonomyFactory _taxonomyFactory;
        private readonly IMediaFactory _mediaFactory;
        private readonly ISummaryFactory _summaryFactory;

        public SpeciesFactory(
            ITaxonomyFactory taxonomyFactory,
            IMediaFactory mediaFactory,
            ISummaryFactory summaryFactory)
        {
            _taxonomyFactory = taxonomyFactory;
            _mediaFactory = mediaFactory;
            _summaryFactory = summaryFactory;
        }

        public string ModuleName => "enarratives";

        public string[] Columns
        {
            get
            {
                return new[]
                {
                    "irn",
                    "AdmPublishWebNoPassword",
                    "AdmDateModified",
                    "AdmTimeModified",
                    "SpeTaxonGroup",
                    "SpeTaxonSubGroup",
                    "SpeColour_tab",
                    "SpeMaximumSize",
                    "SpeUnit",
                    "SpeHabitat_tab",
                    "SpeWhereToLook_tab",
                    "SpeWhenActive_tab",
                    "SpeNationalParks_tab",
                    "SpeDiet",
                    "SpeDietCategories_tab",
                    "SpeFastFact",
                    "SpeHabitatNotes",
                    "SpeDistribution",
                    "SpeBiology",
                    "SpeIdentifyingCharacters",
                    "SpeBriefID",
                    "SpeHazards",
                    "SpeEndemicity",
                    "SpeCommercialSpecies",
                    "conservation=[SpeConservationList_tab,SpeStatus_tab]",
                    "SpeWeb",
                    "SpePlant_tab",
                    "SpeFlightStart",
                    "SpeFlightEnd",
                    "SpeDepth_tab",
                    "SpeWaterColumnLocation_tab",
                    "taxa=TaxTaxaRef_tab.(irn,ClaKingdom,ClaPhylum,ClaSubphylum,ClaSuperclass,ClaClass,ClaSubclass,ClaSuperorder,ClaOrder,ClaSuborder,ClaInfraorder,ClaSuperfamily,ClaFamily,ClaSubfamily,ClaGenus,ClaSubgenus,ClaSpecies,ClaSubspecies,AutAuthorString,ClaApplicableCode,comname=[ComName_tab,ComStatus_tab])",
                    "relatedarticlespecies=AssAssociatedWithRef_tab.(irn,DetPurpose_tab)",
                    "relateditemspecimens=ObjObjectsRef_tab.(irn,MdaDataSets_tab)",
                    "authors=NarAuthorsRef_tab.(NamFirst,NamLast,NamFullName,BioLabel,media=MulMultiMediaRef_tab.(irn,MulTitle,MulIdentifier,MulMimeType,MdaDataSets_tab,metadata=[MdaElement_tab,MdaQualifier_tab,MdaFreeText_tab],DetAlternateText,RigCreator_tab,RigSource_tab,RigAcknowledgementCredit,RigCopyrightStatement,RigCopyrightStatus,RigLicence,RigLicenceDetails,ChaRepository_tab,ChaMd5Sum,AdmPublishWebNoPassword,AdmDateModified,AdmTimeModified))",
                    "dates=[NarDate0,NarExplanation_tab]",
                    "media=MulMultiMediaRef_tab.(irn,MulTitle,MulIdentifier,MulMimeType,MdaDataSets_tab,metadata=[MdaElement_tab,MdaQualifier_tab,MdaFreeText_tab],DetAlternateText,RigCreator_tab,RigSource_tab,RigAcknowledgementCredit,RigCopyrightStatement,RigCopyrightStatus,RigLicence,RigLicenceDetails,ChaRepository_tab,ChaMd5Sum,AdmPublishWebNoPassword,AdmDateModified,AdmTimeModified)",
                };
            }
        }

        public Terms Terms
        {
            get
            {
                var terms = new Terms();

                terms.Add("DetPurpose_tab", Constants.ImuSpeciesQueryString);

                return terms;
            }
        }

        public Species MakeDocument(Map map)
        {
            var species = new Species();

            species.Id = "species/" + map.GetEncodedString("irn");

            species.IsHidden = string.Equals(map.GetEncodedString("AdmPublishWebNoPassword"), "no", StringComparison.OrdinalIgnoreCase);

            species.DateModified = DateTime.ParseExact(
                $"{map.GetEncodedString("AdmDateModified")} {map.GetEncodedString("AdmTimeModified")}",
                "dd/MM/yyyy HH:mm",
                new CultureInfo("en-AU")).ToUniversalTime();

            species.AnimalType = map.GetEncodedString("SpeTaxonGroup");
            species.AnimalSubType = map.GetEncodedString("SpeTaxonSubGroup");

            species.Colours.AddRange(map.GetEncodedStrings("SpeColour_tab").Distinct());

            species.MaximumSize = $"{map.GetEncodedString("SpeMaximumSize")} {map.GetEncodedString("SpeUnit")}".Trim();

            species.Habitats = map.GetEncodedStrings("SpeHabitat_tab");
            species.WhereToLook = map.GetEncodedStrings("SpeWhereToLook_tab");
            species.WhenActive = map.GetEncodedStrings("SpeWhenActive_tab");
            species.NationalParks = map.GetEncodedStrings("SpeNationalParks_tab");

            species.Diet = map.GetEncodedString("SpeDiet");
            species.DietCategories = map.GetEncodedStrings("SpeDietCategories_tab");

            species.FastFact = map.GetEncodedString("SpeFastFact");
            species.Habitat = map.GetEncodedString("SpeHabitatNotes");
            species.Distribution = map.GetEncodedString("SpeDistribution");
            species.Biology = map.GetEncodedString("SpeBiology");
            species.GeneralDescription = map.GetEncodedString("SpeIdentifyingCharacters");
            species.BriefId = map.GetEncodedString("SpeBriefID");
            species.Hazards = map.GetEncodedString("SpeHazards");
            species.Endemicity = map.GetEncodedString("SpeEndemicity");
            species.Commercial = map.GetEncodedString("SpeCommercialSpecies");

            // Get Conservation Status
            foreach (var conservationMap in map.GetMaps("conservation"))
            {
                var authority = conservationMap.GetEncodedString("SpeConservationList_tab");
                var status = conservationMap.GetEncodedString("SpeStatus_tab");

                if(!string.IsNullOrWhiteSpace(authority) && !string.IsNullOrWhiteSpace(status))
                    species.ConservationStatuses.Add($"{authority}: {status}");
            }

            // Animal specific fields (spider/butterflies) 
            species.Web = map.GetEncodedString("SpeWeb");
            species.Plants = map.GetEncodedStrings("SpePlant_tab");
            species.FlightStart = map.GetEncodedString("SpeFlightStart");
            species.FlightEnd = map.GetEncodedString("SpeFlightEnd");
            species.Depths = map.GetEncodedStrings("SpeDepth_tab");
            species.WaterColumnLocations = map.GetEncodedStrings("SpeWaterColumnLocation_tab");

            // Taxonomy
            var taxonomyMap = map.GetMaps("taxa").FirstOrDefault();
            species.Taxonomy = _taxonomyFactory.Make(taxonomyMap);

            // Licence
            species.Licence = Constants.Licences[LicenceType.CcBy];

            // Relationships

            // Related items/specimens (directly related)
            foreach (var relatedItemSpecimen in map.GetMaps("relateditemspecimens").Where(x => x != null && !string.IsNullOrWhiteSpace(x.GetEncodedString("irn"))))
            {
                if (relatedItemSpecimen.GetEncodedStrings("MdaDataSets_tab").Contains(Constants.ImuItemQueryString))
                    species.RelatedItemIds.Add($"items/{relatedItemSpecimen.GetEncodedString("irn")}");
                if (relatedItemSpecimen.GetEncodedStrings("MdaDataSets_tab").Contains(Constants.ImuSpecimenQueryString))
                    species.RelatedSpecimenIds.Add($"specimens/{relatedItemSpecimen.GetEncodedString("irn")}");
            }

            // Related articles/species 
            var relatedArticleSpeciesMap = map.GetMaps("relatedarticlespecies");
            if (relatedArticleSpeciesMap != null)
            {
                species.RelatedArticleIds.AddRangeUnique(relatedArticleSpeciesMap
                    .Where(x => x != null && x.GetEncodedStrings("DetPurpose_tab").Contains(Constants.ImuArticleQueryString))
                    .Select(x => $"articles/{x.GetEncodedString("irn")}"));

                species.RelatedSpeciesIds.AddRangeUnique(relatedArticleSpeciesMap
                    .Where(x => x != null && x.GetEncodedStrings("DetPurpose_tab").Contains(Constants.ImuSpeciesQueryString))
                    .Select(x => $"species/{x.GetEncodedString("irn")}"));
            }

            // Authors
            species.Authors = map.GetMaps("authors")
                .Where(x => x != null)
                .Select(x => new Author
                {
                    FirstName = x.GetEncodedString("NamFirst"),
                    LastName = x.GetEncodedString("NamLast"),
                    FullName = x.GetEncodedString("NamFullName"),
                    Biography = x.GetEncodedString("BioLabel"),
                    ProfileImage = _mediaFactory.Make(x.GetMaps("media").FirstOrDefault()) as ImageMedia
                }).ToList();

            // Year written
            var dateWritten = map.GetMaps("dates")
                .Where(x => x.GetEncodedString("NarExplanation_tab").Contains("date written", StringComparison.OrdinalIgnoreCase))
                .Select(x => x.GetEncodedString("NarDate0"))
                .FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(dateWritten))
            {
                if (DateTime.TryParseExact(dateWritten, "dd/MM/yyyy", new CultureInfo("en-AU"), DateTimeStyles.None, out var parsedDate))
                    species.YearWritten = parsedDate.Year.ToString();
                else if (DateTime.TryParseExact(dateWritten, "/MM/yyyy", new CultureInfo("en-AU"), DateTimeStyles.None, out parsedDate))
                    species.YearWritten = parsedDate.Year.ToString();
                else if (DateTime.TryParseExact(dateWritten, "yyyy", new CultureInfo("en-AU"), DateTimeStyles.None, out parsedDate))
                    species.YearWritten = parsedDate.Year.ToString();
            }

            // Media
            species.Media = _mediaFactory.Make(map.GetMaps("media"));

            // Assign thumbnail
            var media = species.Media.OfType<IHasThumbnail>().FirstOrDefault();
            if (media != null)
                species.ThumbnailUri = media.Thumbnail.Uri;

            // Build summary
            species.Summary = _summaryFactory.Make(species);

            // Display Title
            // TODO: Move to display title factory and encapsulate entire process
            if (species.Taxonomy != null)
            {
                var scientificName = _taxonomyFactory.MakeScientificName(QualifierRankType.None, null, species.Taxonomy);

                species.DisplayTitle = new[]
                {
                    scientificName, 
                    species.Taxonomy.CommonName
                }.Concatenate(", ");
            }

            if (string.IsNullOrWhiteSpace(species.DisplayTitle))
                species.DisplayTitle = "Species";

            Log.Logger.Debug("Completed {Id} creation with {MediaCount} media", species.Id, species.Media.Count);
            
            return species;
        }

        public void UpdateDocument(Species newDocument, Species existingDocument, IList<string> missingDocumentIds,
            IDocumentSession documentSession)
        {
            // Perform any denormalized updates
            var patchCommands = new List<ICommandData>();

            // Related Items update
            foreach (var itemIdToRemove in existingDocument.RelatedItemIds.Except(newDocument.RelatedItemIds))
            {
                patchCommands.Add(new PatchCommandData
                {
                    Key = itemIdToRemove,
                    Patches = new[]
                    {
                        new PatchRequest
                        {
                            Type = PatchCommandType.Remove,
                            AllPositions = true,
                            Name = "RelatedSpeciesIds",
                            Value = newDocument.Id
                        }
                    }
                });
            }
            foreach (var itemIdToAdd in newDocument.RelatedItemIds.Except(existingDocument.RelatedItemIds))
            {
                patchCommands.Add(new PatchCommandData
                {
                    Key = itemIdToAdd,
                    Patches = new[]
                    {
                        new PatchRequest
                        {
                            Type = PatchCommandType.Add,
                            Name = "RelatedSpeciesIds",
                            Value = newDocument.Id
                        }
                    }
                });
            }

            // Related Specimen update
            foreach (var specimenIdtoRemove in existingDocument.RelatedSpecimenIds.Except(newDocument.RelatedSpecimenIds))
            {
                patchCommands.Add(new PatchCommandData
                {
                    Key = specimenIdtoRemove,
                    Patches = new[]
                    {
                        new PatchRequest
                        {
                            Type = PatchCommandType.Remove,
                            AllPositions = true,
                            Name = "RelatedSpeciesIds",
                            Value = newDocument.Id
                        }
                    }
                });
            }
            foreach (var specimenIdtoAdd in newDocument.RelatedSpecimenIds.Except(existingDocument.RelatedSpecimenIds))
            {
                patchCommands.Add(new PatchCommandData
                {
                    Key = specimenIdtoAdd,
                    Patches = new[]
                    {
                        new PatchRequest
                        {
                            Type = PatchCommandType.Add,
                            Name = "RelatedSpeciesIds",
                            Value = newDocument.Id
                        }
                    }
                });
            }

            documentSession.Advanced.Defer(patchCommands.ToArray());

            // Map over existing document
            Mapper.Map(newDocument, existingDocument);
        }
    }
}