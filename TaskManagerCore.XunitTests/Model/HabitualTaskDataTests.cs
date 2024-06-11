using System;
using System.Diagnostics;
using TaskManagerCore.Model;
using TaskManagerCore.XunitTests.TestHelpers;
using Xunit;

namespace TaskManagerCore.XunitTests.Model
{
    public class HabitualTaskDataTests
    {
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
            var minutesFromNow = intervalInMinutes + 1; // + 1 minute beyond interval due time (i.e. Overdue by 1 minute)

            Debug.WriteLine($"Habitual Task Due @ {habitualTask.DueDate} (is overdue: {habitualTask.Overdue}, current streak is {habitualTask.Streak}), Repitions={habitualTask.Repetitions}");

            // Loop #1 - DO NOT CREATE STREAK
            // Task is Completed after it is Overdue - No Streak is started
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
                Debug.WriteLine($"Habitual Task {habitualTask.DueDate} (is overdue: {habitualTask.Overdue}, current streak is {habitualTask.Streak}), Repitions={habitualTask.Repetitions}");
            }

            // Should not have a streak at this point
            Assert.Equal(0, habitualTask.Streak);

            // Set the clock back to ensure that the next task is completed within the due time
            minutesFromNow -= intervalInMinutes + intervalInMinutes / 2;
            habitualTask.FakeDateTime = now.AddMinutes(minutesFromNow);
            Assert.False(habitualTask.Overdue);
            Debug.WriteLine($"Habitual Task {habitualTask.DueDate} (is overdue: {habitualTask.Overdue}, current streak is {habitualTask.Streak})");

            // Loop #2 - CREATE STREAK
            // Task is completed before it is Due, Streak is incremented
            for (int i = 1; i <= testLimit; i++)
            {
                // Complete task before Due Time to start or continue Streak
                habitualTask.FakeDateTime = now.AddMinutes(minutesFromNow);
                habitualTask = habitualTask.WithCompleted(true);
                Assert.False(habitualTask.Overdue);

                minutesFromNow += intervalInMinutes;
                Debug.WriteLine($"Habitual Task {habitualTask.DueDate} (is overdue: {habitualTask.Overdue}, current streak is {habitualTask.Streak}), Repitions={habitualTask.Repetitions}");
            }

            // Check we have the expected Streak
            Assert.Equal(testLimit, habitualTask.Streak);

            // Set the clock forward so we are Overdue - To end the streak
            minutesFromNow += intervalInMinutes + intervalInMinutes / 2;
            habitualTask.FakeDateTime = now.AddMinutes(minutesFromNow);
            Assert.True(habitualTask.Overdue);

            // Complete the task while Overdue
            habitualTask = habitualTask.WithCompleted(true);
            Assert.False(habitualTask.Overdue);

            // Confirm the Streak has been ended
            Assert.Equal(0, habitualTask.Streak);

            Debug.WriteLine($"Habitual Task {habitualTask.DueDate} (is overdue: {habitualTask.Overdue}, current streak is {habitualTask.Streak}), Repitions={habitualTask.Repetitions}");
            Debug.WriteLine("Habitual Task Test Complete");
        }

        [Fact]
        public void CheckOverdueHourlyTwice_WithCompleted_WillNotBeOverdue()
        {
            var now = DateTime.Now;
            var habitualTask = new HabitualTaskDataTestHelperExtension("Habitual task description", "", now.AddHours(1), TimeInterval.Hourly);

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
            var habitualTask = new HabitualTaskDataTestHelperExtension("Habitual task description", "", now.AddHours(1), TimeInterval.Hourly);
            Debug.WriteLine($"Habitual Task {habitualTask.DueDate} (is overdue: {habitualTask.Overdue})");
            var minutesFromNow = 30;

            habitualTask.FakeDateTime = now.AddMinutes(minutesFromNow);
            Assert.False(habitualTask.Overdue);

            for (int i = 1; i <= testLimit; i++)
            {
                habitualTask.FakeDateTime = now.AddMinutes(minutesFromNow - 1);
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

    }
}

