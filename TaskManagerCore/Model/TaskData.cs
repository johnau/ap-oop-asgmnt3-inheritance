namespace TaskManagerCore.Model
{
    /// <summary>
    /// Represents the base class for task data.
    /// </summary>
    public class TaskData //: ITaskMangeable
    {
        /// <value>
        /// The unique identifier of the task.
        /// </value>
        public string Id { get; }

        /// <value>
        /// The description of the task.
        /// </value>
        public string Description { get; }

        /// <value>
        /// Additional notes related to the task.
        /// </value>
        public string Notes { get; }

        /// <value>
        /// Indicates whether the task is completed.
        /// </value>
        public bool Completed { get; }

        /// <value>
        /// The due date of the task.
        /// </value>
        public DateTime? DueDate { get; }

        /// <value>
        /// Indicates whether the task is overdue.
        /// </value>
        public bool Overdue { get => IsOverdue(); }

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskData"/> class with the specified description.
        /// </summary>
        /// <param name="description">The description of the task.</param>
        public TaskData(string description)
        {
            Id = "";
            Description = description;
            Notes = "";
            Completed = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskData"/> class with the specified parameters.
        /// </summary>
        /// <param name="description">The description of the task.</param>
        /// <param name="notes">Additional notes related to the task.</param>
        /// <param name="dueDate">The due date of the task.</param>
        public TaskData(string description, string notes, DateTime? dueDate = null)
            : this(description)
        {
            Id = "";
            Description = description;
            Notes = notes;
            DueDate = dueDate;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskData"/> class with the specified parameters.
        /// </summary>
        /// <param name="id">The unique identifier of the task.</param>
        /// <param name="description">The description of the task.</param>
        /// <param name="notes">Additional notes related to the task.</param>
        /// <param name="completed">Indicates whether the task is completed.</param>
        /// <param name="dueDate">The due date of the task.</param>
        public TaskData(string id, string description, string notes, bool completed, DateTime? dueDate)
        {
            Id = id;
            Description = description;
            Notes = notes;
            Completed = completed;
            DueDate = dueDate;
        }

        #endregion
        /// <summary>
        /// Calculates if the task is overdue.
        /// </summary>
        /// <returns>True if the task is overdue; otherwise, false.</returns>
        public virtual bool IsOverdue() => DueDate != null && !Completed && ComparisonTime() > DueDate;

        #region Builder methods
        /// <summary>
        /// Returns a new instance of <see cref="TaskData"/> with the specified description.
        /// </summary>
        /// <param name="value">The new description for the task.</param>
        /// <returns>A new <see cref="TaskData"/> instance with the updated description.</returns>
        public virtual TaskData WithDescription(string value)
        {
            //value ??= string.Empty; // compound assignment
            if (value == null)
            {
                value = string.Empty;
            }
            return new TaskData(Id, value, Notes, Completed, DueDate);
        }

        /// <summary>
        /// Returns a new instance of <see cref="TaskData"/> with the specified notes.
        /// </summary>
        /// <param name="value">The new notes for the task.</param>
        /// <returns>A new <see cref="TaskData"/> instance with the updated notes.</returns>
        public virtual TaskData WithNotes(string value)
        {
            //value ??= string.Empty; // compound assignment
            if (value == null)
            {
                value = string.Empty;
            }
            return new TaskData(Id, Description, value, Completed, DueDate);
        }

        /// <summary>
        /// Returns a new instance of <see cref="TaskData"/> with the specified completion status.
        /// </summary>
        /// <param name="value">The new completion status for the task.</param>
        /// <returns>A new <see cref="TaskData"/> instance with the updated completion status.</returns>
        public virtual TaskData WithCompleted(bool value)
        {
            return new TaskData(Id, Description, Notes, value, DueDate);
        }

        /// <summary>
        /// Returns a new instance of <see cref="TaskData"/> with the specified due date.
        /// </summary>
        /// <param name="value">The new due date for the task.</param>
        /// <returns>A new <see cref="TaskData"/> instance with the updated due date.</returns>
        public virtual TaskData WithDueDate(DateTime? value)
        {
            return new TaskData(Id, Description, Notes, Completed, value);
        }

        #endregion

        /// <summary>
        /// Protected method that can be overridden for tests by extending the class.
        /// </summary>
        /// <returns>The comparison time used for checking overdue status.</returns>
        protected virtual DateTime ComparisonTime()
        {
            return DateTime.Now;
        }
    }
}
