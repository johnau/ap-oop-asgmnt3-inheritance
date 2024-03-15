using TaskManagerCore.Infrastructure.BinaryFile.Entity;

namespace TaskManagerCore.Infrastructure.BinaryFile.QueryComparers
{
    internal class TaskDataDueDate_DateOnlyComparer : IComparer<TaskDataEntity> // : QueryComparerBase<TaskDataEntity>
    {
        public int Compare(TaskDataEntity? t1, TaskDataEntity? t2)
        {
            return TaskDataEntity.CompareTasksByDueDate(t1!, t2!);
        }
    }
}