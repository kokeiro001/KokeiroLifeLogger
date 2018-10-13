using System;
using KokeiroLifeLogger.Utilities;
using Xunit;

namespace KokeiroLifeLogger.Test
{
    public class DateTimeParserTest
    {
        [Theory]
        [InlineData("March 02, 2017 at 08:18PM", "2017/03/02 20:18")]
        [InlineData("April 02, 2017 at 08:18PM", "2017/04/02 20:18")]
        [InlineData("May 02, 2017 at 08:18PM", "2017/05/02 20:18")]
        [InlineData("June 02, 2017 at 08:18PM", "2017/06/02 20:18")]
        public void DateTimeParser_ParseWithingsDateTest(string str, string expectedStr)
        {
            var date = DateTimeParser.ParseWithingsDate(str);
            var expected = DateTime.Parse(expectedStr);
            date.Is(expected);
        }
    }
}
