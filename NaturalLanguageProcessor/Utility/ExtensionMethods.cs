using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NaturalLanguageProcessor.Utility
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class ExtensionMethods
    {
        #if NET8_0
            [GeneratedRegex(@"^\d+$")]
            private static partial Regex DigitsOnlyRegex();
        #else
            private static readonly Regex _digitsOnlyRegex = new Regex(@"^\d+$", RegexOptions.Compiled);

            private static Regex DigitsOnlyRegex()
            {
                return _digitsOnlyRegex;
            }
        #endif

        private static Dictionary<string, int> numberTable = new Dictionary<string, int>()
        {
            {"zero", 0 },
            {"one", 1 },
            {"two", 2 },
            {"three", 3 },
            {"four", 4 },
            {"five", 5 },
            {"six", 6 },
            {"seven", 7 },
            {"eight", 8 },
            {"nine", 9 },
            {"ten", 10 },
            {"eleven", 11 },
            {"twelve", 12 },
            {"midday", 12 },
            {"noon", 12 },

            {"thirteen", 13 },
            {"fourteen", 14 },
            {"fifteen", 15 },
            {"sixteen", 16 },
            {"seventeen", 17 },
            {"eighteen", 18 },
            {"nineteen", 19 },
        };

        private static Dictionary<string, int> bigNumberTable = new Dictionary<string, int>()
        {
            {"twenty", 20 },
            {"thirty", 30 },
            {"forty", 40 },
            {"fifty", 50 },
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="numberAsString"></param>
        /// <returns></returns>
        public static int NaturalNumberStringToInt(this string numberAsString)
        {
            numberAsString = numberAsString.Trim();
            int number = 0;
            if (DigitsOnlyRegex().Match(numberAsString).Success)
            {
                number = int.Parse(numberAsString);
            }
            else
            {
                if (bigNumberTable.TryGetValue(numberAsString.ToLower(), out var value))
                {
                    number = value;
                }
                else if (numberTable.TryGetValue(numberAsString.ToLower(), out value))
                {
                    number = value;
                } 
                else if (numberAsString.Contains("-"))
                {
                    var parts = numberAsString.Split('-');
                    if (parts.Length == 2 &&
                         bigNumberTable.TryGetValue(parts[0].Trim().ToLower(), out var bigPart) &&
                         numberTable.TryGetValue(parts[1].Trim().ToLower(), out var smallPart))
                    {
                        number = bigPart + smallPart;
                    }
                }
            }

            return number;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToTitleCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var words = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Length > 0)
                {
                    char[] letters = words[i].ToCharArray();
                    letters[0] = char.ToUpper(letters[0]);
                    words[i] = new string(letters);
                }
            }

            return string.Join(" ", words);
        }
    }
}