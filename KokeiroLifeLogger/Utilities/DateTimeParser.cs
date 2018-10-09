﻿using System;

namespace KokeiroLifeLogger.Utilities
{
    public static class DateTimeParser
    {
        public static DateTime ParseWithingsDate(string str) => 
            DateTime.ParseExact(str, "MMMM dd, yyyy 'at' hh:mmtt", new System.Globalization.CultureInfo("en-US"));
    }
}