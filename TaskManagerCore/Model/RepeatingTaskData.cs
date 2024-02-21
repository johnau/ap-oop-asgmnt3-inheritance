namespace TaskManagerCore.Model
{
    public class RepeatingTaskData : TaskData
    {
        public TimeInterval RepeatingInterval { get; }
        //public DateTime StartFrom { get; }  // could drop StartFrom, use DueDate don't change it's value, but change override the getter to return duedate + interval*repeats...
        public int Repititions { get; }
        public new DateTime DueDate { get; }

        #region constructors
        public RepeatingTaskData(string description, string notes, DateTime dueDate, TimeInterval interval)
            : base(description, notes, dueDate)
        {
            RepeatingInterval = interval;
            DueDate = dueDate; // must re-assing dueDate here since we overwrote the member in the base class
            //StartFrom = dueDate;
            Repititions = 0;
        }

        public RepeatingTaskData(string id, string description, string notes, bool completed, DateTime dueDate, TimeInterval interval, int repititions)
            : base(id, description, notes, completed, dueDate)
        {
            RepeatingInterval = interval;
            DueDate = dueDate;
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
            //var hours = (int)RepeatingInterval;
            //if (DueDate == null)
            //{
            //    return DateTime.Now; // This should never occur
            //}
            return DueDate.AddHours((int)RepeatingInterval);
        }


    }
}
