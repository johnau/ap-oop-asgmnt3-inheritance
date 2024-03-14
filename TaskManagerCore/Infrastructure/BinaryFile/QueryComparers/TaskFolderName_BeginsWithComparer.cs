using TaskManagerCore.Infrastructure.BinaryFile.Entity;

namespace TaskManagerCore.Infrastructure.BinaryFile.QueryComparers
{
    internal class TaskFolderName_BeginsWithComparer : QueryComparerBase<TaskFolderEntity>
    {
        protected override bool CompareMethod(TaskFolderEntity searchCriteria, TaskFolderEntity t)
        {
            return t.Name.StartsWith(searchCriteria.Name, StringComparison.OrdinalIgnoreCase);
        }
    }
}
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
