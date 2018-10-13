using System.Collections;
using System.Collections.Generic;

namespace KokeiroLifeLogger.Utilities
{
    public static class StringExtensions
    {
        public static string TrimStarts(this string str, string trimString)
        {
            return System.Text.RegularExpressions.Regex.Replace(str, $"^{trimString}", "");
        }
        public static string TrimEnds(this string str, string trimString)
        {
            return System.Text.RegularExpressions.Regex.Replace(str, $"{trimString}$", "");
        }

        public static string JoinString(this IEnumerable<string> strings, string separator)
        {
            return string.Join(separator, strings);
        }
    }
}
