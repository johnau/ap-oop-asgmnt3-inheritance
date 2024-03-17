using TaskManagerCore.Infrastructure.BinaryFile.Entity;

namespace TaskManagerCore.Infrastructure.BinaryFile.QueryComparers
{
    internal class FolderNameComparer : QueryComparerBase<TaskFolderEntity>
    {
        protected override int CompareNonNull(TaskFolderEntity x, TaskFolderEntity y)
        {
            return TaskFolderEntity.CompareFoldersByName(x, y);
        }
    }
}
