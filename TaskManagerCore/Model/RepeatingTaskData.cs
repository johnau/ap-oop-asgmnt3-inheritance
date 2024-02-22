namespace TaskManagerCore.Model
{
    public class RepeatingTaskData : TaskData
    {
        public new DateTime DueDate { get; }
        public TimeInterval RepeatingInterval { get; }
        //public DateTime StartFrom { get; }  // could drop StartFrom, use DueDate don't change it's value, but change override the getter to return duedate + interval*repeats...
        public int Repititions { get; }
        
        #region constructors
        public RepeatingTaskData(string description, string notes, DateTime dueDate, TimeInterval interval)
            : base(description, notes, dueDate)
        {
            DueDate = dueDate; // must re-assign dueDate here since we overwrote the member in the base class
            RepeatingInterval = interval;
            //StartFrom = dueDate;
            Repititions = 0;
        }

        public RepeatingTaskData(string id, string description, string notes, bool completed, DateTime dueDate, TimeInterval interval, int repititions)
            : base(id, description, notes, completed, dueDate)
        {
            DueDate = dueDate;
            RepeatingInterval = interval;
            //StartFrom = startFrom;
            Repititions = repititions;
        }
        #endregion

        /// <summary>
        /// Override the Overdue method to check against StartFrom + Interval * Repititions
        /// </summary>
        /// <returns></returns>
        public override bool IsOverdue()
        {
            //var hours = (int) RepeatingInterval * (Repititions + 1);
            return !Completed && ComparisonTime() > DueDate;
        }

        /// <summary>
        /// RepeatingTaskData.WithCompleted() method to increment Repititions and the DueDate,
        /// rather than setting Completed = true
        /// Also update the DueDate
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override RepeatingTaskData WithCompleted(bool value)
        {
            if (value == false) // || DueDate == null)
            {
                return this;
            }
            // never completes, just shifts to the next interval (repititions + 1) and updates the due date
            //var hoursTilNextDue = (int)RepeatingInterval * (Repititions + 1);
            //var newDueDate = StartFrom.AddHours(hoursTilNextDue);
            var nextDueDate = NextDueDate();
            return new RepeatingTaskData(Id, Description, Notes, false, nextDueDate, RepeatingInterval, Repititions + 1);
        }

        internal DateTime NextDueDate()
        {
            return DueDate.AddHours((int)RepeatingInterval);
        }
    }
}
