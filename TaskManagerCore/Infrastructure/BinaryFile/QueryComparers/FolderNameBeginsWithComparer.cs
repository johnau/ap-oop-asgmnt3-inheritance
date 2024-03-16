using TaskManagerCore.Infrastructure.BinaryFile.Entity;

namespace TaskManagerCore.Infrastructure.BinaryFile.QueryComparers
{
    internal class FolderNameBeginsWithComparer : QueryComparerBase<TaskFolderEntity>
    {
        protected override int CompareNonNull(TaskFolderEntity x, TaskFolderEntity y)
        {
            var startsWithMatch = x.Name.StartsWith(y.Name, StringComparison.OrdinalIgnoreCase);
            if (startsWithMatch) return 0;
            
            var compared = TaskFolderEntity.CompareTasksByName(x, y);
            return compared;
        }
    }
}
//    {
//        protected override bool Equals(TaskFolderEntity searchCriteria, TaskFolderEntity t)
//        {
//            return t.Name.StartsWith(searchCriteria.Name, StringComparison.OrdinalIgnoreCase);
//        }
//    }
//}
    //internal class TaskFolderName_BeginsWithComparer : IComparer<TaskFolderEntity>
    //{
    //    public int Compare(TaskFolderEntity? searchCriteria, TaskFolderEntity? t)
    //    {
    //        //if (searchCriteria == null && t2 == null) return 0;
    //        if (searchCriteria == null) throw new Exception("No search criteria");
    //        if (t == null) return 1;
    //        if (t.Name.StartsWith(searchCriteria.Name, StringComparison.OrdinalIgnoreCase))
    //        {
    //            return 0;
    //        }

    //        return 1;
    //    }
    //}
//}
