using TaskManagerCore.Infrastructure.BinaryFile.Entity;

namespace TaskManagerCore.Infrastructure.BinaryFile.QueryComparers
{
    internal class TaskCompletedComparer : QueryComparerBase<TaskDataEntity>
    {
        private readonly bool _ascending;

        public TaskCompletedComparer(bool ascending = true) 
        {
            _ascending = ascending;
        }
        protected override int CompareNonNull(TaskDataEntity x, TaskDataEntity y)
        {
            var result = TaskDataEntity.CompareByCompleted(x, y);
            if (!_ascending) 
                result *= -1;
            return result;
        }
    }
}