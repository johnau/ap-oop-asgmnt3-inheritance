using Catalyst;
using NaturalLanguageProcessor.Utility;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace NaturalLanguageProcessor;

/// <summary>
/// NM Tafe C# OOP Task Manager Assignment - Part 5
/// Handling Natural Language User input with Forgiving Format
/// - Implementation with Regex
/// </summary>
public class ForgivingFormatWithRegexProcessor : IForgivingFormatProcessor
{
    /// <summary>
    /// Regex to match a Natural Language Date
    /// </summary>
    /// <remarks>
    /// Regex matches 3 named groups: adj, day, prepadj (adjective, day, preposition and adjective)
    /// The first named group (adj) is optional: <br />
    /// <c>((?<adj>next|this|following))?</c> - _Optional_ adjective match to modify the specified day ('adj')
    /// <c>(\s)*</c> - 0 to many match on space characters
    /// The second named group (day) is required: <br />
    /// <c>(?<day>monday|tuesday|wednesday|thursday|friday|saturday|sunday|tomorrow)</c> - _Require_ day match ('day)
    /// <c>(\s)*</c> - 0 to many match on space characters
    /// The third named group (prepadj) is optional: <br />
    /// <c>((?<prepadj>this\sweek|next\sweek|after\snext|following))*</c> - _Optional_ preposition and adjective match to modify specified day ('prepadj')
    /// </remarks>
    public static string Date_Pattern = @"((?<adj>next|this|following))?(\s)*(?<day>monday|tuesday|wednesday|thursday|friday|saturday|sunday|tomorrow)(\s)*((?<prepadj>this\sweek|next\sweek|after\snext|following))*";

    /// <summary>
    /// Regex to match a Natural Language Time
    /// </summary>
    /// <remarks>
    /// Regex matches 3 named groups: mod, hour, meridiem.
    /// The first named group (mod) is optional: <br />
    /// <c>(?<mod>(half past|quarter past|quarter to))?</c> - _Optional_ proposition phrase match to modify specified time ('mod')
    /// <c>(\s)*</c> - 0 to many match on space characters
    /// The second named group (hour) is required: <br />
    /// <c>(?<hour>(?=\b0?[1-9]|1[0-2]\b)\d{1,2}|one|two|three|four|five|six|seven|eight|nine|ten|eleven|twelve|midday)</c> - _Required_ hour match ('hour')
    /// <c>(\s)*</c> - 0 to many match on space characters (discarded)
    /// <c>(\b?in the\s|this\s)?</c> - _Optional_ preposition component (discarded)
    /// The third named group (meridiem) is optional: <br />
    /// <c>(?<meridiem>(morning|afternoon|evening|AM|PM))?</c> - _Optional_ noun (preposition object) to modify specified time ('meridiem')
    /// </remarks>
    public static string Time_Pattern = @"(?<prep>(half past|quarter past|quarter to))?(\s)*(?<hour>(?=\b0?[1-9]|1[0-2]\b)\d{1,2}|one|two|three|four|five|six|seven|eight|nine|ten|eleven|twelve|midday)(\s)*(\b?in the\s|this\s)?(?<meridiem>(morning|afternoon|evening|AM|PM))?";

    private Func<DateTime> DateTimeNow;

    /// <summary>
    /// No args constructor
    /// </summary>
    public ForgivingFormatWithRegexProcessor() 
    { 
        DateTimeNow = () => DateTime.Now;
    }

    /// <summary>
    /// Testing controller with dependency on DateTime object to be used instead of DateTime.Now value
    /// </summary>
    /// <param name="overrideCurrentTimeValue"></param>
    internal ForgivingFormatWithRegexProcessor(DateTime overrideCurrentTimeValue)
    {
        DateTimeNow = () => overrideCurrentTimeValue;
    }

