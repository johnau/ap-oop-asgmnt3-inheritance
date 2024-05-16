using NaturalLanguageProcessor.Utility;
using System.Diagnostics;

namespace NaturalLanguageProcessor.XunitTests;

public class ForgivingFormatWithRegexUnitTests
{

    //[Fact]
    [Theory]
    [InlineData(2024, 5, 6)] // monday
    [InlineData(2024, 5, 7)] // tuesday
    [InlineData(2024, 5, 8)] // wednesday
    [InlineData(2024, 5, 9)] // thursday
    [InlineData(2024, 5, 10)] // friday
    [InlineData(2024, 5, 11)] // saturday
    [InlineData(2024, 5, 12)] // sunday
    public void ProcessDate_WithProvidedStrings_WillSuceed(int year, int month, int day)
    {
        DateTime mockTime = new DateTime(year, month, day, 12, 30, 1);

        var todayValue = mockTime.DayOfWeek;
        var fridayValue = DayOfWeek.Friday;
        var diffToFriday = fridayValue - todayValue;
        if (diffToFriday < 0) diffToFriday += 7;

        DateOnly nextTuesday;
        DateOnly nextNextTuesday;
        DateOnly thisWednesday;
        DateOnly today = DateOnly.FromDateTime(mockTime);
        Debug.WriteLine($"Today: {today}");

        DateOnly tomorrow = DateOnly.FromDateTime(mockTime).AddDays(1);
        DateOnly friday = DateOnly.FromDateTime(mockTime).AddDays(diffToFriday);
        
        switch (todayValue)
        {
            case DayOfWeek.Monday:
                nextTuesday = today.AddDays(8);
                nextNextTuesday = today.AddDays(8);
                thisWednesday = today.AddDays(2);
                break;
            case DayOfWeek.Tuesday:
                nextTuesday = today.AddDays(7);
                nextNextTuesday = today.AddDays(14);
                thisWednesday = today.AddDays(1);
                break;
            case DayOfWeek.Wednesday:
                nextTuesday = today.AddDays(6);
                nextNextTuesday = today.AddDays(13);
                thisWednesday = today.AddDays(0); // if someone says "this wednesday" on wednesday, they are surely not talking about today ("Did you mean today?")
                break;
            case DayOfWeek.Thursday:
                nextTuesday = today.AddDays(5);
                nextNextTuesday = today.AddDays(12);
                thisWednesday = today.AddDays(6);
                break;
            case DayOfWeek.Friday:
                nextTuesday = today.AddDays(4);
                nextNextTuesday = today.AddDays(11);
                thisWednesday = today.AddDays(5);
                break;
            case DayOfWeek.Saturday:
                nextTuesday = today.AddDays(10);
                nextNextTuesday = today.AddDays(17);
                thisWednesday = today.AddDays(4);
                break;
            case DayOfWeek.Sunday:
                nextTuesday = today.AddDays(9);
                nextNextTuesday = today.AddDays(16);
                thisWednesday = today.AddDays(3);
                break;
            default:
                throw new Exception("The default has to be here so compiler does not complain about an unassigned variable (nextTuesday, thisWednesday)");
        }

        Dictionary<string, DateOnly> mockInputList = new Dictionary<string, DateOnly>(){
            { "Call Rob Next Tuesday", nextTuesday },
            { "Call Rob This Wednesday", thisWednesday },
            { "Call Rob Tomorrow", tomorrow },
            { "Call Rob Friday", friday },
            { "Call Rob Tuesday after next", nextNextTuesday },
        };

        var ffHandler = new ForgivingFormatWithRegexProcessor(mockTime);

        foreach (var mockInput in mockInputList)
        {
            var expected = mockInput.Value;
            var result = ffHandler.ProcessNaturalDate(mockInput.Key);
            Debug.WriteLine($"Result info: Found:{result.Found} V:{result.ValueObject} -- {result.Text} ({result.Start} -> {result.End})");

            Assert.True(result.Found);

            //var _result = DateOnly.FromDateTime(result.ValueObject);
            Assert.Equal(expected.Day, result.ValueObject.Day);
            Assert.Equal(expected.Month, result.ValueObject.Month);
            Assert.Equal(expected.Year, result.ValueObject.Year);

            var strRemainder = mockInput.Key.Remove(result.Start, result.End - result.Start).Trim();
            Debug.WriteLine($"Remaining string: {strRemainder}");
            Assert.Equal("call rob", strRemainder.ToLower());
        }

        Debug.WriteLine("- ProcessNaturalDate() Test Complete\n");
    }

