using TaskManagerCore.Infrastructure.BinaryFile.Entity;

namespace TaskManagerCore.Infrastructure.BinaryFile.QueryComparers
{
    internal class FolderTaskCountComparer : QueryComparerBase<TaskFolderEntity>
    {
        private readonly bool _ascending;

        public FolderTaskCountComparer(bool ascending = true) 
        {
            _ascending = ascending;
        }
        protected override int CompareNonNull(TaskFolderEntity x, TaskFolderEntity y)
        {
            var result = TaskFolderEntity.CompareFoldersByTaskCount(x, y);
            if (!_ascending) result *= -1;
            return result;
        }
    }
}