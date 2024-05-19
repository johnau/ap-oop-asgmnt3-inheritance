using System;
using TaskManagerCore.Infrastructure.BinaryFile.Entity;

namespace TaskManagerCore.Infrastructure.BinaryFile.QueryComparers
{
    internal class TaskNotesComparer_StartsWith : QueryComparerBase<TaskDataEntity>
    {
        protected override int CompareNonNull(TaskDataEntity x, TaskDataEntity y)
        {
            var startsWithMatch = x.Notes.StartsWith(y.Notes, StringComparison.OrdinalIgnoreCase);
            if (startsWithMatch) return 0;

            return TaskDataEntity.CompareByNotes(x, y);
        }
    }
}