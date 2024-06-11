using System;

namespace NaturalLanguageProcessor
{

    /// <summary>
    /// 
    /// </summary>
    public class TooManyValuesFoundException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public TooManyValuesFoundException(string message) : base(message) { }
    }
}