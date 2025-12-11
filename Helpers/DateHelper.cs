using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TreatyXml.Helpers
{
    /// <summary>
    /// Helper class for date parsing and conversion
    /// </summary>
    public static class DateHelper
    {
        private static readonly Dictionary<string, string> MonthMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "January", "Jan" },
            { "February", "Feb" },
            { "March", "Mar" },
            { "April", "Apr" },
            { "May", "May" },
            { "June", "Jun" },
            { "July", "Jul" },
            { "August", "Aug" },
            { "September", "Sep" },
            { "October", "Oct" },
            { "November", "Nov" },
            { "December", "Dec" }
        };

        /// <summary>
        /// Parses a date string and returns day, month abbreviation, and year
        /// </summary>
        public static (int Day, string Month, int Year) ParseDate(string dateStr)
        {
            var match = Regex.Match(dateStr, @"(\d+)\s+(\w+)\s+(\d{4})");

            if (match.Success)
            {
                return (
                    int.Parse(match.Groups[1].Value),
                    ConvertToShortMonth(match.Groups[2].Value),
                    int.Parse(match.Groups[3].Value)
                );
            }

            return (1, "Jan", 2000); // Default
        }

        /// <summary>
        /// Converts full month names to 3-letter abbreviations
        /// </summary>
        public static string ConvertToShortMonth(string month)
        {
            // If already short form or found in map, return appropriate value
            if (month.Length == 3)
                return month;
            
            return MonthMap.TryGetValue(month, out var shortMonth) ? shortMonth : month;
        }
    }
}
