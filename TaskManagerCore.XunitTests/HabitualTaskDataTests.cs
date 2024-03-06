using Microsoft.VisualStudio.TestPlatform.Utilities;
using System.Diagnostics;
using TaskManagerCore.XunitTests.TestHelpers;

namespace TaskManagerCore.XunitTests
{
    public class HabitualTaskDataTests
    {

        [Fact]
        public void CheckOverdueHourlyTwice_WithCompleted_WillNotBeOverdue()
        {
            var now = DateTime.Now;
            var habitualTask = new HabitualTaskDataTestHelperExtension("Habitual task description", "", now.AddHours(1), Model.TimeInterval.Hourly);

            Assert.False(habitualTask.Overdue);

            habitualTask.FakeDateTime = now.AddMinutes(61); // 1 min past old due date

            Assert.True(habitualTask.Overdue);

            habitualTask = habitualTask.WithCompleted(true);

            Assert.False(habitualTask.Overdue);
        }

        [Fact]
        public void HourlyTaskWithStreak_WithCompleted_WillHaveAStreak()
        {
            var testLimit = 5;
            var now = DateTime.Now;
            var habitualTask = new HabitualTaskDataTestHelperExtension("Habitual task description", "", now.AddHours(1), Model.TimeInterval.Hourly);
            Debug.WriteLine($"Habitual Task {habitualTask.DueDate} (is overdue: {habitualTask.Overdue})");
            var minutesFromNow = 30;
            
            habitualTask.FakeDateTime = now.AddMinutes(minutesFromNow);
            Assert.False(habitualTask.Overdue);

            for (int i = 1; i <= testLimit; i++)
            {
                minutesFromNow += 60;
                habitualTask.FakeDateTime = now.AddMinutes(minutesFromNow);
                habitualTask = habitualTask.WithCompleted(true);
                Assert.False(habitualTask.Overdue);
                Console.WriteLine($"Habitual Task {habitualTask.DueDate} (is overdue: {habitualTask.Overdue})");
            }

            Assert.Equal(testLimit, habitualTask.Streak);
        }

        [Fact]
        public void HourlyTaskWithStreak_WithCompleted_WillNotHaveAStreak()
        {
            var testLimit = 5;
            var now = DateTime.Now;
            var habitualTask = new HabitualTaskDataTestHelperExtension("Habitual task description", "", now.AddHours(1), Model.TimeInterval.Hourly);
            Debug.WriteLine($"Habitual Task {habitualTask.DueDate} (is overdue: {habitualTask.Overdue})");
            var minutesFromNow = 65;

            habitualTask.FakeDateTime = now.AddMinutes(minutesFromNow);
            Assert.True(habitualTask.Overdue);

            for (int i = 1; i <= testLimit; i++)
            {
                minutesFromNow += 65;
                habitualTask.FakeDateTime = now.AddMinutes(minutesFromNow);
                Assert.True(habitualTask.Overdue);
                habitualTask = habitualTask.WithCompleted(true);
                Assert.False(habitualTask.Overdue);
                Debug.WriteLine($"!!! Habitual Task {habitualTask.DueDate} (is overdue: {habitualTask.Overdue}, current streak is {habitualTask.Streak})");
            }

            Assert.Equal(0, habitualTask.Streak);
        }
    }
}
