namespace TaskManagerCore.Model.Dto
{
    public class CreateTaskDto
    {
        public string InFolderId { get; }

        public TaskType TaskType { get; }

        public string Description { get; }

        public string Notes { get; }

        public DateTime? DueDate { get; }

        public TimeInterval? RepeatInterval { get; }

        public CreateTaskDto(TaskType type, string folderId, string description, string notes, DateTime? dueDate = null)
        {
            TaskType = type;
            InFolderId = folderId;
            Description = description;
            Notes = notes;
            DueDate = dueDate;
        }

        public CreateTaskDto(TaskType type, string folderId, string description, string notes, DateTime dueDate, TimeInterval interval)
        {
            TaskType = type;
            InFolderId = folderId;
            Description = description;
            Notes = notes;
            RepeatInterval = interval;
            DueDate = dueDate;
        }
    }
}
