using TaskManagerCore.Configuration;

namespace TaskManagerCore.Infrastructure.BinaryFile.Entity
{
    internal class TaskFolderEntity : EntityBase, IComparable<TaskFolderEntity>, ITextSearchable
    {
        public string Name { get; set; }
        public List<string> TaskIds { get; set; } // Uni-directional one-to-many relationship

        public TaskFolderEntity(string? id = "")
            : base(id)
        {
            Name = "";
            TaskIds = new List<string>();
        }

        public int CompareTo(TaskFolderEntity? other)
        {
            throw new NotImplementedException();
        }

        public string GetTextStringForSearch()
        {
            throw new NotImplementedException();
        }
    }
}
