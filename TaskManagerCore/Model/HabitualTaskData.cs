namespace TaskManagerCore.Model
{
    public class HabitualTaskData : RepeatingTaskData
    {
        public int Streak { get; }

        public HabitualTaskData(string description, string notes, DateTime dueDate, TimeInterval interval) 
            : base(description, notes, dueDate, interval)
        {
        }

        public HabitualTaskData(string id, string description, string notes, bool completed, DateTime dueDate, TimeInterval interval, int repititions, int streak)
            : base(id, description, notes, completed, dueDate, interval, repititions)
        {
            Streak = streak;
        }

        public override HabitualTaskData WithCompleted(bool value)
        {
            if (value == false) // || DueDate == null)
            {
                return this;
            }
            // increment streak or reset
            var nextDueDate = NextDueDate();
            Console.WriteLine($"Next Due Date (calculated) = {nextDueDate} and ComparisonTime = {ComparisonTime()}");
            if (ComparisonTime() <= nextDueDate)
            {
                return new HabitualTaskData(Id, Description, Notes, false, nextDueDate, RepeatingInterval, Repititions + 1, Streak + 1);
            }

            // Reset the streak to zero if we have completed it outside of the due date
            return new HabitualTaskData(Id, Description, Notes, false, nextDueDate, RepeatingInterval, Repititions + 1, 0);
        }
    }
}
