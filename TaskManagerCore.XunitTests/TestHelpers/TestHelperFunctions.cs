using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace TaskManagerCore.XunitTests.TestHelpers
{
    internal static class TestHelperFunctions
    {
        /// <summary>
        /// Randomized string of phonetic alphabet
        /// </summary>
        /// <param name="minWords"></param>
        /// <param name="maxWords"></param>
        /// <returns></returns>
        public static string RandomStringOfWords(int minWords = 5, int maxWords = 10)
        {
            var random = new Random();
            var count = random.Next(minWords, maxWords + 1);
            var words = new List<string> { "Alpha", "Bravo", "Charlie", "Delta", "Echo", "Foxtrot", "Golf", "Hotel", "India", "Juliett", "Kilo", "Lima", "Mike", "November", "Oscar", "Papa", "Quebec", "Romeo", "Sierra", "Tango", "Uniform", "Victor", "Whiskey", "X-ray", "Yankee", "Zulu" };

            var sb = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                sb.Append(words[random.Next(words.Count)]);
                sb.Append(' ');
            }

            // Shorten the string with StringBuilder.Length setter
            if (sb.Length > 0) sb.Length--;

            return sb.ToString();
        }

        /// <summary>
        /// Randomized string, alphanumeric by default
        /// </summary>
        /// <param name="minLength"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static string RandomString(int minLength = 10, int maxLength = 30, bool alphaOnly = false)
        {
            Random random = new Random();
            string characters = "abcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZ 0123456789";
            if (alphaOnly) characters.Substring(0, characters.Length - 10); // C#8.0+ characters[..^10];

            int length = random.Next(minLength, maxLength);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                int index = random.Next(characters.Length);
                sb.Append(characters[index]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Get a random boolean value
        /// </summary>
        /// <returns></returns>
        public static bool RandomBool()
        {
            Random random = new Random();
            return random.Next(2) == 0;
        }

        /// <summary>
        /// Get the most common first word from a list of strings
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public static (string name, int count) MostCommonStartingWords(List<string> names, int wordCount = 1)
        {
            Dictionary<string, int> nameOccurences = new Dictionary<string, int>();
            foreach (var n in names)
            {
                string[] words = n.Split(' ');
                string startOfName = words[0];
                for (int i = 1; i <= wordCount; i++)
                {
                    startOfName += $" {words[i]}";
                }

                if (nameOccurences.TryGetValue(startOfName, out _))
                    nameOccurences[startOfName]++;
                else
                    nameOccurences.Add(startOfName, 1);
            }

            var mostCommon = nameOccurences.OrderByDescending(pair => pair.Value)
                                            .FirstOrDefault();

            return (mostCommon.Key, mostCommon.Value);
        }

        /// <summary>
        /// Test cleanup (delete binary file)
        /// </summary>
        /// <param name="filename"></param>
        public static void CleanupAfterTest(string filename)
        {
            File.Delete(filename);
        }

        /// <summary>
        /// Run a test multiple times, used for randomized tests.
        /// </summary>
        /// <param name="test"></param>
        public static void RunTestMultipleTimes(Action test, int testIterations = 10)
        {
            // Run test multiple times
            for (int i = 0; i < testIterations; i++)
            {
                Debug.WriteLine($"Running Test Iteration: #{i + 1}");
                test();
            }
        }
    }
}
