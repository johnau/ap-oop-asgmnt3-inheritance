#if NET8_0_OR_GREATER

using NaturalLanguageProcessor.Utility;
using System.Diagnostics;
using NaturalLanguageProcessor.MicrosoftRecognizers;
using System;
using NaturalLanguageProcessor.Aggregates;

namespace NaturalLanguageProcessor;

/// <summary>
/// 
/// </summary>
public class ForgivingFormatWithRecognizersAndSpacyHandler : IForgivingFormatProcessor
{
    private readonly RecognizersWrapper recognizers;

    private Func<DateTime> DateTimeNow;
    private TimeOnly _defaultTime;

    /// <summary>
    /// Only Constructor
    /// </summary>
    public ForgivingFormatWithRecognizersAndSpacyHandler()
    {
        recognizers = new RecognizersWrapper();
        _defaultTime = new TimeOnly(12, 0);
    }

    #region implemented methods

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public DateTimeResult ProcessNaturalDate(string input)
    {
        input = StringHelper.SanitizeInput(input);

        if (TryDetectDate(input, out var date, out var strStart, out var strEnd))
        {
            var dateOnly = DateOnly.FromDateTime(date).ToDateTime(new TimeOnly(0, 0));

            return new DateTimeResult(true, dateOnly, false, input.Substring(strStart, strEnd - strStart), strStart, strEnd);
        }

        return DateTimeResult.No_Result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public DateTimeResult ProcessNaturalTime(string input)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Turns a Natural Language string input into components for a Task ("Description" and "Occurs At")
    /// </summary>
    /// <remarks>
    /// Uses the <see cref="ProcessNaturalDate(string)"/> and <see cref="ProcessNaturalTime(string)"/> methods.
    /// </remarks>
    /// <param name="input"></param>
    /// <returns></returns>
    public TaskInformation ProcessNaturalTask(string input)
    {
        var _task = new TaskInformation(input, false, DateTime.MinValue);
        
        var dateResult = ProcessNaturalDate(input);
        var timeResult = ProcessNaturalTime(input);

        var occursAt = CombineDateAndTimeResults(dateResult, timeResult);
        if (occursAt == null)
            return _task;

        var _inputFiltered = input;
        _task = new TaskInformation(_inputFiltered, true, occursAt.Value);
        return _task;
    }

    #endregion

    /// <summary>
    /// Tries to detect a date, first by DateTime 
    /// </summary>
    /// <param name="input"></param>
    /// <param name="value"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    /// <exception cref="TooManyValuesFoundException"></exception>
    private bool TryDetectDate(string input, out DateTime value, out int start, out int end)
    {
        start = -1;
        end = -1;
        value = DateTime.MinValue;

        // Try to directly detect date and time strings
        var ddt = recognizers.ParseWithDateTimeRecognizer(input);

        if (ddt == null)
            return false;

        Debug.WriteLine($"Detected a DateTime with Recognizers.DateTime: {ddt}");

        var dateResult = DateOnly.FromDateTime(ddt.Value);
        //var dayOfWeekResult = dateResult.DayOfWeek;
        //DayOfWeek todaysDay = DateTime.Now.DayOfWeek;

        if (dateResult < DateOnly.FromDateTime(DateTime.Now))
        {
            dateResult = dateResult.AddDays(7);
        }

        value = dateResult.ToDateTime(new TimeOnly(12, 0));
        start = ddt.Start;
        end = ddt.End;

        return true;
    }

    private bool TryDetectTime(string input, out TimeOnly? time, out string modifiedString)
    {
        time = null;
        modifiedString = input;


        var detectedAges = recognizers.ParseWithAgeRecognizer(input);
        if (detectedAges.Count == 1)
        {
            var now = DateTime.Now;
            var nowDate = DateOnly.FromDateTime(now);

            var da = detectedAges[0];
            var qty = da.Value;
            var period = da.Period.ToLower();

            DateTime? _dateTime = null;
            switch (period)
            {
                case "second":
                    _dateTime = now.AddSeconds(qty);
                    break;
                case "minute":
                    _dateTime = now.AddMinutes(qty);
                    break;
                case "hour":
                    _dateTime = now.AddHours(qty);
                    break;
                case "day":
                    _dateTime = now.AddDays(qty);
                    break;
                case "week":
                    _dateTime = now.AddDays(7 * qty);
                    break;
                case "month":
                    _dateTime = now.AddMonths(((int)qty));
                    break;
                case "year":
                    _dateTime = now.AddYears(((int)qty));
                    break;
                default:
                    Debug.WriteLine("Unsupported interval: " + period);
                    break;
            }

            if (_dateTime != null)
            {



                return true;
            }
        }
        else if (detectedAges.Count > 1)
        {
            // store the values fin results[] to expose to user...
            foreach (var da in detectedAges)
            {

            }
            // throw an exception if the DateTime detector found multiples, since this is likely a user input error
            //throw new TooManyValuesFoundException("Too many Dates detected: " + string.Join(", ", results));
        }
        return false;
    }

    /// <summary>
    /// Combines the results from the Natural Language processing of Date and Time
    /// TODO: Add support for custom datetime object to use in place of DateTime.Now, and then change how date & time found is handled since we want to bump forwards in some cases
    /// </summary>
    /// <param name="date"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    public DateTime? CombineDateAndTimeResults(DateTimeResult date, DateTimeResult time)
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
            return dd.ToDateTime(_defaultTime);  // Set at Midday for the desired date
        }
        else if (time.Found)
        {
            var now = DateTimeNow.Invoke();
            var nowTime = TimeOnly.FromDateTime(now);
            var nowDate = DateOnly.FromDateTime(now);
            var dt = TimeOnly.FromDateTime(time.ValueObject);

            var _dateTime = nowDate.ToDateTime(dt);
            // If the time is past current time and is AM (ie 5am), assume the person meant 5pm unless results is "Exact" meaning AM/PM was specificed
            //if (dt > nowTime && dt < timeMidday && !time.Exact)
            if (dt > nowTime && !time.Exact)
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

}

#endif