
using TaskManagerCore.Infrastructure.BinaryFile.Entity;

namespace TaskManagerCore.Infrastructure.BinaryFile.QueryComparers
{
    internal class TaskDescriptionComparer_StartsWith : QueryComparerBase<TaskDataEntity>
    {
        protected override int CompareNonNull(TaskDataEntity x, TaskDataEntity y)
        {
            var startsWithMatch = x.Description.StartsWith(y.Description, StringComparison.OrdinalIgnoreCase);
            if (startsWithMatch) return 0;

            return TaskDataEntity.CompareByDescription(x, y);
        }
    }
}