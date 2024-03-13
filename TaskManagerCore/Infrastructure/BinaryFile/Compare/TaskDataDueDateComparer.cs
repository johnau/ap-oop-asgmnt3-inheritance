using TaskManagerCore.Infrastructure.BinaryFile.Entity;

namespace TaskManagerCore.Infrastructure.BinaryFile.Compare
{
    internal class TaskDataDueDateComparer : IComparer<TaskDataEntity>
    {
        public int Compare(TaskDataEntity? t1, TaskDataEntity? t2)
        {
            if (t1 == null && t2 == null) return 0;
            if (t1 == null) return -1;
            if (t2 == null) return 1;
            if (t1.DueDate.HasValue && t2.DueDate.HasValue)
            {
                return t1.DueDate.Value.CompareTo(t2.DueDate.Value);
            }
            else if (t1.DueDate.HasValue) return 1;
            else if (t2.DueDate.HasValue) return -1;

            return 0;
        }
    }
}
