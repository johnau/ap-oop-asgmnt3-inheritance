using System.Diagnostics;
using TaskManagerCore.XunitTests.TestHelpers;

namespace TaskManagerCore.XunitTests
{
    public class RepeatingTaskDataTests
    {
        [Fact]
        public void WithCompleted_BeforeDueDate_WillChangeDueDate()
        {
            var now = DateTime.Now;
            var repeatingTaskData = new RepeatingTaskDataTestHelperExtension("Repeating task description", "", now.AddHours(1), Model.TimeInterval.Hourly);
            var overdue = repeatingTaskData.Overdue;
            Assert.True(overdue);

            repeatingTaskData.FakeDateTime = now.AddMinutes(61);
            //repeatingTaskData.Overdue 
            overdue = repeatingTaskData.Overdue;
            var dueDate = repeatingTaskData.DueDate;
            Console.WriteLine($"{dueDate.ToString("yyyy-MM-dd HH:mm:ss")}");
            Assert.False(overdue);
        }
    }
}
