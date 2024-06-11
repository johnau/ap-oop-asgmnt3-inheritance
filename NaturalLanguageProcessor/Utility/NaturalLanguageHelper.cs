using NaturalLanguageProcessor.Aggregates;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace NaturalLanguageProcessor.Utility
{

    /// <summary>
    /// 
    /// </summary>
    public partial class NaturalLanguageHelper
    {
//#if NET8_0
//        [GeneratedRegex(@"^\d+$")]
//        private static partial Regex DigitsOnlyRegex();
//#else
//        private static readonly Regex _digitsOnlyRegex = new Regex(@"^\d+$", RegexOptions.Compiled);

//        private static Regex DigitsOnlyRegex()
//        {
//            return _digitsOnlyRegex;
//        }
//#endif

        //private static Dictionary<string, int> NumberTable = new Dictionary<string, int>{
        //    {"zero", 0 },
        //    {"one", 1 },
        //    {"two", 2 },
        //    {"three", 3 },
        //    {"four", 4 },
        //    {"five", 5 },
        //    {"six", 6 },
        //    {"seven", 7 },
        //    {"eight", 8 },
        //    {"nine", 9 },
        //    {"ten", 10 },
        //    {"eleven", 11 },
        //    {"twelve", 12 },
        //    {"midday", 12 },
        //    {"noon", 12 },
        //};

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="numberAsString"></param>
        ///// <returns></returns>
        //[Obsolete]
        //public static int NaturalumberStringToInt(string numberAsString)
        //{
        //    int number = 0;
        //    if (DigitsOnlyRegex().Match(numberAsString).Success)
        //    {
        //        number = int.Parse(numberAsString);
        //    }
        //    else
        //    {
        //        if (NumberTable.TryGetValue(numberAsString.ToLower(), out var value))
        //        {
        //            number = value;
        //        }
        //    }

        //    return number;
        //}

        /// <summary>
        /// Removes preposition words from start and end of string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string TrimPrepositionWords(string input)
        {
            string pattern = @"\b(a|in|at|on|by|of)\b"; // Preposition words match

            // split input into words
            input = StringHelper.SanitizeInput(input);
            var words = input.Split(' ');

            int firstRealWord = 0;
            for (int i = 0; i < words.Length; i++)
            {
                if (!Regex.Match(words[i], pattern, RegexOptions.IgnoreCase).Success)
                    break; // as soon as we hit a word that is not a language preposition, bail

                firstRealWord = i;
            }

            int lastRealWord = words.Length - 1;
            for (int i = lastRealWord; i >= 0; i--)
            {
                if (!Regex.Match(words[i], pattern, RegexOptions.IgnoreCase).Success)
                    break; // as soon as we hit a word that is not a language preposition, bail

                if (i == 0)
                    break;

                lastRealWord = i - 1;
            }

            StringBuilder sb = new StringBuilder();
            for (int i = firstRealWord; i <= lastRealWord; i++)
            {
                sb.Append(words[i]);
                sb.Append(' ');
            }

            return sb.ToString().Trim();
        }

    }
}