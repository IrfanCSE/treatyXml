using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TreatyXml.Helpers
{
    /// <summary>
    /// Helper class for text manipulation and normalization
    /// </summary>
    public static class TextHelper
    {
        private static readonly string[] Prepositions = new[] 
        { 
            "of", "in", "on", "at", "to", "for", "with", "from", "by", 
            "about", "as", "into", "through", "during", "before", "after", 
            "above", "below", "between", "under", "over" 
        };

        /// <summary>
        /// Normalizes text by applying proper capitalization rules
        /// </summary>
        public static string NormalizeText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            // Split text into words and capitalize appropriately
            var words = text.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                if (!string.IsNullOrEmpty(words[i]))
                {
                    var lowerWord = words[i].ToLower();
                    // Keep prepositions lowercase unless they're the first word
                    if (i > 0 && Array.Exists(Prepositions, p => p == lowerWord))
                    {
                        words[i] = lowerWord;
                    }
                    else
                    {
                        words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
                    }
                }
            }
            text = string.Join(" ", words);

            // Trim leading and trailing whitespace
            return text.Trim();
        }
    }
}
