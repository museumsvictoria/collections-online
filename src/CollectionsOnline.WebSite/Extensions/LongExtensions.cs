using System;

namespace CollectionsOnline.WebSite.Extensions
{
    public static class LongExtensions
    {
        public static String BytesToString(this long bytes)
        {
            string[] unit = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
            if (bytes == 0)
                return "0" + unit[0];
            bytes = Math.Abs(bytes);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);

            return (Math.Sign(bytes) * num) + unit[place];
        }
    }
}