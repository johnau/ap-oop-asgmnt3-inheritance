using System.Diagnostics;

namespace NaturalLanguageProcessor.XunitTests;

public class ForgivingFormatWithRecognizersAndSpacyUnitTests
{
    [Fact]
    public void ProcessDate_WithProvidedStrings_WillSuceed()
    {
        var todayValue = DateTime.Now.DayOfWeek;
        var fridayValue = DayOfWeek.Friday;
        var diffToFriday = fridayValue - todayValue;
        if (diffToFriday < 0) diffToFriday += 7;

        DateOnly nextTuesday;
        DateOnly nextNextTuesday;
        DateOnly thisWednesday;
        DateOnly today = DateOnly.FromDateTime(DateTime.Now);
        DateOnly tomorrow = DateOnly.FromDateTime(DateTime.Now).AddDays(1);
        DateOnly friday = DateOnly.FromDateTime(DateTime.Now).AddDays(diffToFriday);

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
                thisWednesday = today.AddDays(6); // this will throw an error, but it should, shouldn't it?
                break;
            case DayOfWeek.Friday:
                nextTuesday = today.AddDays(4);
                nextNextTuesday = today.AddDays(11);
                thisWednesday = today.AddDays(5);
                break;
            case DayOfWeek.Saturday:
                nextTuesday = today.AddDays(3);
                nextNextTuesday = today.AddDays(10);
                thisWednesday = today.AddDays(4);
                break;
            case DayOfWeek.Sunday:
                nextTuesday = today.AddDays(9);
                nextNextTuesday = today.AddDays(9);
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
        };

        var ffHandler = new ForgivingFormatWithRecognizersAndSpacyHandler();

        foreach (var mockInput in mockInputList)
        {
            var expected = mockInput.Value;
            var result = ffHandler.ProcessNaturalDate(mockInput.Key);
            Debug.WriteLine($"Result info: Found:{result.Found} V:{result.ValueObject} -- {result.Text} ({result.Start} -> {result.End})");

            Assert.True(result.Found);

            var _result = DateOnly.FromDateTime(result.ValueObject);
            Assert.Equal(expected.Day, result.ValueObject.Day);
            Assert.Equal(expected.Month, result.ValueObject.Month);
            Assert.Equal(expected.Year, result.ValueObject.Year);

            var strRemainder = mockInput.Key.Remove(result.Start, result.End - result.Start).Trim();
            Debug.WriteLine($"Remaining string: {strRemainder}");
            Assert.Equal("call rob", strRemainder.ToLower());
        }

        Debug.WriteLine("- Process Date Test Complete\n");
    }

    [Fact]
    public void ProcessTime_WithProvidedStrings_WillSuceed()
    {

    }

    [Fact]
    public void ProcessTask_WithProvidedStrings_WillSucceed() 
    { 

    }
}
