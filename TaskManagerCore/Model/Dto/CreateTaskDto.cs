namespace TaskManagerCore.Model.Dto
{
    /// <summary>
    /// Data Transfer Object (DTO) representing a new task.
    /// </summary>
    public class CreateTaskDto
    {
        /// <value>
        /// The identifier for the <see cref="TaskFolder"/> the new <see cref="TaskData"/> will be created in.
        /// </value>
        /// <remarks>
        /// <para>This can be <c>TaskFolder.<see cref="TaskFolder.Id"/></c> or <c>TaskFolder.<see cref="TaskFolder.Name"/></c> as both are unique in the database.</para>
        /// </remarks>
        public string InFolderId { get; }

        /// <value>
        /// The type of task that is being created, from the <see cref="TaskType"/> enum.
        /// </value>
        public TaskType TaskType { get; }

        /// <value>
        /// The description of the new task being created.
        /// </value>
        /// <remarks>
        /// * A <c>Description</c> must be provided for a new Task.
        /// </remarks>
        public string Description { get; }

        /// <value>
        /// Notes for the new task being created.
        /// </value>
        public string Notes { get; }

        /// <value>
        /// Due Date for the task being created.
        /// </value>
        /// <remarks>
        /// <list type="bullet">
        /// <item>A Due Date is not required for a Regular/<see cref="TaskType.SINGLE"/> Task</item>
        /// <item>A Due Date in the past is accepted</item>
        /// <item>A Due Date is required for a <see cref="TaskType.REPEATING"/> and Habitual/<see cref="TaskType.REPEATING_STREAK"/> Task</item>
        /// </list>
        /// </remarks>
        public DateTime? DueDate { get; }

        /// <value>
        /// Repeating interval for the task, if applicable.
        /// </value>
        /// <remarks>
        /// <para>The <see cref="TimeInterval"/> enum uses <see cref="int"/> values based on hour value.</para>
        /// The following <see cref="TaskType"/>'s require a <c>RepeatInterval</c>
        /// <list type="table">
        /// <item>
        /// <taskType><see cref="TaskType.REPEATING"/></taskType><class><see cref="RepeatingTaskData"/></class>
        /// </item>
        /// <item>
        /// <taskType><see cref="TaskType.REPEATING_STREAK"/></taskType><class><see cref="HabitualTaskData"/></class>
        /// </item>
        /// </list>
        /// </remarks>
        public TimeInterval? RepeatInterval { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateTaskDto"/> class with a <see cref="TaskType"/> to specify type.
        /// </summary>
        /// <param name="type">The task type (<see cref="TaskType.SINGLE"/>, <see cref="TaskType.REPEATING"/>, or <see cref="TaskType.REPEATING_STREAK"/>)</param>
        /// <param name="folderId">The parent folder for the task (Folder <see cref="TaskFolder.Id"/> or <see cref="TaskFolder.Name"/>)</param>
        /// <param name="description">The task description</param>
        /// <param name="notes">Additional notes for the task.</param>
        /// <param name="dueDate">The due date of the task.</param>
        /// <param name="interval">The repeating interval for the task.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="folderId"/> or <paramref name="description"/> is <see langword="null"/>.</exception>
        public CreateTaskDto(TaskType type, string folderId, string description, string notes, DateTime? dueDate = null, TimeInterval interval = TimeInterval.None)
        {
            TaskType = type;
            InFolderId = folderId ?? throw new ArgumentNullException(nameof(folderId), "Folder ID cannot be null.");
            Description = description ?? throw new ArgumentNullException(nameof(description), "Description cannot be null.");
            Notes = notes ?? string.Empty;
            DueDate = dueDate;
            RepeatInterval = interval;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateTaskDto"/> class without a TaskType specified.
        /// </summary>
        /// <param name="folderId">The ID of the folder in which the task will be created.</param>
        /// <param name="description">The description of the task.</param>
        /// <param name="notes">Additional notes for the task.</param>
        /// <param name="dueDate">The due date of the task. (Optional)</param>
        /// <param name="interval">The repeat interval for the task. (Optional)</param>
        /// <param name="trackStreaks">A boolean value indicating whether to track streaks for the task. (Optional)</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="folderId"/> or <paramref name="description"/> is <see langword="null"/>.</exception>
        public CreateTaskDto(string folderId, string description, string notes, DateTime? dueDate = null, TimeInterval interval = TimeInterval.None, bool trackStreaks = false)
        {
            if (interval == TimeInterval.None && !trackStreaks)
            {
                TaskType = TaskType.SINGLE;
            }
            else if (interval != TimeInterval.None && !trackStreaks)
            {
                TaskType = TaskType.REPEATING;
            }
            else if (interval != TimeInterval.None && trackStreaks)
            {
                TaskType = TaskType.REPEATING_STREAK;
            }

            InFolderId = folderId ?? throw new ArgumentNullException(nameof(folderId), "Folder ID cannot be null.");
            Description = description ?? throw new ArgumentNullException(nameof(description), "Description cannot be null.");
            Notes = notes ?? string.Empty;
            DueDate = dueDate;
            RepeatInterval = interval;
        }
    }
}
