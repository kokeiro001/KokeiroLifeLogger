using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
