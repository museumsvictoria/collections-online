using System;

namespace CollectionsOnline.Core.Config
{
    public static class Constants
    {
        public static string ApplicationId = "applications/collectionsonline";

        public static int DataBatchSize = 50;

        public static int PagingPageSizeDefault = 30;

        public static int PagingPageSizeMax = 120;

        public static int SuggestionsMinResultsSize = 15;

        public static int SummaryMaxChars = 200;

        public static string ImuItemQueryString = "Collections Online - Humanities";

        public static string ImuSpeciesQueryString = "Website - Species profile";

        public static string ImuSpecimenQueryString = "Website  Atlas of Living Australia";

        public static string ImuStoryQueryString = "Website - History & Technology Collections";

        public static string ImuMultimediaQueryString = "Website - Collections Online";

        public static TimeSpan AggressiveCacheTimeSpan = TimeSpan.FromHours(1);
    }
}