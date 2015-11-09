using System;
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

        public CollectionEvent Make(Map map)
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
                    DepthFrom = map.GetEncodedString("AquDepthFromMet"),
                    CollectedBy = map.GetMaps("collectors").Where(x => x != null).Select(x => _partiesNameFactory.Make(x)).Concatenate(", ")
                };

                DateTime dateVisitedFrom;
                if (DateTime.TryParseExact(map.GetEncodedString("ColDateVisitedFrom"), "dd/MM/yyyy", new CultureInfo("en-AU"), DateTimeStyles.AssumeLocal, out dateVisitedFrom))
                {
                    TimeSpan timeVisitedFrom;
                    if (TimeSpan.TryParseExact(map.GetEncodedString("ColTimeVisitedFrom"), @"hh\:mm", new CultureInfo("en-AU"), out timeVisitedFrom))
                    {
                        dateVisitedFrom += timeVisitedFrom;
                    }

                    collectionEvent.DateVisitedFrom = dateVisitedFrom;
                }

                DateTime dateVisitedTo;
                if (DateTime.TryParseExact(map.GetEncodedString("ColDateVisitedTo"), "dd/MM/yyyy", new CultureInfo("en-AU"), DateTimeStyles.AssumeLocal, out dateVisitedTo))
                {
                    TimeSpan timeVisitedTo;
                    if (TimeSpan.TryParseExact(map.GetEncodedString("ColTimeVisitedTo"), @"hh\:mm", new CultureInfo("en-AU"), out timeVisitedTo))
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