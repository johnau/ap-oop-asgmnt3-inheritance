using TaskManagerCore.XunitTests.TestHelpers;

namespace TaskManagerCore.XunitTests
{
    public class RepeatingTaskDataTests
    {
        [Fact]
        public void CheckOverdueTwice_WithCompleted_WillNotBeOverdue()
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
        public void CheckOverdue_PassedDueDate_WillBeOverdue()
        {
            var now = DateTime.Now;
            var repeatingTask = new RepeatingTaskDataTestHelperExtension("Repeating task description", "", now.AddHours(1), Model.TimeInterval.Hourly);

            Assert.False(repeatingTask.Overdue);

            repeatingTask.FakeDateTime = now.AddMinutes(61); // 1 min past old due date

            Assert.True(repeatingTask.Overdue);
        }

        [Fact]
        public void CheckOverdue_WithCompleted_WillNotBeOverdue()
        {
            var now = DateTime.Now;
            var repeatingTask = new RepeatingTaskDataTestHelperExtension("Repeating task description", "", now.AddHours(1), Model.TimeInterval.Hourly);

            Assert.False(repeatingTask.Overdue);

            repeatingTask = repeatingTask.WithCompleted(true);
            repeatingTask.FakeDateTime = now.AddMinutes(61); // 1 min past old due date
            
            Assert.False(repeatingTask.Overdue);
        }
    }
}
