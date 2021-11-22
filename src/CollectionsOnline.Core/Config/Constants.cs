using System;
using System.Collections.Generic;
using CollectionsOnline.Core.Models;

namespace CollectionsOnline.Core.Config
{
    public static class Constants
    {
        public static string ApplicationId = "applications/collectionsonline";

        public static int DataBatchSize = 50;

        public static int DataLoadBatchSize = 20;

        public static int CachedDataBatchSize = 10000;

        public static int PagingPerPageDefault = 40;

        public static int PagingPerPageMax = 100;

        public static int SuggestionsMinResultsSize = 15;

        public static int SummaryMaxChars = 230;

        public static int MetadataDescriptionMaxChars = 400;

        public static int FileMaxChars = 160;

        public static int CollectionSummaryMaxChars = 120;

        public static int HomeMaxRecentResults = 12;

        public static int MaxSitemapUrls = 50000;

        public static int PagingStreamSize = 10000;

        public static string ImuItemQueryString = "Collections Online: Humanities";

        public static string ImuSpeciesQueryString = "Collections Online: Species Profile";

        public static string ImuSpecimenQueryString = "Collections Online: Natural Sciences";

        public static string ImuArticleQueryString = "Collections Online: Article";

        public static string ImuCollectionQueryString = "Collections Online: Collection Overview";

        public static string ImuMultimediaQueryString = "Collections Online: MMR";

        public static string ImuVideoQueryString = "Video URL";

        public static string ImuUriQueryString = "Webpage Link URL";

        public static TimeSpan AggressiveCacheTimeSpan = TimeSpan.FromHours(24);

        public static TimeSpan ImuOfflineTimeSpanStart = new TimeSpan(19, 00, 0); // 7:00pm
        
        public static TimeSpan ImuOfflineTimeSpanEnd = new TimeSpan(23, 00, 0); // 11:00pm        

        public static string ApiPathBase = "/api";
        
        public static Dictionary<LicenceType, Licence> Licences = new Dictionary<LicenceType, Licence>
        {
            {
                LicenceType.CcBy,
                new Licence { Type = LicenceType.CcBy, Name = "Attribution 4.0 International", ShortName = "CC BY", Uri = "https://creativecommons.org/licenses/by/4.0", Open = true, NonCommercialOpen = false }
            },
            {
                LicenceType.CcByNc,
                new Licence { Type = LicenceType.CcByNc, Name = "Attribution-NonCommercial 4.0 International", ShortName = "CC BY-NC", Uri = "https://creativecommons.org/licenses/by-nc/4.0/", Open = true, NonCommercialOpen = true }
            },
            {
                LicenceType.CcByNcNd,
                new Licence { Type = LicenceType.CcByNcNd, Name = "Attribution-NonCommercial-NoDerivatives 4.0 International", ShortName = "CC BY-NC-ND", Uri = "https://creativecommons.org/licenses/by-nc-nd/4.0/", Open = true, NonCommercialOpen = true }
            },
            {
                LicenceType.CcByNcSa,
                new Licence { Type = LicenceType.CcByNcSa, Name = "Attribution-NonCommercial-ShareAlike 4.0 International", ShortName = "CC BY-NC-SA", Uri = "https://creativecommons.org/licenses/by-nc-sa/4.0/", Open = true, NonCommercialOpen = true }
            },
            {
                LicenceType.CcByNd,
                new Licence { Type = LicenceType.CcByNd, Name = "Attribution-NoDerivatives 4.0 International", ShortName = "CC BY-ND", Uri = "https://creativecommons.org/licenses/by-nd/4.0/", Open = true, NonCommercialOpen = false }
            },
            {
                LicenceType.CcBySa,
                new Licence { Type = LicenceType.CcBySa, Name = "Attribution-ShareAlike 4.0 International", ShortName = "CC BY-SA", Uri = "https://creativecommons.org/licenses/by-sa/4.0/", Open = true, NonCommercialOpen = false }
            },
            {
                LicenceType.Cc0,
                new Licence { Type = LicenceType.Cc0, Name = "Public Domain Dedication", ShortName = "Public Domain", Uri = "https://creativecommons.org/publicdomain/zero/1.0/", Open = true, NonCommercialOpen = false }
            },
            {
                LicenceType.PublicDomainMark,
                new Licence { Type = LicenceType.PublicDomainMark, Name = "Public Domain Mark", ShortName = "Public Domain", Uri = "https://creativecommons.org/publicdomain/mark/1.0/", Open = true, NonCommercialOpen = false }
            },
            {
                LicenceType.AllRightsReserved,
                new Licence { Type = LicenceType.AllRightsReserved, Name = "All Rights Reserved", ShortName = "All Rights Reserved", Uri = string.Empty, Open = false, NonCommercialOpen = false }
            }
        };

        public static string[] TaxonomyTypeStatuses = { "holotype", "lectotype", "neotype", "paralectotype", "paratype", "syntype", "type" };
    }
}