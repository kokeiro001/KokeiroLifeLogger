using System;
using System.Globalization;

namespace KokeiroLifeLogger.Utilities
{
    public static class DateTimeParser
    {
        private readonly static string[] WithingsDateFormats = new string[]
        {
            "MMM d, yyyy 'at' hh:mmtt",
            "MMMM d, yyyy 'at' hh:mmtt",
            "MMMMM d, yyyy 'at' hh:mmtt",
            "MMMMMM d, yyyy 'at' hh:mmtt",
            "MMMMMMM d, yyyy 'at' hh:mmtt",
            "MMMMMMMM d, yyyy 'at' hh:mmtt",
            "MMMMMMMMM d, yyyy 'at' hh:mmtt",
        };

        public static DateTime ParseWithingsDate(string str)
        {

            return DateTime.ParseExact(str, WithingsDateFormats, new CultureInfo("en-US"), DateTimeStyles.None);
        }
    }
}
