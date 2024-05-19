using System;

namespace TaskManagerCore.Model
{
    /// <summary>
    /// Represents a habitual task, which extends the functionality of a repeating task by tracking streaks.
    /// </summary>
    public class HabitualTaskData : RepeatingTaskData
    {
        /// <value>
        /// The current streak of completing the task consecutively.
        /// </value>
        public int Streak { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HabitualTaskData"/> class with the specified properties.
        /// </summary>
        /// <param name="description">The description of the task.</param>
        /// <param name="notes">Additional notes related to the task.</param>
        /// <param name="dueDate">The due date of the task.</param>
        /// <param name="interval">The repeating interval of the task.</param>
        public HabitualTaskData(string description, string notes, DateTime dueDate, TimeInterval interval)
            : base(description, notes, dueDate, interval)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HabitualTaskData"/> class with the specified properties.
        /// </summary>
        /// <param name="id">The unique identifier of the task.</param>
        /// <param name="description">The description of the task.</param>
        /// <param name="notes">Additional notes related to the task.</param>
        /// <param name="completed">Indicates whether the task is completed.</param>
        /// <param name="dueDate">The due date of the task.</param>
        /// <param name="interval">The repeating interval of the task.</param>
        /// <param name="repititions">The number of repetitions of the task.</param>
        /// <param name="streak">The current streak of completing the task consecutively.</param>
        public HabitualTaskData(string id, string description, string notes, bool completed, DateTime dueDate, TimeInterval interval, int repititions, int streak)
            : base(id, description, notes, completed, dueDate, interval, repititions)
        {
            Streak = streak;
        }


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
        /// Overrides the base method to mark the task as completed and update the streak.
        /// </summary>
        /// <param name="value">The value indicating whether the task is completed.</param>
        /// <returns>A new <see cref="HabitualTaskData"/> object with the updated completion status and streak.</returns>
        public override TaskData WithCompleted(bool value)
        {
            if (value == false)
                return this;

            var _ = (RepeatingTaskData)base.WithCompleted(value); // use the repeating task method to increment DueDate and Repititions
            var newStreak = ComparisonTime() <= DueDate ? Streak + 1 : 0;

            return new HabitualTaskData(_.Id, _.Description, _.Notes, _.Overdue, _.DueDate, _.RepeatingInterval, _.Repetitions, newStreak);
        }

        #endregion
    }
}
