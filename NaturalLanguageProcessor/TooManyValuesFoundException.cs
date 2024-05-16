namespace NaturalLanguageProcessor;

public class TooManyValuesFoundException : Exception
{
    public TooManyValuesFoundException(string message) : base(message) { }
}
