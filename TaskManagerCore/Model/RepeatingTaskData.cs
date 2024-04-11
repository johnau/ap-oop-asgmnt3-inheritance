using System.Diagnostics;

namespace TaskManagerCore.Model
{
    /// <summary>
    /// Represents task data for a repeating task.
    /// </summary>
    public class RepeatingTaskData : TaskData
    {
        /// <value>
        /// The due date of the repeating task.
        /// </value>
        /// <remarks>
        /// The override is to remove the nullable property, as Repeating tasks must have a due date.
        /// </remarks>
        public new DateTime DueDate { get; }
        /// <value>
        /// The time interval for the repeating task.
        /// </value>
        public TimeInterval RepeatingInterval { get; }
        /// <value>
        /// The number of repetitions completed for the repeating task.
        /// </value>
        public int Repetitions { get; }

        #region constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="RepeatingTaskData"/> class with the specified parameters.
        /// </summary>
        /// <param name="description">The description of the task.</param>
        /// <param name="notes">The notes related to the task.</param>
        /// <param name="dueDate">The due date of the task.</param>
        /// <param name="interval">The time interval for the task.</param>
        public RepeatingTaskData(string description, string notes, DateTime dueDate, TimeInterval interval)
            : base(description, notes, dueDate)
        {
            DueDate = dueDate; // must re-assign dueDate here since we overwrote the member in the base class
            RepeatingInterval = interval;
            //StartFrom = dueDate;
            Repetitions = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepeatingTaskData"/> class with the specified parameters.
        /// </summary>
        /// <param name="id">The unique identifier of the task.</param>
        /// <param name="description">The description of the task.</param>
        /// <param name="notes">The notes related to the task.</param>
        /// <param name="completed">A value indicating whether the task is completed.</param>
        /// <param name="dueDate">The due date of the task.</param>
        /// <param name="interval">The time interval for the task.</param>
        /// <param name="repititions">The number of repetitions completed for the task.</param>
        public RepeatingTaskData(string id, string description, string notes, bool completed, DateTime dueDate, TimeInterval interval, int repititions)
            : base(id, description, notes, completed, dueDate)
        {
            DueDate = dueDate;
            RepeatingInterval = interval;
            //StartFrom = startFrom;
            Repetitions = repititions;
        }
        #endregion

        /// <summary>
        /// Updates the task data to indicate completion and adjusts repetitions and due date for the next repetition.
        /// </summary>
        /// <param name="value">A value indicating whether the task is completed.</param>
        /// <returns>A new instance of <see cref="RepeatingTaskData"/> with the specified completion status and updated repetitions and due date.</returns>
        public override RepeatingTaskData WithCompleted(bool value)
        {
            if (value == false) // || DueDate == null)
            {
                return this;
            }

            // Increment repititions and due date for the next reptition
            // (Would you want this to go to the next possible due date, or just stick to the intervals. i.e. If a daily task is completed 2 days late, does the next due time become yesterday, or today/tomorrow)
            var nextDueDate = DueDate.AddHours((int)RepeatingInterval);
            return new RepeatingTaskData(Id, Description, Notes, false, nextDueDate, RepeatingInterval, Repetitions + 1);
        }
    }
}
