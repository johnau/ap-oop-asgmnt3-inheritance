
namespace TaskManagerCore.Model.Dto
{
    public class GetTaskDto
    {
        public TaskType Type { get; }

        public string Id { get; }

        public string Description { get; }

        public string Notes { get; }
        
        public bool Completed { get; }

        public DateTime? DueDate { get; }

        public bool Overdue { get; }

        public string? InFolderId { get; }

        private readonly Dictionary<string, string> _xData;
        public Dictionary<string, string> XData { 
            get => new Dictionary<string, string>(_xData); 
        }

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

        public GetTaskDto(TaskType type,
                    string id,
                    string description,
                    string notes,
                    bool completed,
                    DateTime dueDate,
                    Dictionary<string, string> xData,
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
            _xData = xData;
        }
    }
}
