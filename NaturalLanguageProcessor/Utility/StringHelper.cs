using System.Text.RegularExpressions;

namespace NaturalLanguageProcessor.Utility;

internal class StringHelper
{
    public static string SanitizeInput(string value)
    {
        value = value.Trim();
        value = value.Replace("  ", " "); // ensure all spaces are single
        //Regex.Replace(value, @"\s+", " ");

        return value;
    }

    public static string[] SplitInput(string input)
    {
        return input.Split(' ');
    }


    /// <summary>
    /// Removes leading and trailing commas, semi-colons, and other characters that 
    /// should not belong at the end of a 'sentence'
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string TrimNonAlphanumeric(string input)
    {
        char[] charsToTrim = [',', ';', '(', ')', '[', ']', '{', '}', ':', ' '];

        int startIndex = 0;
        while (startIndex < input.Length && Array.IndexOf(charsToTrim, input[startIndex]) != -1)
        {
            startIndex++;
        }

        int endIndex = input.Length - 1;
        while (endIndex >= 0 && Array.IndexOf(charsToTrim, input[endIndex]) != -1)
        {
            endIndex--;
        }

        return input.Substring(startIndex, endIndex - startIndex + 1).Trim();
    }
}
