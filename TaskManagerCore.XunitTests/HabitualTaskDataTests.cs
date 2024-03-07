using Microsoft.VisualStudio.TestPlatform.Utilities;
using System.Diagnostics;
using TaskManagerCore.Model;
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
                habitualTask.FakeDateTime = now.AddMinutes(minutesFromNow);
                habitualTask = habitualTask.WithCompleted(true);
                Assert.False(habitualTask.Overdue);
                Console.WriteLine($"Habitual Task {habitualTask.DueDate} (is overdue: {habitualTask.Overdue})");
                minutesFromNow += 60;
            }

            Assert.Equal(testLimit, habitualTask.Streak);
        }

        [Fact]
        public void HourlyTaskWithStreak_WithCompleted_WillNotHaveAStreak()
        {
            var testLimit = 5;
            var now = DateTime.Now;
            var habitualTask = new HabitualTaskDataTestHelperExtension("Habitual task description", "", now.AddHours(1), TimeInterval.Hourly);
            Debug.WriteLine($"Habitual Task {habitualTask.DueDate} (is overdue: {habitualTask.Overdue})");
            var minutesFromNow = 61;

            for (int i = 1; i <= testLimit; i++)
            {
                
                habitualTask.FakeDateTime = now.AddMinutes(minutesFromNow);
                Assert.True(habitualTask.Overdue);
                habitualTask = habitualTask.WithCompleted(true);
                Assert.False(habitualTask.Overdue);
                Debug.WriteLine($"!!! Habitual Task {habitualTask.DueDate} (is overdue: {habitualTask.Overdue}, current streak is {habitualTask.Streak})");
                minutesFromNow += 61;
            }

            Assert.Equal(0, habitualTask.Streak);
        }

        /// <summary>
        /// Fully tests the Habitual Task (Fail to create streak, create streak, ruin streak)
        /// It first fails to create a streak by completing hourly tasks 1 minute late
        /// - Assertions  are made to ensure that behavior is as expected
        /// It then succeeds to create a streak by completing hourly tasks within the due time
        /// - Assertions are made to ensure that behavior is as expected
        /// It then fails to meet a streak, thereby resetting streak to 0
        /// - Assertions are made to ensure that behavior is as expected
        /// </summary>
        [Theory]
        [InlineData(TimeInterval.Hourly)]
        [InlineData(TimeInterval.Daily)]
        [InlineData(TimeInterval.Weekly)]
        [InlineData(TimeInterval.Monthly)]
        [InlineData(TimeInterval.Yearly)]
        public void HabitualTask_TestStreakBehavior(TimeInterval timeInterval)
        {
            var testLimit = 5;
            var intervalInMinutes = (int)timeInterval * 60;
            var now = DateTime.Now;
            var habitualTask = new HabitualTaskDataTestHelperExtension("Habitual task description", "", now.AddHours(1), timeInterval); // start with a task due in an hour
            var minutesFromNow = intervalInMinutes + 1; // 1 minute offset to ensure we start overdue.

            Debug.WriteLine($"Habitual Task Due @ {habitualTask.DueDate} (is overdue: {habitualTask.Overdue}, current streak is {habitualTask.Streak}), Repitions={habitualTask.Repititions}");

            // Loop through amount of times for test
            // Each loop adds the current minutes interval
            // Checks that the task is overdue, complete it to increment the task to the next due time, without succeeding in a streak worthy completion
            // Increment the interval time by one hour
            // At completion, no streak shoud be created, but all tasks have been completed (late) up to the current hour
            for (int i = 1; i <= testLimit; i++)
            {
                habitualTask.FakeDateTime = now.AddMinutes(minutesFromNow);
                Debug.WriteLine($"Comparing task due date: {habitualTask.DueDate} against current fake date: {habitualTask.FakeDateTime}");
                
                // Complete task after Due Time to ensure that a streak is not started
                Assert.True(habitualTask.Overdue); 
                habitualTask = habitualTask.WithCompleted(true);
                Assert.False(habitualTask.Overdue);
                
                minutesFromNow += intervalInMinutes;
                Debug.WriteLine($"!!! Habitual Task {habitualTask.DueDate} (is overdue: {habitualTask.Overdue}, current streak is {habitualTask.Streak}), Repitions={habitualTask.Repititions}");
            }

            // Should not have a streak at this point
            Assert.Equal(0, habitualTask.Streak);

            minutesFromNow -= intervalInMinutes + (intervalInMinutes/2); // Set the clock back to ensure that the next task is completed within the due time
            habitualTask.FakeDateTime = now.AddMinutes(minutesFromNow);

            Assert.False(habitualTask.Overdue); // the task should not be overdue with the time setback above
            Debug.WriteLine($"Habitual Task {habitualTask.DueDate} (is overdue: {habitualTask.Overdue}, current streak is {habitualTask.Streak})");

            // Loop to testLimit.
            // Each loop will mark the task completed within the duedate, counting as a successful addition to the streak
            // At the end of the loop, the interval will be incremented by 1 hour for the next task iteration
            for (int i = 1; i <= testLimit; i++)
            {
                habitualTask.FakeDateTime = now.AddMinutes(minutesFromNow);
                habitualTask = habitualTask.WithCompleted(true);
                Assert.False(habitualTask.Overdue);
                Debug.WriteLine($"!!! Habitual Task {habitualTask.DueDate} (is overdue: {habitualTask.Overdue}, current streak is {habitualTask.Streak}), Repitions={habitualTask.Repititions}");
                minutesFromNow += intervalInMinutes;
            }

            Assert.Equal(testLimit, habitualTask.Streak); // A streak of *testLimit* variable will be achieved here

            minutesFromNow += intervalInMinutes + (intervalInMinutes / 2); // Add on an amount of time  to ensure that the next task is not completed within the due date (ending the streak)
            habitualTask.FakeDateTime = now.AddMinutes(minutesFromNow);

            Assert.True(habitualTask.Overdue); // The task should be overdue (which will end the streak)
            
            habitualTask = habitualTask.WithCompleted(true);
            Assert.False(habitualTask.Overdue); // Check that the task is completed, and no longer overdue
            
            Debug.WriteLine($"Habitual Task {habitualTask.DueDate} (is overdue: {habitualTask.Overdue}, current streak is {habitualTask.Streak}), Repitions={habitualTask.Repititions}");

            Assert.Equal(0, habitualTask.Streak); // Check that the task streak is over, now back to 0.
        }
    }
}

