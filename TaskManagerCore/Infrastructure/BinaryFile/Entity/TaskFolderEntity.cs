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

        #region Static Compare Methods
        /// <summary>
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public static int CompareFoldersByName(TaskFolderEntity t1, TaskFolderEntity t2)
        {
            return string.Compare(t1.Name.ToLower(), t2.Name.ToLower());
        }

        public static int CompareFoldersByTaskCount(TaskFolderEntity t1, TaskFolderEntity t2)
        {
            return t1.TaskIds.Count.CompareTo(t2.TaskIds.Count);
        }
        #endregion



        public string ToValuesOnlyString()
        {
            return Name;
        }

        #region Static Helper Methods
        public static TaskFolderEntity BLANK => new TaskFolderEntity();
        #endregion

        public override string ToString()
        {
            return $"TaskFolderEntity: [ID={Id}, Name={Name}, Tasks Count={TaskIds.Count}]";
        }
    }
}
