using TaskManagerCore.Configuration;

namespace TaskManagerCore.Infrastructure.BinaryFile.Entity
{
    internal class TaskFolderEntity : EntityBase, IComparable<TaskFolderEntity>, ISearchable
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

            // add sorting for task count
        }

        /// <summary>
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public static int CompareTasksByName(TaskFolderEntity t1, TaskFolderEntity t2)
        {
            return string.Compare(t1.Name, t2.Name, StringComparison.OrdinalIgnoreCase);
        }

        public static int CompareTasksByTaskCount(TaskFolderEntity t1, TaskFolderEntity t2)
        {
            return t1.TaskIds.Count.CompareTo(t2.TaskIds.Count);
        }

        public override string ToString()
        {
            return $"TaskFolderEntity: [Name={Name}]";
        }

        public string ToString_ValuesOnly()
        {
            return Name;
        }
    }
}
