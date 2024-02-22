using System.Diagnostics;
using TaskManagerCore.XunitTests.TestHelpers;

namespace TaskManagerCore.XunitTests
{
    public class RepeatingTaskDataTests
    {
        [Fact]
        public void CheckOverdueHourlyTwice_WithCompleted_WillNotBeOverdue()
        {
            var now = DateTime.Now;
            var repeatingTask = new RepeatingTaskDataTestHelperExtension("Repeating task description", "", now.AddHours(1), Model.TimeInterval.Hourly);

            Assert.False(repeatingTask.Overdue);

            repeatingTask.FakeDateTime = now.AddMinutes(61); // 1 min past old due date

            Assert.True(repeatingTask.Overdue);

            repeatingTask = repeatingTask.WithCompleted(true);

            Assert.False(repeatingTask.Overdue);
        }

        [Fact]
        public void CheckOverdueHourly_PassedDueDate_WillBeOverdue()
        {
            var now = DateTime.Now;
            var repeatingTask = new RepeatingTaskDataTestHelperExtension("Repeating task description", "", now.AddHours(1), Model.TimeInterval.Hourly);

            Assert.False(repeatingTask.Overdue);

            repeatingTask.FakeDateTime = now.AddMinutes(61); // 1 min past old due date

            Assert.True(repeatingTask.Overdue);
        }

        [Fact]
        public void CheckOverdueHourly_WithCompleted_WillNotBeOverdue()
        {
            var now = DateTime.Now;
            var repeatingTask = new RepeatingTaskDataTestHelperExtension("Repeating task description", "", now.AddHours(1), Model.TimeInterval.Hourly);

            Assert.False(repeatingTask.Overdue);

            repeatingTask = repeatingTask.WithCompleted(true);
            repeatingTask.FakeDateTime = now.AddMinutes(61); // 1 min past old due date
            
            Assert.False(repeatingTask.Overdue);
        }

        [Fact]
        public void CheckOverdueDaily_WithCompleted_WillNotBeOverdue()
        {
            var now = DateTime.Now;
            var repeatingTask = new RepeatingTaskDataTestHelperExtension("Repeating task description", "", now.AddHours(1), Model.TimeInterval.Daily);

            Assert.False(repeatingTask.Overdue);

            repeatingTask.FakeDateTime = now.AddMinutes(30);

            Console.WriteLine($"DueDate (initial): {repeatingTask.DueDate.ToString()}");

            repeatingTask = repeatingTask.WithCompleted(true);

            Console.WriteLine($"DueDate (next): {repeatingTask.DueDate.ToString()}");

            Assert.False(repeatingTask.Overdue);

            repeatingTask.FakeDateTime = now.AddHours(12); 

            Assert.False(repeatingTask.Overdue);

            repeatingTask.FakeDateTime = now.AddHours(26);

            Assert.True(repeatingTask.Overdue);

        }
    }
}
