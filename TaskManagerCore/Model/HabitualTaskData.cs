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

        /// <summary>
        /// Refactored the method to call the base method from the RepeatingTaskData, in this case a debatable action, but for inheritance exercise is appropriate?
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override HabitualTaskData WithCompleted(bool value)
        {
            if (value == false)
            {
                return this;
            }

            var _ = base.WithCompleted(value); // use the repeating task method to increment DueDate and Repititions

            var newStreak = ComparisonTime() <= DueDate ? Streak + 1 : 0;

            return new HabitualTaskData(_.Id, _.Description, _.Notes, _.Overdue, _.DueDate, _.RepeatingInterval, _.Repititions, newStreak);

            ////var nextDueDate = NextDueDate();
            //// Increment Streak if completed within the due time.
            //if (comparisonTime <= DueDate)
            //{
            //    return new HabitualTaskData(Id, Description, Notes, false, nextDueDate, RepeatingInterval, Repititions + 1, Streak + 1);
            //}

            //// Reset the streak to zero if we have completed it outside of the due date.
            //return new HabitualTaskData(Id, Description, Notes, false, nextDueDate, RepeatingInterval, Repititions + 1, 0);
        }
    }
}
