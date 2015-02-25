﻿using System;
using System.Collections.Generic;
using CollectionsOnline.Core.Models;

namespace CollectionsOnline.WebSite.Models.Api
{
    public class SpecimenApiViewModel : AggregateRoot
    {
        public DateTime DateModified { get; set; }

        public string Category { get; set; }

        public string ScientificGroup { get; set; }

        public string Discipline { get; set; }

        public string RegistrationNumber { get; set; }

        public IList<string> CollectionNames { get; set; }

        public string Type { get; set; }

        public string PrimaryClassification { get; set; }

        public string SecondaryClassification { get; set; }

        public string TertiaryClassification { get; set; }

        public string ObjectName { get; set; }

        public string ObjectSummary { get; set; }

        public string IsdDescriptionOfContent { get; set; }

        public string Significance { get; set; }

        public IList<string> Keywords { get; set; }

        public IList<string> CollectionPlans { get; set; }

        public IList<Association> Associations { get; set; }

        public IList<string> RelatedItemIds { get; set; }

        public IList<string> RelatedSpecimenIds { get; set; }

        public IList<string> RelatedArticleIds { get; set; }

        public IList<string> RelatedSpeciesIds { get; set; }

        public string AcquisitionInformation { get; set; }

        public string Acknowledgement { get; set; }

        public MuseumLocation MuseumLocation { get; set; }

        public IList<Media> Media { get; set; }

        public string NumberOfSpecimens { get; set; }

        public string ClutchSize { get; set; }

        public string Sex { get; set; }

        public string StageOrAge { get; set; }

        public IList<Storage> Storages { get; set; }

        public string TypeStatus { get; set; }

        public string IdentifiedBy { get; set; }

        public string DateIdentified { get; set; }

        public string Qualifier { get; set; }

        public QualifierRankType QualifierRank { get; set; }

        public Taxonomy Taxonomy { get; set; }

        public string ScientificName { get; set; }

        public string ExpeditionName { get; set; }

        public string CollectionEventCode { get; set; }

        public string SamplingMethod { get; set; }

        public DateTime? DateVisitedFrom { get; set; }

        public DateTime? DateVisitedTo { get; set; }

        public string DepthTo { get; set; }

        public string DepthFrom { get; set; }

        public string CollectedBy { get; set; }

        public string SiteCode { get; set; }

        public string Ocean { get; set; }

        public string Continent { get; set; }

        public string Country { get; set; }

        public string State { get; set; }

        public string District { get; set; }

        public string Town { get; set; }

        public string NearestNamedPlace { get; set; }

        public string PreciseLocation { get; set; }

        public string MinimumElevation { get; set; }

        public string MaximumElevation { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string GeodeticDatum { get; set; }

        public string SiteRadius { get; set; }

        public string GeoreferenceBy { get; set; }

        public string GeoreferenceDate { get; set; }

        public string GeoreferenceProtocol { get; set; }

        public string GeoreferenceSource { get; set; }

        public string GeologyEra { get; set; }

        public string GeologyPeriod { get; set; }

        public string GeologyEpoch { get; set; }

        public string GeologyStage { get; set; }

        public string GeologyGroup { get; set; }

        public string GeologyFormation { get; set; }

        public string GeologyMember { get; set; }

        public string GeologyRockType { get; set; }

        public string PalaeontologyDateCollectedFrom { get; set; }

        public string PalaeontologyDateCollectedTo { get; set; }

        public string MineralogySpecies { get; set; }

        public string MineralogyVariety { get; set; }

        public string MineralogyGroup { get; set; }

        public string MineralogyClass { get; set; }

        public string MineralogyAssociatedMatrix { get; set; }

        public string MineralogyType { get; set; }

        public string MineralogyTypeOfType { get; set; }

        public string MeteoritesName { get; set; }

        public string MeteoritesClass { get; set; }

        public string MeteoritesGroup { get; set; }

        public string MeteoritesType { get; set; }

        public string MeteoritesMinerals { get; set; }

        public string MeteoritesSpecimenWeight { get; set; }

        public string MeteoritesTotalWeight { get; set; }

        public string MeteoritesDateFell { get; set; }

        public string MeteoritesDateFound { get; set; }

        public string TektitesName { get; set; }

        public string TektitesClassification { get; set; }

        public string TektitesShape { get; set; }

        public string TektitesLocalStrewnfield { get; set; }

        public string TektitesGlobalStrewnfield { get; set; }

        public string PetrologyRockClass { get; set; }

        public string PetrologyRockGroup { get; set; }

        public string PetrologyRockName { get; set; }

        public string PetrologyRockDescription { get; set; }
    }
}