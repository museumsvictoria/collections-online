using System;

namespace CollectionsOnline.Core.Extensions
{
    public static class LongExtensions
    {
        public static string BytesToString(this long input)
        {
            string[] unit = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
            if (input == 0)
                return "0" + unit[0];
            input = Math.Abs(input);
            int place = Convert.ToInt32(Math.Floor(Math.Log(input, 1024)));
            double num = Math.Round(input / Math.Pow(1024, place), 1);

            return (Math.Sign(input) * num) + unit[place];
        }
    }
}
