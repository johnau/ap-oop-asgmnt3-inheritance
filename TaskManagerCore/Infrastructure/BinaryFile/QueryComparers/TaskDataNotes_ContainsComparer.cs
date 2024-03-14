using TaskManagerCore.Infrastructure.BinaryFile.Entity;

namespace TaskManagerCore.Infrastructure.BinaryFile.QueryComparers
{
    internal class TaskDataNotes_ContainsComparer : QueryComparerBase<TaskDataEntity>
    {
        protected override bool CompareMethod(TaskDataEntity searchCriteria, TaskDataEntity t)
        {
            return t.Notes.Contains(searchCriteria.Notes, StringComparison.OrdinalIgnoreCase);
        }
    }
}
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