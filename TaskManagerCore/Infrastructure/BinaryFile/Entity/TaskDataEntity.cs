﻿using TaskManagerCore.Configuration;

namespace TaskManagerCore.Infrastructure.BinaryFile.Entity
{
    internal class TaskDataEntity : EntityBase, IComparable<TaskDataEntity>, ITextSearchable
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

            int compareCompleted = Completed.CompareTo(other.Completed);
            if (compareCompleted != 0) return compareCompleted;

            if (DueDate.HasValue && other.DueDate.HasValue)
            {
                int compareDueDate = DueDate.Value.CompareTo(other.DueDate.Value);
                if (compareDueDate != 0) return compareDueDate;
            }
            else if (DueDate.HasValue) return 1;
            else if (other.DueDate.HasValue) return -1;

            int compareDescription = string.Compare(Description, other.Description, StringComparison.OrdinalIgnoreCase);
            if (compareDescription != 0) return compareDescription;

            return string.Compare(Notes, other.Notes, StringComparison.OrdinalIgnoreCase);
        }

        public string GetTextStringForSearch()
        {
            return $"{Description}_{Notes}_{Completed}_{DueDate?.ToString("yyyy-MM-dd_HH:mm:ss")}";
        }

        public static int CompareTasksByDueDate(TaskDataEntity t1, TaskDataEntity t2)
        {
            if (t1.DueDate.HasValue && t2.DueDate.HasValue)
            {
                return t1.DueDate.Value.CompareTo(t2.DueDate.Value);
            }
            else if (t1.DueDate.HasValue) return 1;
            else if (t2.DueDate.HasValue) return -1;

            return 0;
        }
        public static int CompareTasksByCompleted(TaskDataEntity t1, TaskDataEntity t2)
        {
            return t1.Completed.CompareTo(t2.Completed);
        }

        public static int CompareTasksByDescription(TaskDataEntity t1, TaskDataEntity t2)
        {
            return string.Compare(t1.Description, t2.Description, StringComparison.OrdinalIgnoreCase);
        }

        public static int CompareTasksByNotes(TaskDataEntity t1, TaskDataEntity t2)
        {
            return string.Compare(t1.Notes, t2.Notes, StringComparison.OrdinalIgnoreCase);
        }
    }
}
