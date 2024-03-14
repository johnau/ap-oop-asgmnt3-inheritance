using TaskManagerCore.Infrastructure.BinaryFile.Entity;

namespace TaskManagerCore.Infrastructure.BinaryFile.QueryComparers
{
    internal class TaskDataCompleted_Comparer : QueryComparerBase<TaskDataEntity>
    {
        protected override bool CompareMethod(TaskDataEntity searchCriteria, TaskDataEntity t)
        {
            return t.Completed == searchCriteria.Completed;
        }
    }
}
    //internal class TaskDataCompleted_Comparer : IComparer<TaskDataEntity>
    //{
    //    public int Compare(TaskDataEntity? searchCriteria, TaskDataEntity? t)
    //    {
    //        //if (searchCriteria == null && t2 == null) return 0;
    //        if (searchCriteria == null) throw new Exception("No search criteria");
    //        if (t == null) return 1;
    //        if (t.Completed == searchCriteria.Completed)
    //        {
    //            return 0;
    //        }

    //        return 1;
    //    }
    //}
//}
