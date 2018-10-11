using System.Diagnostics;
using System.Text.RegularExpressions;

namespace KokeiroLifeLogger.Utilities
{
    public static class StringUtility
    {
        public static string RemoveHttp(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }
            var result = input;
            var matches = Regex.Matches(input, @"http[^\s]* ", RegexOptions.Compiled);
            foreach (Match match in matches)
            {
                result = result.Replace(match.Value, "");
            }

            var matches2 = Regex.Matches(result, @"http.*", RegexOptions.Compiled);
            foreach (Match match in matches2)
            {
                result = result.Replace(match.Value, "");
            }
            return result.TrimEnd();
        }
    }
}
