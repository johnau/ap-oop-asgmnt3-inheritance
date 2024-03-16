using TaskManagerCore.Infrastructure.BinaryFile.Entity;

namespace TaskManagerCore.Infrastructure.BinaryFile.QueryComparers
{
    internal class TaskNotesStartsWithComparer : QueryComparerBase<TaskDataEntity>
    {
        //private readonly bool _flip;
        //private readonly bool _matchStartsWith;

        //public TaskNotesStartsWithComparer(bool matchStartsWith = true, bool flip = false)
        //{
        //    _matchStartsWith = matchStartsWith;
        //    _flip = flip;
        //}

        protected override int CompareNonNull(TaskDataEntity x, TaskDataEntity y)
        {
            var startsWithMatch = x.Notes.StartsWith(y.Notes, StringComparison.OrdinalIgnoreCase);
            //if (!_matchStartsWith) startsWithMatch = !startsWithMatch; // flip behavior to find first non-match
            if (startsWithMatch) return 0;

            var compared = TaskDataEntity.CompareTasksByNotes(x, y);
            //if (_flip) compared *= -1;
            return compared;
        }
    }
}
    //internal class TaskNotesContainsComparer : QueryComparerBase<TaskDataEntity>
    //{
    //    protected override int CompareNonNull(TaskDataEntity x, TaskDataEntity y)
    //    {
    //        var startsWithMatch = x.Notes.StartsWith(y.Notes, StringComparison.OrdinalIgnoreCase);
    //        if (startsWithMatch) return 0;
            
    //        var compared = TaskDataEntity.CompareTasksByNotes(x, y);
    //        return compared;
    //    }
    //}
//}
//    {
//        protected override bool Equals(TaskDataEntity searchCriteria, TaskDataEntity t)
//        {
//            return t.Notes.Contains(searchCriteria.Notes, StringComparison.OrdinalIgnoreCase);
//        }
//    }
//}
    //internal class TaskDataNotes_ContainsComparer : IComparer<TaskDataEntity>
    //{
    //    public int Compare(TaskDataEntity? searchCriteria, TaskDataEntity? t)
    //    {
    //        //if (searchCriteria == null && t2 == null) return 0;
    //        if (searchCriteria == null) throw new Exception("No search criteria");
    //        if (t == null) return 1;
    //        if (t.Notes.Contains(searchCriteria.Notes, StringComparison.OrdinalIgnoreCase))
    //        {
    //            return 0;
    //        }

    //        return 1;
    //    }
    //}
//}