    [Theory]
    [InlineData(3, 00)] // 3:00AM
    [InlineData(9, 30)] // 9:30AM
    [InlineData(14, 30)]
    [InlineData(15, 30)]
    [InlineData(16, 14)]
    [InlineData(17, 00)]
    [InlineData(18, 02)]
    public void ProcessTime_WithProvidedStrings_WillSuceed(int hour, int minute)
    {
        DateTime mockTime = new DateTime(2024, 1, 1, hour, minute, 1);

        var nowTime = new TimeOnly(hour, minute);

        var after6amBefore6pm = (nowTime.Hour == 6 && nowTime.Minute > 0 || nowTime.Hour > 6)
                                                                        && (nowTime.Hour < 18);
        var six = after6amBefore6pm ? new TimeOnly(18, 0) : new TimeOnly(6, 0);
        var sixAM = new TimeOnly(6, 0);
        var sevenAM = new TimeOnly(7, 0);

        var after630amBefore630pm = (nowTime.Hour == 6 && nowTime.Minute > 30 || nowTime.Hour > 6) 
                                && (nowTime.Hour == 18 && nowTime.Minute < 30 || nowTime.Hour < 18);
        var six30 = after630amBefore630pm ? new TimeOnly(18, 30) : new TimeOnly(6, 30);
        var threePM = new TimeOnly(15, 0);

        var after345amBefore345pm = (nowTime.Hour == 3 && nowTime.Minute > 45 || nowTime.Hour > 3) 
                                && (nowTime.Hour == 15 && nowTime.Minute < 45 || nowTime.Hour < 15);
        var three45 = after345amBefore345pm ? new TimeOnly(15, 45) : new TimeOnly(3, 45);

        var after415amBefore415pm = (nowTime.Hour == 4 && nowTime.Minute > 15 || nowTime.Hour > 4) 
                                && (nowTime.Hour == 16 && nowTime.Minute < 15 || nowTime.Hour < 16);
        var four15 = after415amBefore415pm ? new TimeOnly(16, 15) : new TimeOnly(4, 15);

        var tenAm = new TimeOnly(10, 0);
        var tenPm = new TimeOnly(22, 0);
        var noon = new TimeOnly(12, 0);

        Dictionary<string, TimeOnly> mockInputList = new Dictionary<string, TimeOnly>() {
            { "Call Rob at 6", six },
            { "Call Rob at six in the morning", sixAM },
            { "Call Rob at seven AM", sevenAM },
            { "Call Rob at Half past six", six30 },
            { "Call Rob at quarter to 4", three45 },
            { "Call Rob at a quarter past 4", four15 },
            { "Call Rob at 3 PM", threePM },
            { "Call Rob at three in the afternoon", threePM },
            { "Call Rob at ten this morning", tenAm },
            { "Call Rob at ten this evening", tenPm },
            { "Call Rob at midday", noon },
        };

        var ffHandler = new ForgivingFormatWithRegexProcessor(mockTime);

        foreach (var input in mockInputList)
        {
            var expected = input.Value;
            var result = ffHandler.ProcessNaturalTime(input.Key);

            Assert.Equal(expected.Hour, result.ValueObject.Hour);
            Assert.Equal(expected.Minute, result.ValueObject.Minute);
            Assert.Equal(expected.Second, result.ValueObject.Second);

            var _inputMod = input.Key.Remove(result.Start, result.End - result.Start);
            _inputMod = NaturalLanguageHelper.TrimPrepositionWords(_inputMod);
            Debug.WriteLine($"Remainder string: '{_inputMod}'");

            Assert.Equal("Call Rob", _inputMod);
        }

        Debug.WriteLine("- ProcessNaturalTime() Test Complete\n");
    }

    [Fact]
    public void ProcessTask_WithProvidedStrings_WillSucceed()
    {
        var ffHandler = new ForgivingFormatWithRegexProcessor();

        var wed = DateOnly.FromDateTime(DateTime.Now);
        while (wed.DayOfWeek != DayOfWeek.Wednesday) wed = wed.AddDays(1);
        var threePm = new TimeOnly(15, 0);

        var wed3pm = wed.ToDateTime(threePm);

        var callWed3pm = new TaskInformation("Call Rob", true, wed3pm);
        var call = new TaskInformation("Call Rob", false, DateTime.MinValue);

        Dictionary<string, TaskInformation> mockInputList = new Dictionary<string, TaskInformation>(){
            { "Call Rob on Wednesday at three PM", callWed3pm }, //<Task> on <date> at <time>
            { "Call Rob at three PM on Wednesday", callWed3pm }, //<Task> at <time> on <date>
            { "Call Rob", call }, //<Task>
            { "Call Rob, three PM, Wednesday", callWed3pm }, //<Task>, <time>, <date>
            { "Call Rob, Wednesday, three PM", callWed3pm }, //<Task>, <date>, <time>
            { "Call Rob three PM Wednesday", callWed3pm }, // <Task> <time> <date>
            { "Call Rob Wednesday three PM", callWed3pm } // <Task> <date> <time>
        };

        foreach (var expected in mockInputList)
        {
            var result = ffHandler.ProcessNaturalTask(expected.Key);
            Assert.Equal("Call Rob", result.Description);
            Assert.Equal(expected.Value.OccursAt, result.OccursAt);
            Debug.WriteLine("Success");
        }

        Debug.WriteLine("- ProcessNaturalTask() Tests Complete\n");
    }
}