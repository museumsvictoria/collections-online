﻿using System;
using System.Globalization;
using System.Linq;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Import.Extensions;
using IMu;

namespace CollectionsOnline.Import.Factories
{
    public class CollectionEventFactory : ICollectionEventFactory
    {
        private readonly IPartiesNameFactory _partiesNameFactory;

        public CollectionEventFactory(
            IPartiesNameFactory partiesNameFactory)
        {
            _partiesNameFactory = partiesNameFactory;
        }

        public CollectionEvent Make(Map map, string type, string registrationPrefix)
        {
            if (map != null)
            {
                var collectionEvent = new CollectionEvent
                {
                    Irn = long.Parse(map.GetEncodedString("irn")),
                    ExpeditionName = map.GetEncodedString("ExpExpeditionName"),
                    CollectionEventCode = map.GetEncodedString("ColCollectionEventCode"),
                    SamplingMethod = map.GetEncodedString("ColCollectionMethod"),
                    DepthTo = map.GetEncodedString("AquDepthToMet"),
                    DepthFrom = map.GetEncodedString("AquDepthFromMet")
                };
                
                if (!(string.Equals(type, "Observation", StringComparison.OrdinalIgnoreCase) && string.Equals(registrationPrefix, "ZI", StringComparison.OrdinalIgnoreCase)))
                {
                    collectionEvent.CollectedBy = map.GetMaps("collectors").Where(x => x != null)
                        .Select(x => _partiesNameFactory.Make(x)).Concatenate(", ");
                }
                
                if (DateTime.TryParseExact(map.GetEncodedString("ColDateVisitedFrom"), "dd/MM/yyyy", new CultureInfo("en-AU"), DateTimeStyles.AssumeLocal, out DateTime dateVisitedFrom))
                {
                    if (TimeSpan.TryParseExact(map.GetEncodedString("ColTimeVisitedFrom"), @"hh\:mm", new CultureInfo("en-AU"), out TimeSpan timeVisitedFrom))
                    {
                        dateVisitedFrom += timeVisitedFrom;
                    }

                    collectionEvent.DateVisitedFrom = dateVisitedFrom;
                }

                if (DateTime.TryParseExact(map.GetEncodedString("ColDateVisitedTo"), "dd/MM/yyyy", new CultureInfo("en-AU"), DateTimeStyles.AssumeLocal, out DateTime dateVisitedTo))
                {
                    if (TimeSpan.TryParseExact(map.GetEncodedString("ColTimeVisitedTo"), @"hh\:mm", new CultureInfo("en-AU"), out TimeSpan timeVisitedTo))
                    {
                        dateVisitedTo += timeVisitedTo;
                    }

                    collectionEvent.DateVisitedTo = dateVisitedTo;
                }

                return collectionEvent;
            }

            return null;
        }
    }
}