
namespace TaskManagerCore.Model.Dto
{
    public class GetTaskDto
    {
        /// <value>
        /// The type of the task.
        /// </value>
        public TaskType Type { get; }

        /// <value>
        /// The unique identifier of the task.
        /// </value>
        public string Id { get; }

        /// <value>
        /// The description of the task.
        /// </value>
        public string Description { get; }

        /// <value>
        /// The additional notes associated with the task.
        /// </value>
        public string Notes { get; }

        /// <value>
        /// A value indicating whether the task is completed.
        /// </value>
        public bool Completed { get; }

        /// <value>
        /// The due date of the task, if any.
        /// </value>
        public DateTime? DueDate { get; }

        /// <value>
        /// A value indicating whether the task is overdue.
        /// </value>
        public bool Overdue { get; }

        /// <value>
        /// The identifier of the folder to which the task belongs.
        /// </value>
        public string? InFolderId { get; }

        /// <value>
        /// The additional data associated with the task, stored in a dictionary.
        /// </value>
        public Dictionary<string, string> XData
        {
            get => new Dictionary<string, string>(_xData);
        }

        private readonly Dictionary<string, string> _xData;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetTaskDto"/> class with the specified parameters.
        /// </summary>
        /// <param name="type">The type of the task.</param>
        /// <param name="id">The unique identifier of the task.</param>
        /// <param name="description">The description of the task.</param>
        /// <param name="notes">The additional notes associated with the task.</param>
        /// <param name="completed">A value indicating whether the task is completed.</param>
        /// <param name="dueDate">The due date of the task, if any.</param>
        /// <param name="overdue">A value indicating whether the task is overdue.</param>
        /// <param name="folderId">The identifier of the folder to which the task belongs.</param>
        public GetTaskDto(TaskType type,
                            string id,
                            string description,
                            string notes,
                            bool completed,
                            DateTime? dueDate = null,
                            bool overdue = false,
                            string? folderId = null)
        {
            Type = type;
            Id = id;
            Description = description;
            Notes = notes;
            Completed = completed;
            DueDate = dueDate;
            Overdue = overdue;
            InFolderId = folderId;
            _xData = new Dictionary<string, string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetTaskDto"/> class with the specified parameters, including additional data.
        /// </summary>
        /// <param name="type">The type of the task.</param>
        /// <param name="id">The unique identifier of the task.</param>
        /// <param name="description">The description of the task.</param>
        /// <param name="notes">The additional notes associated with the task.</param>
        /// <param name="completed">A value indicating whether the task is completed.</param>
        /// <param name="dueDate">The due date of the task, if any.</param>
        /// <param name="overdue">A value indicating whether the task is overdue.</param>
        /// <param name="folderId">The identifier of the folder to which the task belongs.</param>
        /// <param name="xData">The additional data associated with the task, stored in a dictionary.</param>
        private GetTaskDto(TaskType type,
                    string id,
                    string description,
                    string notes,
                    bool completed,
                    DateTime? dueDate,
                    bool overdue,
                    string? folderId,
                    Dictionary<string, string> xData)
            : this(type, id, description, notes, completed, dueDate, overdue, folderId)
        {
            _xData = xData;
        }

        /// <summary>
        /// Creates a new instance of <see cref="GetTaskDto"/> with additional data.
        /// </summary>
        /// <param name="xData">The additional data associated with <see cref="RepeatingTaskData"/> and <see cref="HabitualTaskData"/></param>
        /// <returns>A new instance of <see cref="GetTaskDto"/> with the specified additional data.</returns>
        public GetTaskDto WithExtraData(Dictionary<string, string> xData)
        {
            return new GetTaskDto(Type, Id, Description, Notes, Completed, DueDate, Overdue, InFolderId, xData);
        }

        /// <summary>
        /// Adds a key-value pair of extra data to the task.
        /// </summary>
        /// <param name="key">The key of the extra data.</param>
        /// <param name="value">The value of the extra data.</param>
        public void AddExtraData(string key, string value)
        {
            _xData.Add(key, value);
        }

        /// <summary>
        /// Adds multiple key-value pairs of extra data to the task.
        /// </summary>
        /// <param name="xData">The dictionary containing the additional data to add.</param>
        public void AddExtraData(Dictionary<string, string> xData)
        {
            foreach (var pair in xData)
            {
                _xData.Add(pair.Key, pair.Value);
            }
        }
    }
}
