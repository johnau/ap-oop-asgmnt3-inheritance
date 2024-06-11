using System;
using System.Collections.Generic;
using System.Text;

namespace NaturalLanguageProcessor.Aggregates
{
#if NET8_0_OR_GREATER
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Value"></param>
    /// <param name="Period"></param>
    /// <param name="Text"></param>
    /// <param name="Start"></param>
    /// <param name="End"></param>
    public record RecognizersAgeResult(double Value, string Period, string Text, int Start, int End) { }
#elif NETSTANDARD2_0
    public class RecognizersAgeResult
    {
        public double Value { get; }
        public string Period { get; }
        public string Text { get; }
        public int Start { get; }
        public int End { get; }

        public RecognizersAgeResult(double value, string period, string text, int start, int end)
        {
            Value = value;
            Period = period;
            Text = text;
            Start = start;
            End = end;
        }
    }
#endif
}