    #region implemented interface methods
    /// <summary>
    /// Process Natural Date - Assignment Part 5
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public DateTimeResult ProcessNaturalDate(string input)
    {
        Debug.WriteLine($"Processing Date from input: {input} (Based on Date: {DateTimeNow.Invoke()})");
        var _input = StringHelper.SanitizeInput(input);
        var match = Regex.Match(_input, Date_Pattern, RegexOptions.IgnoreCase);
        if (!match.Success)
            return DateTimeResult.No_Result; // Quick escape for no matches

        var detectedDayOfWeek = RegexHandleDayMatch(match.Groups["day"].Value);
        int daysUntilDetectedDay = DaysFromToday(detectedDayOfWeek);
        var resultDate = DateInXDays(daysUntilDetectedDay);
        resultDate = RegexHandleDateModifier(match.Groups["adj"].Value, match.Groups["prepadj"].Value, resultDate, daysUntilDetectedDay);

        return new DateTimeResult(
            Found: true, 
            ValueObject: resultDate, 
            Exact: true, 
            Text: input.Substring(match.Index, match.Length), 
            Start: match.Index, 
            End: match.Index + match.Length); // Exact always true if found, would need to change for handling unspecific dates (like 4th April)
    }

    /// <summary>
    /// Process Natural Time - Assignment Part 5
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public DateTimeResult ProcessNaturalTime(string input)
    {
        var _input = StringHelper.SanitizeInput(input);
        var match = Regex.Match(_input, Time_Pattern, RegexOptions.IgnoreCase);
        if (!match.Success)
            return DateTimeResult.No_Result; // Quick escape for no matches

        var hourMatchGroup = match.Groups["hour"];
        var merediemGroup = match.Groups["meridiem"];
        var prepositionGroup = match.Groups["prep"];

        int targetHour;
        var target = DateTime.MinValue;

        targetHour = RegexHandleTemporalExpression(hourMatchGroup.Value); // if no temporal expression ("midday", "tonight"), targetHour is -1
        target = DateTimeNow.Invoke();

        if (targetHour < 0)
        {
            target = RegexHandleTimeMatch(hourMatchGroup.Value, prepositionGroup.Value);
            target = RegexHandleTimeMeridiem(merediemGroup.Value, target);
        }
        else
        {
            var dateOnly = DateOnly.FromDateTime(target);
            var timeOnly = new TimeOnly(targetHour, 0);
            target = new DateTime(dateOnly, timeOnly);
        }

        return new DateTimeResult(true, target, merediemGroup.Success, match.Value, match.Index, match.Index + match.Length);
    }

    /// <summary>
    /// Process Natural Task - Assignment Part 5
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public TaskInformation ProcessNaturalTask(string input)
    {
        var _input = input;
        var resultDate = ProcessNaturalDate(input);
        if (resultDate.Found)
            _input = _input.Replace(resultDate.Text.Trim(), "");

        var resultTime = ProcessNaturalTime(_input);
        if (resultTime.Found)
            _input = _input.Replace(resultTime.Text.Trim(), "");

        _input = NaturalLanguageHelper.TrimPrepositionWords(_input);
        _input = StringHelper.TrimNonAlphanumeric(_input);

        var combinedResult = NaturalLanguageHelper.CombineDateAndTimeResults(resultDate, resultTime);

        if (combinedResult == null)
        {
            return new TaskInformation(_input, false, DateTime.MinValue);
        }

        return new TaskInformation(_input, true, combinedResult.Value);
    }

