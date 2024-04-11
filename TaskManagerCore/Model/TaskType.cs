namespace TaskManagerCore.Model
{
    /// <summary>
    /// Represents the type of a task.
    /// </summary>
    public enum TaskType
    {
        /// <summary>
        /// Indicates a single task.
        /// </summary>
        SINGLE = 1,

        /// <summary>
        /// Indicates a repeating task.
        /// </summary>
        REPEATING = 2,

        /// <summary>
        /// Indicates a repeating task with streak tracking.
        /// </summary>
        REPEATING_STREAK = 3,
    }
}
