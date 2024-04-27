using NaturalLanguageProcessor.Microsoft.Recognizers;
using System.Reflection.Metadata;
using System.Threading;

namespace NaturalLanguageProcessor.XunitTests
{
    public class ForgivingFormatUnitTests
    {
        [Fact]
        public void ProcessDate_WithProvidedStrings_WillSuceed()
        {
            var todayValue = new DateOnly().DayOfWeek;
            var fridayValue = DayOfWeek.Friday;
            var diffToFriday = fridayValue - todayValue;
            if (diffToFriday < 0) diffToFriday += 7;

            DateOnly nextTuesday;
            DateOnly thisWednesday;
            DateOnly tomorrow = new DateOnly().AddDays(1);
            DateOnly friday = new DateOnly().AddDays(diffToFriday);
            
            switch (todayValue)
            {
                case DayOfWeek.Monday:
                    nextTuesday = new DateOnly().AddDays(8);
                    thisWednesday = new DateOnly().AddDays(2);
                    break;
                case DayOfWeek.Tuesday:
                    nextTuesday = new DateOnly().AddDays(7);
                    thisWednesday = new DateOnly().AddDays(1);
                    break;
                case DayOfWeek.Wednesday:
                    nextTuesday = new DateOnly().AddDays(6);
                    thisWednesday = new DateOnly().AddDays(0); // if someone says "this wednesday" on wednesday, they are surely not talking about today ("Did you mean today?")
                    break;
                case DayOfWeek.Thursday:
                    nextTuesday = new DateOnly().AddDays(5);
                    thisWednesday = new DateOnly().AddDays(-1);
                    break;
                case DayOfWeek.Friday:
                    nextTuesday = new DateOnly().AddDays(4);
                    thisWednesday = new DateOnly().AddDays(-2);
                    break;
                case DayOfWeek.Saturday:
                    nextTuesday = new DateOnly().AddDays(3);
                    thisWednesday = new DateOnly().AddDays(4);
                    break;
                case DayOfWeek.Sunday:
                    nextTuesday = new DateOnly().AddDays(9);
                    thisWednesday = new DateOnly().AddDays(3);
                    break;
                default:
                    throw new Exception("The default has to be here so compiler does not complain about an unassigned variable (nextTuesday, thisWednesday)");
            }

            Dictionary<string, DateOnly> mockInputList = new Dictionary<string, DateOnly>(){
                { "Next Tuesday", nextTuesday },
                { "This Wednesday", thisWednesday },
                { "Tomorrow", tomorrow },
                { "Friday", friday },
            };

            var ffHandler = new ForgivingFormatWithRecognizersHandler();

            foreach (var input in mockInputList)
            {
                var expected = input.Value;
                var result = ffHandler.ProcessNaturalDate(input.Key);

                Assert.Equal(expected.Day, result.Day);
                Assert.Equal(expected.Month, result.Month);
                Assert.Equal(expected.Year, result.Year);
                Assert.Equal(expected, result);
            }
        }

        [Fact]
        public void ProcessTime_WithProvidedStrings_WillSuceed()
        {
            var nowTime = new TimeOnly();
            var six = nowTime.Hour < 6 ? new TimeOnly(6, 0) : new TimeOnly(18, 0);
            var sixAM = new TimeOnly(6, 0);
            var six30 = six.AddMinutes(30);
            var threePM = new TimeOnly(15, 0);

            Dictionary<string, TimeOnly> mockInputList = new Dictionary<string, TimeOnly>(){
                { "6", six },
                { "six in the morning", sixAM },
                { "Half past six", six30 },
                { "3 PM", threePM },
                { "three in the afternoon", threePM },
            };

            var ffHandler = new ForgivingFormatWithRecognizersHandler();

            foreach (var input in mockInputList)
            {
                var expected = input.Value;
                var result = ffHandler.ProcessNaturalTime(input.Key);

                Assert.Equal(expected.Hour, result.Minute);
                Assert.Equal(expected.Hour, result.Minute);
                Assert.Equal(expected.Hour, result.Minute);
                Assert.Equal(expected, result);
            }
        }

        [Fact]
        public void ProcessTask_WithProvidedStrings_WillSucceed()
        {
            var ffHandler = new ForgivingFormatWithRecognizersHandler();

            var wed = new DateOnly();
            while (wed.DayOfWeek != DayOfWeek.Wednesday) wed = wed.AddDays(1);
            var threePm = new TimeOnly(15, 0);
            var callWed3pm = new TaskInformation("Call Rob", wed, threePm);
            var call = new TaskInformation("Call Rob", null, null);

            Dictionary<string, TaskInformation> mockInputList = new Dictionary<string, TaskInformation>(){
                { "Call Rob on Wednesday at three PM", callWed3pm }, //<Task> on <date> at <time>
                { "Call Rob at three PM on Wednesday", callWed3pm }, //<Task> at <time> on <date>
                { "Call Rob", call }, //<Task>
                { "Call Rob, three PM, Wednesday", callWed3pm }, //<Task>, <time>, <date>
                { "Call Rob, Wednesday, three PM", callWed3pm }, //<Task>, <date>, <time>
                { "Call Rob three PM Wednesday", callWed3pm }, // <Task> <time> <date>
                { "Call Rob Wednesday three PM", callWed3pm } // <Task> <date> <time>
            };

            foreach (var s in mockInputList)
            {
                var result = ffHandler.ProcessNaturalTask(s.Key);
                Assert.Equal(s.Value.Description, result.Description);
                Assert.Equal(s.Value.Date, result.Date);
                Assert.Equal(s.Value.Time, result.Time);
            }
        }

        [Theory]
        [InlineData("Call Rob on Wednesday at three PM", "Call Rob", DayOfWeek.Wednesday, 15, 0)]
        [InlineData("Call Rob at three PM on Wednesday", "Call Rob", DayOfWeek.Wednesday, 15, 0)]
        [InlineData("Call Rob", "Call Rob", null, null)]
        [InlineData("Call Rob, three PM, Wednesday", "Call Rob", DayOfWeek.Wednesday, 15, 0)]
        [InlineData("Call Rob, Wednesday, three PM", "Call Rob", DayOfWeek.Wednesday, 15, 0)]
        [InlineData("Call Rob three PM Wednesday", "Call Rob", DayOfWeek.Wednesday, 15, 0)]
        [InlineData("Call Rob Wednesday three PM", "Call Rob", DayOfWeek.Wednesday, 15, 0)]
        public void TProcessTask_WithProvidedStrings_WillSucceed(string inputString, string expectedDescription,
                                                DayOfWeek? expectedDayOfWeek, int? expectedHour, int expectedMinute = 0)
        {
            var date = new DateOnly();
            DateOnly? expectedDate = null;
            if (expectedDayOfWeek.HasValue)
            {
                while (date.DayOfWeek != expectedDayOfWeek) date = date.AddDays(1);
                expectedDate = new DateOnly(date.Year, date.Month, date.Day);
            }
            TimeOnly? expectedTime = null;
            if (expectedHour.HasValue)
            {
                expectedTime = new TimeOnly(expectedHour.Value, expectedMinute);
            }
            
            var expectedResult = new TaskInformation(expectedDescription, expectedDate, expectedTime);

            var ffHandler = new ForgivingFormatWithRecognizersHandler();

            var result = ffHandler.ProcessNaturalTask(inputString);

            Assert.Equal(expectedResult.Description, result.Description);
            Assert.Equal(expectedResult.Date, result.Date);
            Assert.Equal(expectedResult.Time, result.Time);
        }
    }
}