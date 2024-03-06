
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

        GetTaskDto(TaskType type,
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

        public GetTaskDto WithExtraData(Dictionary<string, string> xData)
        {
            return new GetTaskDto(Type, Id, Description, Notes, Completed, DueDate, Overdue, InFolderId, xData);
        }

        public void AddExtraData(string key, string value) { 
            _xData.Add(key, value);
        }

        public void AddExtraData(Dictionary<string, string> xData)
        {
            foreach (var _ in xData) 
            { 
                _xData.Add(_.Key, _.Value);
            }
        }

        //public GetTaskDto(TaskType type,
        //            string id,
        //            string description,
        //            string notes,
        //            bool completed,
        //            DateTime dueDate,
        //            bool overdue = false,
        //            string? folderId = null,
        //            Dictionary<string, string>? xData = null)
        //    : this(type, id, description, notes, completed, dueDate, overdue, folderId)
        //{
        //    //Type = type;
        //    //Id = id;
        //    //Description = description;
        //    //Notes = notes;
        //    //Completed = completed;
        //    //DueDate = dueDate;
        //    //Overdue = overdue;
        //    //InFolderId = folderId;
        //    if (xData != null)
        //    {
        //        _xData = xData;
        //    } else
        //    {
        //        _xData = new Dictionary<string, string>();
        //    }
        //}
    }
}
