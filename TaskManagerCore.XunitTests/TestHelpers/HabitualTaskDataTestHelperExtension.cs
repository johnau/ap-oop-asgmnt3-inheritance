using TaskManagerCore.Model;

namespace TaskManagerCore.XunitTests.TestHelpers
{
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
        { }

        /// <summary>
        /// Convenience method to set a specific date
        /// </summary>
        /// <param name="dateString"></param>
        public void SetFakeDateTimeByString(string dateString)
        {
            FakeDateTime = DateTime.Parse(dateString);
        }

        public override HabitualTaskDataTestHelperExtension WithCompleted(bool value)
        {
            var completed = base.WithCompleted(value);

            return new HabitualTaskDataTestHelperExtension(completed.Id, completed.Description, completed.Notes, false, completed.DueDate, completed.RepeatingInterval, completed.Repititions, completed.Streak);
        }

        protected override DateTime ComparisonTime()
        {
            Console.WriteLine($"Returning FakeDateTime for tests: {FakeDateTime}");
            return FakeDateTime;
        }
    }
}
