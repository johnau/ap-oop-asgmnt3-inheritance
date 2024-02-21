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
            repeatingTaskData.FakeDateTime = now;

            var dueDate = repeatingTaskData.DueDate;
            var overdue = repeatingTaskData.Overdue;
            now = DateTime.Now;
            Console.WriteLine($"{dueDate.ToString("yyyy-MM-dd HH:mm:ss")}");
            
            Assert.False(overdue);

            repeatingTaskData.FakeDateTime = now.AddMinutes(61);
            //repeatingTaskData.Overdue 
            dueDate = repeatingTaskData.DueDate;
            overdue = repeatingTaskData.Overdue;
            Console.WriteLine($"{dueDate.ToString("yyyy-MM-dd HH:mm:ss")}");
            Assert.True(overdue);
        }
    }
}
