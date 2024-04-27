
using Microsoft.Recognizers.Text.Number;
using Microsoft.Recognizers.Text;
using System.Text.RegularExpressions;
using Microsoft.Recognizers.Text.NumberWithUnit;
using Microsoft.Recognizers.Text.DateTime;

namespace NaturalLanguageProcessor.Microsoft.Recognizers;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// 
/// </remarks>
public class ForgivingFormatWithRecognizersHandler : IForgivingFormatProcessor
{
    private readonly string RecognizersCulture;
    private readonly NumberModel NaturalNumber;
    private readonly OrdinalModel NaturalOrdinal;
    private readonly NumberRangeModel NaturalNumberRange;
    private readonly DateTimeModel NaturalDateTime;
    private readonly AgeModel NaturalAge;

    public ForgivingFormatWithRecognizersHandler(string recognizersCulture = Culture.English)
    {
        RecognizersCulture = recognizersCulture;
        var numberRecognizer = new NumberRecognizer(RecognizersCulture);
        NaturalNumber = numberRecognizer.GetNumberModel();
        NaturalOrdinal = numberRecognizer.GetOrdinalModel();
        NaturalNumberRange = numberRecognizer.GetNumberRangeModel();
        var numberWithUnitRecognizer = new NumberWithUnitRecognizer(RecognizersCulture);
        NaturalAge = numberWithUnitRecognizer.GetAgeModel();

        numberRecognizer.GetNumberRangeModel();
        var dateRecognizer = new DateTimeRecognizer(RecognizersCulture);
        NaturalDateTime = dateRecognizer.GetDateTimeModel();


    }

    public DateOnly ProcessNaturalDate(string input)
    {
        var num = DetectNumber(input);


        throw new NotImplementedException();
    }

    public TaskInformation ProcessNaturalTask(string input)
    {

        throw new NotImplementedException();
    }

    public TimeOnly ProcessNaturalTime(string input)
    {
        throw new NotImplementedException();
    }

    private int DetectNumber(string query)
    {
        // Number recognizer will find any number from the input
        // E.g "I have two apples" will return "2".
        var results_number = NaturalNumber.Parse(query);
        var results_ordinal = NaturalOrdinal.Parse(query);
        var results_range = NaturalNumberRange.Parse(query);


        throw new NotImplementedException(); ;
    }

    private int DetectOrdinal(string query)
    {
        // Ordinal number recognizer will find any ordinal number
        // E.g "eleventh" will return "11".
        var ordinalValue = NaturalOrdinal.Parse(query);

        throw new NotImplementedException();
    }

    private (int, int) DetectNumberRange(string query)
    {
        // Number Range recognizer will find any cardinal or ordinal number range
        // E.g. "between 2 and 5" will return "(2,5)"
        var rangeValue = NaturalNumberRange.Parse(query);


        throw new NotImplementedException();
    }

    private (int, string) DetectAge(string query)
    {
        // Age recognizer will find any age number presented
        // E.g "After ninety five years of age, perspectives change" will return "95 Year"
        var ageValue = NaturalAge.Parse(query);

        throw new NotImplementedException();
    }



    private DateTime DetectDateTime(string query)
    {
        // Datetime recognizer This model will find any Date even if its write in coloquial language 
        // E.g "I'll go back 8pm today" will return "2017-10-04 20:00:00"
        var dateTimeValue = NaturalDateTime.Parse(query);

        throw new NotImplementedException();
    }
}
