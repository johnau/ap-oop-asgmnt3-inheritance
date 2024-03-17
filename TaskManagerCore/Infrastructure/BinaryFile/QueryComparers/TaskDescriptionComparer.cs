using TaskManagerCore.Infrastructure.BinaryFile.Entity;

namespace TaskManagerCore.Infrastructure.BinaryFile.QueryComparers
{
    internal class TaskDescriptionComparer : QueryComparerBase<TaskDataEntity>
    {
        protected override int CompareNonNull(TaskDataEntity x, TaskDataEntity y)
        {
            return TaskDataEntity.CompareTasksByDescription(x, y);
        }
    }
}