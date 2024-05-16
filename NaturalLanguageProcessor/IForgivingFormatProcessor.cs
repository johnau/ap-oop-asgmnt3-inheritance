namespace NaturalLanguageProcessor;

/// <summary>
/// 
/// </summary>
/// <param name="Found"></param>
/// <param name="TimeObject"></param>
/// <param name="Exact">Whether the time was entered without ambiguity (ie. 5 oclock vs 5 PM)</param>
/// <param name="Text"></param>
/// <param name="Start"></param>
/// <param name="End"></param>
public record DateTimeResult(bool Found, DateTime ValueObject, bool Exact, string Text, int Start, int End) 
{ 
    public static DateTimeResult No_Result => new DateTimeResult(false, DateTime.MinValue, false, "", -1, -1);
}

/// <summary>
/// Represents generic information about a 'Task'
/// </summary>
/// <param name="Description"></param>
/// <param name="HasTime"></param>
/// <param name="OccursAt"></param>
public record TaskInformation(string Description, bool HasTime, DateTime OccursAt);

/// <summary>
/// Functionality to handle Natural Language Input with a Forgiving Format.
/// </summary>
public interface IForgivingFormatProcessor
{
    /// <summary>
    /// Tries to convert string input to a Date
    /// </summary>
    /// <remarks>
    /// eg “Next Tuesday”, “This Wednesday”, “Tomorrow”, “Friday”
    /// </remarks>
    /// <param name="input"></param>
    /// <returns></returns>
    DateTimeResult ProcessNaturalDate(string input);
    /// <summary>
    /// Tries to convert string input to a Time
    /// </summary>
    /// <remarks>
    /// eg “6”, “six in the morning”. “Half past six”, “3 PM”, “three
    /// in the afternoon”
    /// </remarks>
    /// <param name="input"></param>
    /// <returns></returns>
    DateTimeResult ProcessNaturalTime(string input);
    /// <summary>
    /// Tries to convert a string input into information about a 'Task'
    /// </summary>
    /// <remarks>
    /// \<Task\> on \<date\> at \<time\>. Eg “Call Rob on Wednesday at three PM”
    /// \<Task\> at\<time\> on\<date\>.Eg “Call Rob at three PM on Wednesday”
    /// \<Task\>. Eg. “Call Rob”
    /// \<Task\>, \<time\>, \<date\>. Eg. “Call Rob, three PM, Wednesday”
    /// \<Task\>, \<date\>, \<time\>. Eg. “Call Rob, Wednesday, three PM”
    /// \<Task\> \<time\> \<date\>. Eg. “Call Rob three PM Wednesday”
    /// \<Task\> \<date\> \<time\>. Eg. “Call Rob Wednesday three PM” 
    /// </remarks>
    /// <param name="input"></param>
    /// <returns><see cref="TaskInformation"/></returns>
    TaskInformation ProcessNaturalTask(string input);
}
