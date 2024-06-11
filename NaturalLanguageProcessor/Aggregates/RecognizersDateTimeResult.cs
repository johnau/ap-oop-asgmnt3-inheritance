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
    /// <param name="Text"></param>
    /// <param name="Start"></param>
    /// <param name="End"></param>
    public record RecognizerDateTimeResult(DateTime Value, string Text, int Start, int End) { }
#elif NETSTANDARD2_0
    public class RecognizerDateTimeResult
    {
        public DateTime Value { get; }
        public string Text { get; }
        public int Start { get; }
        public int End { get; }

        public RecognizerDateTimeResult(DateTime value, string text, int start, int end)
        {
            Value = value;
            Text = text;
            Start = start;
            End = end;
        }
    }
#endif
}
