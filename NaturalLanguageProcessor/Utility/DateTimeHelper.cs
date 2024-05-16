namespace NaturalLanguageProcessor.Utility;

internal class DateTimeHelper
{
    internal static DateTime ConvertHourToDateTime(int targetHour)
    {
        DateTime now = DateTime.Now;
        // hours till next occurence of target hour
        int hoursUntil = (targetHour - now.Hour + 24) % 24;
        var targetDateTime = now.AddHours(hoursUntil);
        // clean up minutes, seconds, etc
        var dateOnly = DateOnly.FromDateTime(targetDateTime);
        var timeOnly = new TimeOnly((targetDateTime.Hour), 0);
        // create the clean DateTime object
        targetDateTime = dateOnly.ToDateTime(timeOnly);

        return targetDateTime;
    }
}
