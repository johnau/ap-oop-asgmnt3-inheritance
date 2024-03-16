
using TaskManagerCore.Infrastructure.BinaryFile.Entity;

namespace TaskManagerCore.Infrastructure.BinaryFile.QueryComparers
{
    internal class TaskDescriptionStartsWithComparer : QueryComparerBase<TaskDataEntity>
    {
        // none of this flip stuff can work because it's always a random match that is found
        //private readonly bool _flip;
        //// Set to false to use the comparer to find the first entry that does not match
        //private bool _matchStartsWith = true;

        //public TaskDescriptionStartsWithComparer(bool matchStartsWith = true, bool flip = false)
        //{
        //    _matchStartsWith = matchStartsWith;
        //    _flip = flip;
        //}

        protected override int CompareNonNull(TaskDataEntity x, TaskDataEntity y)
        {
            var startsWithMatch = x.Description.StartsWith(y.Description, StringComparison.OrdinalIgnoreCase);
            //if (!_matchStartsWith) startsWithMatch = !startsWithMatch; // flip behavior to find first non-match
            if (startsWithMatch) return 0;

            var compared = TaskDataEntity.CompareTasksByDescription(x, y);
            //if (_flip) compared *= -1;
            return compared;
        }
    }
}


//        protected override bool Equals(TaskDataEntity searchCriteria, TaskDataEntity t)
//        {
//            return t.Description.StartsWith(searchCriteria.Description, StringComparison.OrdinalIgnoreCase);
//        }
//    }
//}
    //internal class TaskDataDescription_BeginsWithComparer : IComparer<TaskDataEntity>
    //{
    //    public int Compare(TaskDataEntity? searchCriteria, TaskDataEntity? t)
    //    {
    //        //if (searchCriteria == null && t2 == null) return 0;
    //        if (searchCriteria == null) throw new Exception("No search criteria");
    //        if (t == null) return 1;
    //        if (t.Description.StartsWith(searchCriteria.Description, StringComparison.OrdinalIgnoreCase))
    //        {
    //            return 0;
    //        }

    //        return 1;
    //    }
    //}
//}
