using TaskManagerCore.Infrastructure.BinaryFile.Entity;

namespace TaskManagerCore.Infrastructure.BinaryFile.QueryComparers
{
    internal class TaskCompletedComparer : QueryComparerBase<TaskDataEntity>
    {
        protected override int CompareNonNull(TaskDataEntity x, TaskDataEntity y)
        {
            return TaskDataEntity.CompareTasksByCompleted(x, y);
        }
    }
}