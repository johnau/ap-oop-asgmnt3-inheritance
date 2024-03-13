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
            if (other == null) return 1;

            return string.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);
        }

        public string GetTextStringForSearch()
        {
            return Name;
        }
    }
}
