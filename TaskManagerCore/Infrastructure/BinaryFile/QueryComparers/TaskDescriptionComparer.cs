using TaskManagerCore.Infrastructure.BinaryFile.Entity;

namespace TaskManagerCore.Infrastructure.BinaryFile.QueryComparers
{
    internal class TaskDescriptionComparer : QueryComparerBase<TaskDataEntity>
    {
        protected override int CompareNonNull(TaskDataEntity x, TaskDataEntity y)
        {
            return TaskDataEntity.CompareTasksByDescription(x, y);
        }
    }
}


//    {
//        protected override bool Equals(TaskDataEntity searchCriteria, TaskDataEntity t)
//        {
//            return t.Description.Equals(searchCriteria.Description, StringComparison.OrdinalIgnoreCase);
//        }
//    }
//}
    //internal class TaskDataDescription_Comparer : IComparer<TaskDataEntity>
    //{
    //    public int Compare(TaskDataEntity? searchCriteria, TaskDataEntity? t)
    //    {
    //        //if (searchCriteria == null && t2 == null) return 0;
    //        if (searchCriteria == null) throw new Exception("No search criteria");
    //        if (t == null) return 1;
    //        if (t.Description.Equals(searchCriteria.Description, StringComparison.OrdinalIgnoreCase))
    //        {
    //            return 0;
    //        }

    //        return 1;
    //    }
    //}
//}
