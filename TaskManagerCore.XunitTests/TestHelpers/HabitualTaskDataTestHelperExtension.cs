using System;
using System.Diagnostics;
using TaskManagerCore.Model;

namespace TaskManagerCore.XunitTests.TestHelpers
{
    /// <summary>
    /// Test Wrapper Class
    /// Would be better to refactor the data class a lightweight struct and use an accessor class to mutate (A check on overdue may include a mutation)
    /// This would allow for a slightly nicer test setup? This setup is not the nicest for tests
    /// </summary>
    internal class HabitualTaskDataTestHelperExtension : HabitualTaskData
    {
        public DateTime FakeDateTime { get; set; }

        public HabitualTaskDataTestHelperExtension(string description, string notes, DateTime dueDate, TimeInterval interval) 
            : base(description, notes, dueDate, interval)
        {
            FakeDateTime = DateTime.Now;
        }

        public HabitualTaskDataTestHelperExtension(string id, string description, string notes, bool completed, DateTime dueDate, TimeInterval interval, int repititions, int streak) 
            : base(id, description, notes, completed, dueDate, interval, repititions, streak)
        {
            FakeDateTime = DateTime.MinValue;
        }

        /// <summary>
        /// Convenience method to set a specific date
        /// </summary>
        /// <param name="dateString"></param>
        public void SetFakeDateTimeByString(string dateString)
        {
            FakeDateTime = DateTime.Parse(dateString);
        }

        public new HabitualTaskDataTestHelperExtension WithCompleted(bool value)
        {
            var completed = base.WithCompleted(value);

            return new HabitualTaskDataTestHelperExtension(completed.Id, completed.Description, completed.Notes, false, completed.DueDate, completed.RepeatingInterval, completed.Repetitions, completed.Streak) 
            { 
                FakeDateTime = FakeDateTime
            };
        }

        protected override DateTime ComparisonTime()
        {
            Debug.WriteLine($"Returning FakeDateTime for tests: {FakeDateTime}");
            return FakeDateTime;
        }
    }
}
