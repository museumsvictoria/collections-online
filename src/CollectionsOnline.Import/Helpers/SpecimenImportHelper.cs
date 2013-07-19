using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CollectionsOnline.Core.DomainModels;
using CollectionsOnline.Core.Factories;
using CollectionsOnline.Core.Extensions;
using IMu;

namespace CollectionsOnline.Import.Helpers
{
    public class SpecimenImportHelper : IImportHelper<Specimen>
    {
        public string MakeModuleName()
        {
            return "ecatalogue";
        }

        public string[] MakeColumns()
        {
            return new[]
                {
                    "irn",
                    "ColRegPrefix",
                    "ColRegNumber",
                    "ColRegPart",
                    "ColTypeOfItem",
                    "AdmDateModified",
                    "AdmTimeModified",
                    "ColDiscipline",
                    "colevent=ColCollectionEventRef.(ExpExpeditionName,ColCollectionEventCode,ColCollectionMethod,ColDateVisitedTo,ColTimeVisitedTo,AquDepthToMet,AquDepthFromMet,site=ColSiteRef.(SitSiteCode,SitSiteNumber,geo=[LocOcean_tab,LocContinent_tab,LocCountry_tab,LocProvinceStateTerritory_tab,LocDistrictCountyShire_tab,LocTownship_tab],LocPreciseLocation,LocElevationASLFromMt,LocElevationASLToMt,latlong=[LatCentroidLongitudeDec_tab,LatCentroidLatitudeDec_tab,LatDatum_tab,determinedBy=LatDeterminedByRef_tab.(SummaryData),LatDetDate0,LatLatLongDetermination_tab,LatDetSource_tab]),collectors=ColParticipantRef_tab.(SummaryData))",
                    "SpeNoSpecimens",
                    "SpeSex_tab",
                    "SpeStageAge_tab",
                    "preparations=[StrSpecimenNature_tab,StrSpecimenForm_tab,StrFixativeTreatment_tab,StrStorageMedium_tab]",
                    "ManPreviousNumbers_tab",
                    "DarYearCollected",
                    "DarMonthCollected",
                    "DarDayCollected",                                   
                    "site=SitSiteRef.(SitSiteCode,SitSiteNumber,geo=[LocOcean_tab,LocContinent_tab,LocCountry_tab,LocProvinceStateTerritory_tab,LocDistrictCountyShire_tab,LocTownship_tab],LocPreciseLocation,LocElevationASLFromMt,LocElevationASLToMt,latlong=[LatCentroidLongitudeDec_tab,LatCentroidLatitudeDec_tab,LatDatum_tab,determinedBy=LatDeterminedByRef_tab.(SummaryData),LatDetDate0,LatLatLongDetermination_tab,LatDetSource_tab])",
                    "identifications=[IdeTypeStatus_tab,IdeCurrentNameLocal_tab,identifiers=IdeIdentifiedByRef_nesttab.(SummaryData),IdeDateIdentified0,IdeAccuracyNotes_tab,IdeQualifier_tab,taxa=TaxTaxonomyRef_tab.(irn,ClaScientificName,ClaKingdom,ClaPhylum,ClaSubphylum,ClaSuperclass,ClaClass,ClaSubclass,ClaSuperorder,ClaOrder,ClaSuborder,ClaInfraorder,ClaSuperfamily,ClaFamily,ClaSubfamily,ClaTribe,ClaSubtribe,ClaGenus,ClaSubgenus,ClaSpecies,ClaSubspecies,ClaRank,AutAuthorString,ClaApplicableCode,comname=[ComName_tab,ComStatus_tab])]",
                    "media=MulMultiMediaRef_tab.(irn,MulTitle,MulDescription,MulCreator_tab,MdaDataSets_tab,credit=<erights:MulMultiMediaRef_tab>.(RigAcknowledgement,RigType),AdmPublishWebNoPassword)"
                };
        }

        public Terms MakeTerms()
        {
            var terms = new Terms();

            terms.Add("ColCategory", "Natural Sciences");
            terms.Add("MdaDataSets_tab", "Website - Atlas of Living Australia");
            terms.Add("AdmPublishWebNoPassword", "Yes");     

            return terms;
        }

