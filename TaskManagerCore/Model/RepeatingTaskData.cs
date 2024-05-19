using System;
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

        #region Builder methods
        /// <summary>
        /// Returns a new instance of <see cref="TaskData"/> with the specified description.
        /// </summary>
        /// <param name="value">The new description for the task.</param>
        /// <returns>A new <see cref="TaskData"/> instance with the updated description.</returns>
        public override TaskData WithDescription(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value), "The Description should never be null or empty in a TaskData object");

            return new RepeatingTaskData(Id, value, Notes, Completed, DueDate, RepeatingInterval, Repetitions);
        }

        /// <summary>
        /// Returns a new instance of <see cref="TaskData"/> with the specified notes.
        /// </summary>
        /// <param name="value">The new notes for the task.</param>
        /// <returns>A new <see cref="TaskData"/> instance with the updated notes.</returns>
        public override TaskData WithNotes(string value)
        {
            if (value == null)
                value = string.Empty;

            return new RepeatingTaskData(Id, Description, value, Completed, DueDate, RepeatingInterval, Repetitions);
        }

        /// <summary>
        /// Returns a new instance of <see cref="TaskData"/> with the specified due date.
        /// </summary>
        /// <param name="value">The new due date for the task.</param>
        /// <returns>A new <see cref="TaskData"/> instance with the updated due date.</returns>
        public override TaskData WithDueDate(DateTime? value)
        {
            if (value == null || !value.HasValue)
                throw new ArgumentNullException(nameof(value), "The Due Date should never be null in a RepeatingTaskData object");

            return new RepeatingTaskData(Id, Description, Notes, Completed, value.Value, RepeatingInterval, Repetitions);
        }

        /// <summary>
        /// Updates the task data to indicate completion and adjusts repetitions and due date for the next repetition.
        /// </summary>
        /// <param name="value">A value indicating whether the task is completed.</param>
        /// <returns>A new instance of <see cref="RepeatingTaskData"/> with the specified completion status and updated repetitions and due date.</returns>
        public override TaskData WithCompleted(bool value)
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

        #endregion
    }
}
