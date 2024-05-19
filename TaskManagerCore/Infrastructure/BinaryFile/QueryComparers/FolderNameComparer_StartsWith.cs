using System;
using TaskManagerCore.Infrastructure.BinaryFile.Entity;

namespace TaskManagerCore.Infrastructure.BinaryFile.QueryComparers
{
    internal class FolderNameComparer_StartsWith : QueryComparerBase<TaskFolderEntity>
    {
        protected override int CompareNonNull(TaskFolderEntity x, TaskFolderEntity y)
        {
            var startsWithMatch = x.Name.StartsWith(y.Name, StringComparison.OrdinalIgnoreCase);
            if (startsWithMatch) return 0;
            
            return TaskFolderEntity.CompareByName(x, y);
        }
    }
}