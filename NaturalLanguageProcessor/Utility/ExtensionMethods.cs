using System.Text.RegularExpressions;

namespace NaturalLanguageProcessor.Utility;

public static partial class ExtensionMethods
{
    [GeneratedRegex(@"^\d+$")]
    private static partial Regex DigitsOnlyRegex();

    private static Dictionary<string, int> numberTable = new Dictionary<string, int>{
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

        //{"thirteen", 13 },
        //{"fourteen", 14 },
        //{"fifteen", 15 },
        //{"sixteen", 16 },
        //{"seventeen", 17 },
        //{"eighteen", 18 },
        //{"nineteen", 19 },
        //{"twenty", 20 },
        //{"twenty-one", 21 },
        //{"twenty-two", 22 },
        //{"twenty-three", 23 },
        //{"twenty-four", 24 },
    };

    public static int NaturalumberStringToInt(this string numberAsString)
    {
        int number = 0;
        if (DigitsOnlyRegex().Match(numberAsString).Success)
        {
            number = int.Parse(numberAsString);
        }
        else
        {
            if (numberTable.TryGetValue(numberAsString.ToLower(), out var value))
            {
                number = value;
            }
        }

        return number;
    }
}
