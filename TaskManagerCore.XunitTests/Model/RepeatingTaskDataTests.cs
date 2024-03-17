using System.Diagnostics;
using TaskManagerCore.Model;
using TaskManagerCore.XunitTests.TestHelpers;

namespace TaskManagerCore.XunitTests.Model
{
    public class RepeatingTaskDataTests
    {
        /// <summary>
        /// Fully tests the Repeating Task
        /// Each time a task is completed, the due date is shifted forward to the next interval
        /// </summary>
        [Theory]
        [InlineData(TimeInterval.Hourly)]
        [InlineData(TimeInterval.Daily)]
        [InlineData(TimeInterval.Weekly)]
        [InlineData(TimeInterval.Monthly)]
        [InlineData(TimeInterval.Yearly)]
        public void RepeatingTask_TestStreakBehavior(TimeInterval timeInterval)
        {
            var testLimit = 5;
            var intervalInMinutes = (int)timeInterval * 60;
            var now = DateTime.Now;
            var repeatingTask = new RepeatingTaskDataTestHelperExtension("Repeating task description", "", now.AddHours(1), timeInterval); // start with a task due in an hour
            var minutesFromNow = intervalInMinutes + 1; // 1 minute offset to ensure we start overdue.

            Debug.WriteLine($"Habitual Task Due @ {repeatingTask.DueDate} (is overdue: {repeatingTask.Overdue}), current Repititions={repeatingTask.Repetitions}");

            // Loop - Complete while not overdue
            for (int i = 1; i <= testLimit; i++)
            {
                // Complete task before due time
                Debug.WriteLine($"Comparing task due date: {repeatingTask.DueDate} against current fake date: {repeatingTask.FakeDateTime}");

                Assert.False(repeatingTask.Overdue);
                repeatingTask = repeatingTask.WithCompleted(true);

                repeatingTask.FakeDateTime = now.AddMinutes(minutesFromNow);
                Assert.False(repeatingTask.Overdue);

                minutesFromNow += intervalInMinutes;
                Debug.WriteLine($"Habitual Task Due @ {repeatingTask.DueDate} (is overdue: {repeatingTask.Overdue}), current Repititions={repeatingTask.Repetitions}");
            }

            Assert.Equal(testLimit, repeatingTask.Repetitions);
            Assert.False(repeatingTask.Overdue);

            // Start next overloop while overdue
            repeatingTask.FakeDateTime = now.AddMinutes(minutesFromNow);
            Assert.True(repeatingTask.Overdue);

            // Loop - Completing while overdue
            for (int i = 1; i <= testLimit; i++)
            {
                // Complete task before due time
                Debug.WriteLine($"Comparing task due date: {repeatingTask.DueDate} against current fake date: {repeatingTask.FakeDateTime}");

                Assert.True(repeatingTask.Overdue);
                repeatingTask = repeatingTask.WithCompleted(true);

                Assert.False(repeatingTask.Overdue);

                minutesFromNow += intervalInMinutes;
                repeatingTask.FakeDateTime = now.AddMinutes(minutesFromNow);
                Debug.WriteLine($"Habitual Task Due @ {repeatingTask.DueDate} (is overdue: {repeatingTask.Overdue}), current Repititions={repeatingTask.Repetitions}");
            }

            // Check that we have successfully incremented the task 
            Assert.Equal(testLimit * 2, repeatingTask.Repetitions);
        }

        [Fact]
        public void CheckOverdueHourlyTwice_WithCompleted_WillNotBeOverdue()
        {
            var now = DateTime.Now;
            var repeatingTask = new RepeatingTaskDataTestHelperExtension("Repeating task description", "", now.AddHours(1), TimeInterval.Hourly);

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
            var repeatingTask = new RepeatingTaskDataTestHelperExtension("Repeating task description", "", now.AddHours(1), TimeInterval.Hourly);

            Assert.False(repeatingTask.Overdue);

            repeatingTask.FakeDateTime = now.AddMinutes(61); // 1 min past old due date

            Assert.True(repeatingTask.Overdue);
        }

        [Fact]
        public void CheckOverdueHourly_WithCompleted_WillNotBeOverdue()
        {
            var now = DateTime.Now;
            var repeatingTask = new RepeatingTaskDataTestHelperExtension("Repeating task description", "", now.AddHours(1), TimeInterval.Hourly);

            Assert.False(repeatingTask.Overdue);

            repeatingTask = repeatingTask.WithCompleted(true);
            repeatingTask.FakeDateTime = now.AddMinutes(61); // 1 min past old due date

            Assert.False(repeatingTask.Overdue);
        }

        [Fact]
        public void CheckOverdueDaily_WithCompleted_WillNotBeOverdue()
        {
            var now = DateTime.Now;
            var repeatingTask = new RepeatingTaskDataTestHelperExtension("Repeating task description", "", now.AddHours(1), TimeInterval.Daily);

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
