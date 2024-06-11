using Microsoft.Recognizers.Text;
using Microsoft.Recognizers.Text.DateTime;
using Microsoft.Recognizers.Text.Number;
using Microsoft.Recognizers.Text.NumberWithUnit;
using NaturalLanguageProcessor.Aggregates;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NaturalLanguageProcessor.MicrosoftRecognizers
{
    internal class RecognizersWrapper
    {
        private readonly string RecognizersCulture;
        private readonly NumberModel NaturalNumber;
        private readonly OrdinalModel NaturalOrdinal;
        private readonly NumberRangeModel NaturalNumberRange;
        private readonly DateTimeModel NaturalDateTime;
        private readonly AgeModel NaturalAge;

        public RecognizersWrapper(string recognizersCulture = Culture.English)
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
        public RecognizerDateTimeResult ParseWithDateTimeRecognizer(string query)
        {
            // Datetime recognizer This model will find any Date even if its write in coloquial language 
            // E.g "I'll go back 8pm today" will return "2017-10-04 20:00:00"
            var dateTimeValues = NaturalDateTime.Parse(query);
            if (dateTimeValues.Count == 0)
            {
                return null;
            }

            foreach (var entry in dateTimeValues)
            {
                var values = entry.Resolution.Values;
                foreach (var value in values)
                {
                    var vList = (List<Dictionary<string, string>>)value;
                    var stringValue = vList.First()["value"];
                    var dateValue = DateTime.Parse(stringValue);
                    Debug.WriteLine($"Date value as date: {dateValue.DayOfWeek} Day={dateValue.Day} Month={dateValue.Month} Year={dateValue.Year}");

                    return new RecognizerDateTimeResult(dateValue, entry.Text, entry.Start, entry.End + 1);
                }
            }

            return null;
        }

        public List<RecognizersNumberResult> ParseWithNumberRecognizer(string query)
        {
            // Number recognizer will find any number from the input
            // E.g "I have two apples" will return "2".
            throw new NotImplementedException();
        }

        public List<RecognizerNumberRangeResult> ParseWithNumberRangeRecognizer(string query)
        {
            throw new NotImplementedException();
        }

        public List<RecognizersNumberResult> ParseWithOrdinalRecognizer(string query)
        {
            // Ordinal number recognizer will find any ordinal number
            // E.g "eleventh" will return "11".
            throw new NotImplementedException();
        }

        public List<RecognizersAgeResult> ParseWithAgeRecognizer(string query)
        {
            // Age recognizer will find any age number presented
            // E.g "After ninety five years of age, perspectives change" will return "95 Year"
            throw new NotImplementedException();
        }

        private static DateTime? FindDateTimeValue(Dictionary<string, object> dict)
        {
            foreach (var entry in dict)
            {
                if (entry.Key == "value" && entry.Value is DateTime dt)
                {
                    return dt;
                }
                else if (entry.Value is Dictionary<string, object> nestedDict)
                {
                    var result = FindDateTimeValue(nestedDict);
                    if (result.HasValue)
                    {
                        return result;
                    }
                }
            }
            return null;
        }
    }
}