    #endregion
    /// <summary>
    /// Converts the matched string for "day" to a DayOfWeek value.
    /// </summary>
    /// <remarks>
    /// The method expects the string value parameter is a result of the Date_Pattern regex match
    /// </remarks>
    /// <param name="value"></param>
    /// <returns></returns>
    private DayOfWeek RegexHandleDayMatch(string value)
    {
        var day = value.Trim().ToTitleCase();
        if (day.Contains("tomorrow", StringComparison.OrdinalIgnoreCase))
        {
            var dateTimeNow = DateTimeNow.Invoke();
            return dateTimeNow.AddDays(1).DayOfWeek;
        }

        DayOfWeek dayOfWeek = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), day, true);
        return dayOfWeek;
    }

    /// <summary>
    /// Handles a Regex match component
    /// </summary>
    /// <param name="adjective"></param>
    /// <param name="prepositionAndAdjective"></param>
    /// <param name="resultDate"></param>
    /// <param name="daysUntil"></param>
    /// <returns></returns>
    private DateTime RegexHandleDateModifier(string? adjective, string? prepositionAndAdjective, DateTime resultDate, int daysUntil)
    {
        var dateTimeNow = DateTimeNow.Invoke();
        //DayOfWeek today = dateTimeNow.DayOfWeek;
        var modifier = "";
        if (!string.IsNullOrWhiteSpace(adjective))
            modifier = adjective.Trim();
        else if (!string.IsNullOrWhiteSpace(prepositionAndAdjective)) // ignore post-preposition/adjective if adjective was found
            modifier = prepositionAndAdjective.Trim();

        if (modifier.Contains("following", StringComparison.OrdinalIgnoreCase)
            || modifier.Contains("after next", StringComparison.CurrentCultureIgnoreCase))
        {
            if ((dateTimeNow.DayOfWeek == DayOfWeek.Saturday || dateTimeNow.DayOfWeek == DayOfWeek.Sunday) && daysUntil <= 5)
                resultDate = resultDate.AddDays(7);

            if (daysUntil == 0 || dateTimeNow.DayOfWeek == resultDate.DayOfWeek)
                return resultDate.AddDays(14);
            else
                return resultDate.AddDays(7);
        }
        
        if (modifier.Contains("next week", StringComparison.OrdinalIgnoreCase))
        {
            // Check to see if result date is next week, if not add 7 days
            var startOfWeek = dateTimeNow.AddDays(-(int)dateTimeNow.DayOfWeek);
            var endOfWeek = startOfWeek.AddDays(6);
            var isInCurrentWeek = resultDate.Date >= startOfWeek && resultDate.Date <= endOfWeek;
            if (isInCurrentWeek)
                return resultDate.AddDays(7);
        }
        
        if (modifier.Contains("next", StringComparison.OrdinalIgnoreCase))
        {
            // if the current date is a saturday or sunday (end of week), "next" refers to the following week
            if ((dateTimeNow.DayOfWeek == DayOfWeek.Saturday || dateTimeNow.DayOfWeek == DayOfWeek.Sunday) && daysUntil <= 5)
                resultDate = resultDate.AddDays(7);

            // if it's less than a week to the target, and we haven't crossed a weekend (still within same week), "next" refers to the following week.
            // ie. If it's Monday, and someone says "Next Thursday", they are likely not referring to the Thursday in 3 days, which would be referred to as "This Thursday"
            if (daysUntil < 7 && !CrossesEndOfWeek(daysUntil))
                resultDate = resultDate.AddDays(7);

            return resultDate;
        }

        return resultDate;
    }

    /// <summary>
    /// Handles a Regex match component
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private int RegexHandleTemporalExpression(string value)
    {
        if (value.Contains("midday", StringComparison.CurrentCultureIgnoreCase))
            return 12;
        else if (value.Contains("tonight", StringComparison.CurrentCultureIgnoreCase))
            return 18;
        else 
            return -1;
    }

    /// <summary>
    /// Handles a Regex match component
    /// </summary>
    /// <param name="value"></param>
    /// <param name="temporalPreposition"></param>
    /// <returns></returns>
    private DateTime RegexHandleTimeMatch(string value, string temporalPreposition)
    {
        var hourString = value.Trim();

        var targetHour = hourString.NaturalumberStringToInt();
        var targetMinute = RegexHandleTimePreposition(temporalPreposition);
        if (targetMinute < 0)
        {
            targetHour -= 1;
            targetMinute = 60 + targetMinute;
        }

        var targetDateTime = DateTimeHelper.ConvertHourToDateTime(targetHour);
        targetDateTime = targetDateTime.AddMinutes(targetMinute);

        return targetDateTime;
    }

    /// <summary>
    /// Handles a Regex match component
    /// </summary>
    /// <param name="merediem"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    private DateTime RegexHandleTimeMeridiem(string merediem, DateTime value)
    {
        if (merediem.Contains("AM", StringComparison.OrdinalIgnoreCase)
            || merediem.Contains("morning", StringComparison.OrdinalIgnoreCase))
        {
            // time should already default to the morning - do nothing atm
        }
        else if (merediem.Contains("PM", StringComparison.OrdinalIgnoreCase)
                || merediem.Contains("afternoon", StringComparison.OrdinalIgnoreCase)
                || merediem.Contains("evening", StringComparison.OrdinalIgnoreCase))
        {
            value = value.AddHours(12);
        }
        else
        {
            var dateTimeNow = DateTimeNow.Invoke();
            var targetHasPassed1 = dateTimeNow.Hour > value.Hour;
            var targetHasPassed2 = dateTimeNow.Hour == value.Hour && dateTimeNow.Minute > value.Minute;
            var targetIsNotToday1 = dateTimeNow.Hour > value.Hour + 12;
            var targetIsNotToday2 = dateTimeNow.Hour == value.Hour + 12 && dateTimeNow.Minute > value.Minute;
            
            if ((targetHasPassed1 || targetHasPassed2) && !(targetIsNotToday1 || targetIsNotToday2))
                value = value.AddHours(12);
        }

        return value;
    }

    /// <summary>
    /// Handles a Regex match component
    /// </summary>
    /// <param name="temporalPreposition"></param>
    /// <returns></returns>
    private int RegexHandleTimePreposition(string temporalPreposition)
    {
        if (string.IsNullOrWhiteSpace(temporalPreposition))
            return 0;

        if (temporalPreposition.Contains("half past", StringComparison.OrdinalIgnoreCase))
            return 30;
        else if (temporalPreposition.Contains("quarter past", StringComparison.OrdinalIgnoreCase))
            return 15;
        else if (temporalPreposition.Contains("quarter to", StringComparison.OrdinalIgnoreCase))
            return -15;
        else
            return 0;
    }

    /// <summary>
    /// Counts to the next occurence of the specified day
    /// </summary>
    /// <param name="dayOfWeek"></param>
    /// <returns></returns>
    private int DaysFromToday(DayOfWeek dayOfWeek)
    {
        var today = DateTimeNow.Invoke();
        var daysUntil = ((int)dayOfWeek - (int)today.DayOfWeek + 7) % 7; // positive day diff

        return daysUntil;
    }

    /// <summary>
    /// Returns the Date in the future for the given interval, from todays date.
    /// </summary>
    /// <remarks>
    /// Funny method name
    /// </remarks>
    /// <param name="days"></param>
    /// <returns></returns>
    private DateTime DateInXDays(int days)
    {
        var today = DateTimeNow.Invoke();
        var resultDate = today.AddDays(days);

        return resultDate;
    }

    /// <summary>
    /// Checks if the day after interval provided by param <c>numberOfDays</c> lapses a weekend.
    /// </summary>
    /// <param name="numberOfDays"></param>
    /// <returns></returns>
    private bool CrossesEndOfWeek(int numberOfDays)
    {
        var dateTime = DateTimeNow.Invoke();
        var endDate = dateTime.AddDays(numberOfDays);

        while (dateTime <= endDate)
        {
            if (dateTime.DayOfWeek == DayOfWeek.Saturday || dateTime.DayOfWeek == DayOfWeek.Sunday)
            {
                return true;
            }
            dateTime = dateTime.AddDays(1);
        }

        return false;
    }
}
