using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KokeiroLifeLogger.Common
{
    public static class DateTimeParser
    {
        public static DateTime ParseWithingsDate(string str) => 
            DateTime.ParseExact(str, "MMMM dd, yyyy 'at' hh:mmtt", new System.Globalization.CultureInfo("en-US"));
    }
}
