using TaskManagerCore.Infrastructure.BinaryFile.Entity;

namespace TaskManagerCore.Infrastructure.BinaryFile.QueryComparers
{
    internal class TaskDataDueDate_DateOnlyComparer : QueryComparerBase<TaskDataEntity>
    {
        protected override bool CompareMethod(TaskDataEntity searchCriteria, TaskDataEntity t)
        {
            return t.DueDate.HasValue 
                && searchCriteria.DueDate.HasValue 
                && t.DueDate.Value.Date == searchCriteria.DueDate.Value.Date;
        }

        protected override bool SearchCriteriaOk(TaskDataEntity searchCriteria) => searchCriteria.DueDate.HasValue;

    }
}
//    internal class TaskDataDueDate_DateOnlyComparer : IComparer<TaskDataEntity>
//    {
//        public int Compare(TaskDataEntity? searchCriteria, TaskDataEntity? t)
//        {
//            //if (searchCriteria == null && t2 == null) return 0;
//            if (searchCriteria == null) throw new Exception("No search criteria");
//            if (!searchCriteria.DueDate.HasValue) throw new Exception("No search criteria");
//            if (t == null) return 1;
//            if (t.DueDate.HasValue)
//            {
//                return t.DueDate.Value.Date == searchCriteria.DueDate.Value.Date ? 0 : 1;
//            }

//            return 1;
//        }
//    }
//}
