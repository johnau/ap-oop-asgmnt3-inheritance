using System.Formats.Asn1;

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

        // TaskType could be removed, and replaced by a bool (`trackStreaks`).
        // - If TimeInerval IS `None` and `trackStreaks` is FALSE, its a regular task
        // - If TimeInerval IS NOT `None` and `trackStreaks` is FALSE, its a repeating task
        // - If TimeInerval IS NOT `None` and `trackStreaks` is TRUE, its a habitual task
        public CreateTaskDto(TaskType type, string folderId, string description, string notes, DateTime? dueDate = null, TimeInterval interval = TimeInterval.None)
        {
            TaskType = type;
            InFolderId = folderId;
            Description = description;
            Notes = notes;
            DueDate = dueDate;
            RepeatInterval = interval;
        }

        public CreateTaskDto(string folderId, string description, string notes, DateTime? dueDate = null, TimeInterval interval = TimeInterval.None, bool trackStreaks = false)
        {
            if (interval == TimeInterval.None  && !trackStreaks)
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
            
            InFolderId = folderId;
            Description = description;
            Notes = notes;
            DueDate = dueDate;
            RepeatInterval = interval;
        }
    }
}