        public Specimen MakeDocument(Map map)
        {
            var specimen = new Specimen(map.GetString("irn"));

            specimen.DateModified = DateTime.ParseExact(
                string.Format("{0} {1}", map.GetString("AdmDateModified"), map.GetString("AdmTimeModified")),
                "dd/MM/yyyy HH:mm",
                new CultureInfo("en-AU"));

            #region Record-level Terms

            //dcterms:type
            if (map.GetString("ColTypeOfItem") == "Specimen")
                specimen.DctermsType = "PhysicalObject";

            //dcterms:language
            specimen.DctermsLanguage = "en";

            //dcterms:rights
            specimen.DctermsRights = "Dataset licensed under Creative Commons Attribution (CC-BY) 3.0 Australian license. Use of data of individual specimen occurrences does not require attribution on a per record basis.";

            //dcterms:rightsHolder
            specimen.DctermsRightsHolder = "Museum Victoria";

            //institutionID
            specimen.InstitutionId = "NMV";

            //collectionID
            specimen.CollectionId = "urn:lsid:biocol.org:col:34978";

            //datasetID
            specimen.DatasetId = map.GetString("ColDiscipline");

            //institutionCode
            specimen.InstitutionCode = "NMV";

            //collectionCode
            specimen.CollectionCode = map.GetString("ColDiscipline");

            //ownerInstitutionCode
            specimen.OwnerInstitutionCode = "NMV";

            //datasetName
            var colevent = map.GetMap("colevent");
            if (colevent != null)
                specimen.DatasetName = colevent.GetString("ExpExpeditionName");

            //ownerInstitutionCode
            specimen.OwnerInstitutionCode = "NMV";

            //basisOfRecord
            if (map.GetString("ColTypeOfItem") == "Specimen")
                specimen.BasisOfRecord = "PreservedSpecimen";

            #endregion

            #region Occurrence

            //occurrenceID                                        
            specimen.OccurrenceId = (string.IsNullOrWhiteSpace(map.GetString("ColRegPart")))
                                      ? string.Format(
                                          "urn:lsid:ozcam.taxonomy.org.au:NMV:{0}:PreservedSpecimen:{1}{2}",
                                          map.GetString("ColDiscipline"), map.GetString("ColRegPrefix"),
                                          map.GetString("ColRegNumber"))
                                      : string.Format(
                                          "urn:lsid:ozcam.taxonomy.org.au:NMV:{0}:PreservedSpecimen:{1}{2}-{3}",
                                          map.GetString("ColDiscipline"), map.GetString("ColRegPrefix"),
                                          map.GetString("ColRegNumber"), map.GetString("ColRegPart"));

            //catalogNumber
            specimen.CatalogNumber = (string.IsNullOrWhiteSpace(map.GetString("ColRegPart")))
                                      ? string.Format("{0}{1}", map.GetString("ColRegPrefix"), map.GetString("ColRegNumber"))
                                      : string.Format("{0}{1}-{2}", map.GetString("ColRegPrefix"), map.GetString("ColRegNumber"), map.GetString("ColRegPart"));

            //recordedBy
            if (colevent != null && colevent.GetMaps("collectors") != null)
            {
                specimen.RecordedBy = colevent.GetMaps("collectors").Where(x => x != null).Select(x => x.GetString("SummaryData")).Concatenate(";");
            }

            //individualCount
            specimen.IndividualCount = map.GetString("SpeNoSpecimens");

            //sex
            var sex = map.GetStrings("SpeSex_tab");
            if (sex.Any())
            {
                specimen.Sex = sex.Concatenate(";");
            }

            //lifeStage
            var lifeStage = map.GetStrings("SpeStageAge_tab");
            if (lifeStage.Any())
            {
                specimen.LifeStage = lifeStage.Concatenate(";");
            }

            //occurrenceStatus
            specimen.OccurrenceStatus = "present";

            //preparations
            foreach (var preparationMap in map.GetMaps("preparations"))
            {
                var preparation = new[]
                            {
                                new KeyValuePair<string, string>("specimenNature", preparationMap.GetString("StrSpecimenNature_tab")),
                                new KeyValuePair<string, string>("specimenForm", preparationMap.GetString("StrSpecimenForm_tab")),
                                new KeyValuePair<string, string>("fixativeTreatment", preparationMap.GetString("StrFixativeTreatment_tab")),
                                new KeyValuePair<string, string>("storageMedium", preparationMap.GetString("StrStorageMedium_tab"))
                            }
                    .Where(x => !string.IsNullOrWhiteSpace(x.Value))
                    .Select(x => string.Format("{0}={1}", x.Key, x.Value))
                    .Concatenate(",");

                if (specimen.Preparations != null)
                {
                    specimen.Preparations += ";" + preparation;
                }
                else
                {
                    specimen.Preparations = preparation;
                }
            }

            //otherCatalogNumbers
            var otherCatalogNumbers = map.GetStrings("ManPreviousNumbers_tab");
            if (otherCatalogNumbers.Any())
            {
                specimen.OtherCatalogNumbers = otherCatalogNumbers.Concatenate(";");
            }

            //associatedMedia
            var media = new List<Media>();
            foreach (var mediaMap in map.GetMaps("media").Where(x => x.GetString("AdmPublishWebNoPassword") == "Yes" && x.GetStrings("MdaDataSets_tab").Contains("Website  Atlas of Living Australia")))
            {
                media.Add(new Media
                {
                    DateModified = DateTime.ParseExact(string.Format("{0} {1}", mediaMap.GetString("AdmDateModified"),
                                                        mediaMap.GetString("AdmTimeModified")),
                                                       "dd/MM/yyyy HH:mm", new CultureInfo("en-AU")),
                    Title = mediaMap.GetString("MulTitle"),
                    Type = mediaMap.GetString("MulMimeType")
                });
            }
            specimen.Media = media;
            specimen.AssociatedMedia = media.Select(x => x.Title).Concatenate(";");

            #endregion

            #region Event

            //eventID
            if (colevent != null)
                specimen.EventId = colevent.GetString("ColCollectionEventCode");

            //samplingProtocol
            if (colevent != null)
                specimen.SamplingProtocol = colevent.GetString("ColCollectionMethod");

            //eventDate
            if (colevent != null && !string.IsNullOrWhiteSpace(colevent.GetString("ColDateVisitedTo")))
            {
                DateTime eventDate;
                if (DateTime.TryParseExact(colevent.GetString("ColDateVisitedTo"), "dd/MM/yyyy", new CultureInfo("en-AU"), DateTimeStyles.None, out eventDate))
                    specimen.EventDate = eventDate.ToString("s");
            }

            //eventTime
            if (colevent != null)
                specimen.EventDate = colevent.GetString("ColTimeVisitedTo");

            //year
            specimen.Year = map.GetString("DarYearCollected");

            //month
            specimen.Month = map.GetString("DarMonthCollected");

            //day
            specimen.Day = map.GetString("DarDayCollected");

            //verbatimEventDate
            if (colevent != null)
                specimen.VerbatimEventDate = colevent.GetString("ColDateVisitedTo");

            //fieldNumber
            if (colevent != null)
                specimen.FieldNumber = colevent.GetString("ColCollectionEventCode");

            #endregion

            #region Location

            var site = map.GetMap("site");

            if (site == null && colevent != null)
                site = colevent.GetMap("site");

            //locationID
            //locality
            //verbatimLocality
            //minimumElevationInMeters
            //maximumElevationInMeters
            if (site != null)
            {
                if (!string.IsNullOrWhiteSpace(site.GetString("SitSiteCode")) || !string.IsNullOrWhiteSpace(site.GetString("SitSiteNumber")))
                    specimen.LocationID = string.Format("{0}{1}", site.GetString("SitSiteCode"), site.GetString("SitSiteNumber"));

                specimen.Locality = site.GetString("LocPreciseLocation");
                specimen.VerbatimLocality = site.GetString("LocPreciseLocation");
                specimen.MinimumElevationInMeters = site.GetString("LocElevationASLFromMt");
                specimen.MaximumElevationInMeters = site.GetString("LocElevationASLToMt");

                var geo = site.GetMaps("geo").FirstOrDefault();

                //higherGeography
                //continent
                //waterBody
                //country
                //stateProvince
                //county
                //municipality                        
                if (geo != null)
                {
                    specimen.HigherGeography = new[]
                                {
                                    geo.GetString("LocOcean_tab"),
                                    geo.GetString("LocContinent_tab"),
                                    geo.GetString("LocCountry_tab"),
                                    geo.GetString("LocProvinceStateTerritory_tab")
                                }.Concatenate(", ");

                    specimen.Continent = geo.GetString("LocContinent_tab");
                    specimen.WaterBody = geo.GetString("LocOcean_tab");
                    specimen.Country = geo.GetString("LocCountry_tab");
                    specimen.StateProvince = geo.GetString("LocProvinceStateTerritory_tab");
                    specimen.County = geo.GetString("LocDistrictCountyShire_tab");
                    specimen.Municipality = geo.GetString("LocTownship_tab");
                }

                var latlong = site.GetMaps("latlong").FirstOrDefault();

                //decimalLatitude
                //decimalLongitude
                //geodeticDatum
                //georeferencedBy
                //georeferencedDate
                //georeferenceProtocol
                //georeferenceSources
                if (latlong != null)
                {
                    specimen.DecimalLatitude = latlong.GetString("LatCentroidLatitudeDec_tab");
                    specimen.DecimalLongitude = latlong.GetString("LatCentroidLongitudeDec_tab");

                    specimen.GeodeticDatum = (string.IsNullOrWhiteSpace(latlong.GetString("LatDatum_tab"))) ? "WGS84" : latlong.GetString("LatDatum_tab");

                    var determinedBy = latlong.GetMap("determinedBy");
                    if (determinedBy != null)
                    {
                        specimen.GeoreferencedBy = determinedBy.GetString("SummaryData");
                    }

                    DateTime georeferencedDate;
                    if (DateTime.TryParseExact(latlong.GetString("LatDetDate0"), "dd/MM/yyyy", new CultureInfo("en-AU"), DateTimeStyles.None, out georeferencedDate))
                        specimen.GeoreferencedDate = georeferencedDate.ToString("s");

                    specimen.GeoreferenceProtocol = latlong.GetString("LatLatLongDetermination_tab");
                    specimen.GeoreferenceSources = latlong.GetString("LatDetSource_tab");
                }
            }

            //minimumDepthInMeters
            //maximumDepthInMeters
            if (colevent != null)
            {
                specimen.MinimumDepthInMeters = colevent.GetString("AquDepthFromMet");
                specimen.MaximumDepthInMeters = colevent.GetString("AquDepthToMet");
            }

            #endregion

            #region Identification

            var types = new[] { "holotype", "lectotype", "neotype", "paralectotype", "paratype", "syntype", "type" };
            var identification = map.GetMaps("identifications").FirstOrDefault(x => (x.GetString("IdeTypeStatus_tab") != null && types.Contains(x.GetString("IdeTypeStatus_tab").Trim().ToLower()))) ??
                                 map.GetMaps("identifications").FirstOrDefault(x => (x.GetString("IdeCurrentNameLocal_tab") != null && x.GetString("IdeCurrentNameLocal_tab").Trim().ToLower() == "yes"));

            if (identification != null)
            {
                //typeStatus
                specimen.TypeStatus = identification.GetString("IdeTypeStatus_tab");

                //identifiedBy
                var identifiers = identification.GetMaps("identifiers");
                if (identifiers != null && identifiers.Any())
                {
                    specimen.IdentifiedBy = identifiers.Where(x => x != null).Select(x => x.GetString("SummaryData")).Concatenate(";");
                }

                //dateIdentified
                //identificationRemarks
                //identificationQualifier
                specimen.DateIdentified = identification.GetString("IdeDateIdentified0");
                specimen.IdentificationRemarks = identification.GetString("IdeAccuracyNotes_tab");
                specimen.IdentificationQualifier = identification.GetString("IdeQualifier_tab");

                var taxonomy = identification.GetMap("taxa");
                if (taxonomy != null)
                {
                    //scientificName
                    //kingdom
                    //phylum
                    //class
                    //order
                    //family
                    //genus
                    //subgenus
                    //specificEpithet
                    //infraspecificEpithet
                    //taxonRank
                    //scientificNameAuthorship
                    //nomenclaturalCode
                    specimen.ScientificName = taxonomy.GetString("ClaScientificName");
                    specimen.Kingdom = taxonomy.GetString("ClaKingdom");
                    specimen.Phylum = taxonomy.GetString("ClaPhylum");
                    specimen.Class = taxonomy.GetString("ClaClass");
                    specimen.Order = taxonomy.GetString("ClaOrder");
                    specimen.Family = taxonomy.GetString("ClaFamily");
                    specimen.Genus = taxonomy.GetString("ClaGenus");
                    specimen.Subgenus = taxonomy.GetString("ClaSubgenus");
                    specimen.SpecificEpithet = taxonomy.GetString("ClaSpecies");
                    specimen.InfraspecificEpithet = taxonomy.GetString("ClaSubspecies");
                    specimen.TaxonRank = taxonomy.GetString("ClaRank");
                    specimen.ScientificNameAuthorship = taxonomy.GetString("AutAuthorString");
                    specimen.NomenclaturalCode = taxonomy.GetString("ClaApplicableCode");

                    //higherClassification
                    specimen.HigherClassification = new[]
                                {
                                    taxonomy.GetString("ClaKingdom"), 
                                    taxonomy.GetString("ClaPhylum"),
                                    taxonomy.GetString("ClaSubphylum"),
                                    taxonomy.GetString("ClaSuperclass"),
                                    taxonomy.GetString("ClaClass"),
                                    taxonomy.GetString("ClaSubclass"),
                                    taxonomy.GetString("ClaSuperorder"),
                                    taxonomy.GetString("ClaOrder"),
                                    taxonomy.GetString("ClaSuborder"),
                                    taxonomy.GetString("ClaInfraorder"),
                                    taxonomy.GetString("ClaSuperfamily"),
                                    taxonomy.GetString("ClaFamily"),
                                    taxonomy.GetString("ClaSubfamily"),
                                    taxonomy.GetString("ClaTribe"),
                                    taxonomy.GetString("ClaSubtribe"),
                                    taxonomy.GetString("ClaGenus"),
                                    taxonomy.GetString("ClaSubgenus"),
                                    taxonomy.GetString("ClaSpecies"),
                                    taxonomy.GetString("ClaSubspecies")
                                }.Concatenate(";");

                    //vernacularName
                    var vernacularName = taxonomy.GetMaps("comname").FirstOrDefault(x => x.GetString("ComStatus_tab") != null && x.GetString("ComStatus_tab").Trim().ToLower() == "preferred");
                    if (vernacularName != null)
                        specimen.VernacularName = vernacularName.GetString("ComName_tab");
                }
            }

            // acceptedNameUsage
            var acceptedNameIdentification = map.GetMaps("identifications").FirstOrDefault(x => x.GetString("IdeCurrentNameLocal_tab") != null && x.GetString("IdeCurrentNameLocal_tab").Trim().ToLower() == "yes");

            if (acceptedNameIdentification != null)
            {
                var taxonomy = acceptedNameIdentification.GetMap("taxa");

                if (taxonomy != null)
                {
                    specimen.AcceptedNameUsage = taxonomy.GetString("ClaScientificName");
                }
            }

            // originalNameUsage
            var originalNameIdentification = map.GetMaps("identifications").FirstOrDefault(x => (x.GetString("IdeTypeStatus_tab") != null && types.Contains(x.GetString("IdeTypeStatus_tab").Trim().ToLower())));

            if (originalNameIdentification != null)
            {
                var taxonomy = originalNameIdentification.GetMap("taxa");

                if (taxonomy != null)
                {
                    specimen.OriginalNameUsage = taxonomy.GetString("ClaScientificName");
                }
            }

            #endregion

            return specimen;
        }
    }
}