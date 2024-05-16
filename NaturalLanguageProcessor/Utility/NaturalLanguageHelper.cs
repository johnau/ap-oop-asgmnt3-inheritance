using System.Text;
using System.Text.RegularExpressions;

namespace NaturalLanguageProcessor.Utility;

public partial class NaturalLanguageHelper
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

    /// <summary>
    /// Combines the results from the Natural Language processing of Date and Time
    /// </summary>
    /// <param name="date"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    public static DateTime? CombineDateAndTimeResults(DateTimeResult date, DateTimeResult time)
    {
        var timeMidday = new TimeOnly(12, 0);
        if (date.Found && time.Found)
        {
            var dd = DateOnly.FromDateTime(date.ValueObject);
            var dt = TimeOnly.FromDateTime(time.ValueObject);
            return dd.ToDateTime(dt); // Combine the date and time results
        }
        else if (date.Found)
        {
            var dd = DateOnly.FromDateTime(date.ValueObject);
            return dd.ToDateTime(timeMidday);  // Set at Midday for the desired date
        }
        else if (time.Found)
        {
            var now = DateTime.Now;
            var nowTime = TimeOnly.FromDateTime(now);
            var nowDate = DateOnly.FromDateTime(now);
            var dt = TimeOnly.FromDateTime(time.ValueObject);
            

            var _dateTime = nowDate.ToDateTime(dt);
            // If the time is past current time and is AM (ie 5am), assume the person meant 5pm unless results is "Exact" meaning AM/PM was specificed
            if (dt > nowTime && dt < timeMidday && !time.Exact)
            {
                _dateTime = _dateTime.AddHours(12);
            }
            // If the time is past current time and is PM (ie 5pm), assume the person meant 5pm the next day
            else if (dt > nowTime && dt > timeMidday)
            {
                _dateTime = _dateTime.AddHours(24);
            }

            return _dateTime;
        }
        else // found neither
        {
            return null;
        }
    }

    public static int NaturalumberStringToInt(string numberAsString)
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
