using System;
using System.Collections.Generic;
using System.Text;

namespace NaturalLanguageProcessor.Aggregates
{
#if NET8_0_OR_GREATER
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Value1"></param>
    /// <param name="Value2"></param>
    /// <param name="Text"></param>
    /// <param name="Start"></param>
    /// <param name="End"></param>
    public record RecognizerNumberRangeResult(double Value1, double Value2, string Text, int Start, int End) { }
#elif NETSTANDARD2_0
    public class RecognizerNumberRangeResult
    {
        public double Value1 { get; }
        public double Value2 { get; }
        public string Text { get; }
        public int Start { get; }
        public int End { get; }

        public RecognizerNumberRangeResult(double value1, double value2, string text, int start, int end)
        {
            Value1 = value1;
            Value2 = value2;
            Text = text;
            Start = start;
            End = end;
        }
    }
#endif
}
