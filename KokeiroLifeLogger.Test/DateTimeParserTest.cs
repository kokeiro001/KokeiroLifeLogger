using System;
using KokeiroLifeLogger.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KokeiroLifeLogger.Test
{
    [TestClass]
    public class DateTimeParserTest
    {
        [DataTestMethod]
        [DataRow("March 02, 2017 at 08:18PM", "2017/03/02 20:18")]
        [DataRow("April 02, 2017 at 08:18PM", "2017/04/02 20:18")]
        [DataRow("May 02, 2017 at 08:18PM", "2017/05/02 20:18")]
        [DataRow("June 02, 2017 at 08:18PM", "2017/06/02 20:18")]
        public void DateTimeParser_ParseWithingsDateTest(string str, string expectedStr)
        {
            var date = DateTimeParser.ParseWithingsDate(str);
            var expected = DateTime.Parse(expectedStr);
            Assert.AreEqual(expected, date);
        }
    }
}
