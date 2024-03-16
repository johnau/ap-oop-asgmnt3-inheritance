using TaskManagerCore.Configuration;

namespace TaskManagerCore.Infrastructure.BinaryFile.Entity
{
    internal class TaskDataEntity : EntityBase, IComparable<TaskDataEntity> , ISearchable
    {
        public string Description { get; set; }

        public string Notes { get; set; }

        public bool Completed { get; set; }

        public DateTime? DueDate { get; set; }

        public TaskDataEntity(string? id = "")
            : base(id)
        {
            Description = "";
            Notes = "";
            Completed = false;
        }

        public int CompareTo(TaskDataEntity? other)
        {
            if (other == null) return 1;

            //return string.Compare(Description, other.Description, StringComparison.OrdinalIgnoreCase);

            int compareCompleted = Completed.CompareTo(other.Completed);
            if (compareCompleted != 0) return compareCompleted;

            int compareDescription = string.Compare(Description, other.Description, StringComparison.OrdinalIgnoreCase);
            if (compareDescription != 0) return compareDescription;

            if (DueDate.HasValue && other.DueDate.HasValue)
            {
                int compareDueDate = DueDate.Value.CompareTo(other.DueDate.Value);
                if (compareDueDate != 0) return compareDueDate;
            }
            else if (DueDate.HasValue) return 1;
            else if (other.DueDate.HasValue) return -1;

            return string.Compare(Notes, other.Notes, StringComparison.OrdinalIgnoreCase);
        }

        public string ToString_ValuesOnly()
        {
            return $"{Description} {Notes} {Completed} {DueDate?.ToString("yyyy-MM dd_HH:mm tt")}";// use spaces for delimiters here for regex search matching?
        }

        #region static compare methods
        public static int CompareTasksByDueDate(TaskDataEntity x, TaskDataEntity y)
        {
            if (x.DueDate.HasValue && y.DueDate.HasValue)
            {
                return x.DueDate.Value.CompareTo(y.DueDate.Value);
            }
            else if (x.DueDate.HasValue) return 1;
            else if (y.DueDate.HasValue) return -1;

            return 0;
        }
        public static int CompareTasksByCompleted(TaskDataEntity x, TaskDataEntity y)
        {
            return x.Completed.CompareTo(y.Completed);
        }

        public static int CompareTasksByDescription(TaskDataEntity x, TaskDataEntity y)
        {
            return string.Compare(x.Description, y.Description, StringComparison.OrdinalIgnoreCase);
        }

        public static int CompareTasksByNotes(TaskDataEntity x, TaskDataEntity y)
        {
            return string.Compare(x.Notes, y.Notes, StringComparison.OrdinalIgnoreCase);
        }

        #endregion
    }
}
