using System;
using System.Collections.Generic;
using System.Text;

namespace NaturalLanguageProcessor.Aggregates
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Found"></param>
    /// <param name="Exact">Whether the time was entered without ambiguity (ie. 5 oclock vs 5 PM)</param>
    /// <param name="Text"></param>
    /// <param name="Start"></param>
    /// <param name="End"></param>
#if NET8_0
    public record DateTimeResult(bool Found, DateTime ValueObject, bool Exact, string Text, int Start, int End)
    {
        public static DateTimeResult No_Result => new DateTimeResult(false, DateTime.MinValue, false, "", -1, -1);
    }

#elif NETSTANDARD2_0

    public class DateTimeResult
    {
        public bool Found { get; }
        public DateTime ValueObject { get; }
        public bool Exact { get; }
        public string Text { get; }
        public int Start { get; }
        public int End { get; }

        public static DateTimeResult No_Result => new DateTimeResult(false, DateTime.MinValue, false, "", -1, -1);

        public DateTimeResult(bool found, DateTime valueObject, bool exact, string text, int start, int end)
        {
            Found = found;
            ValueObject = valueObject;
            Exact = exact;
            Text = text;
            Start = start;
            End = end;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (obj == null || GetType() != obj.GetType()) return false;

            var other = (DateTimeResult)obj;
            return Found == other.Found &&
                   ValueObject == other.ValueObject &&
                   Exact == other.Exact &&
                   Text == other.Text &&
                   Start == other.Start &&
                   End == other.End;
        }
    }

#endif
}
