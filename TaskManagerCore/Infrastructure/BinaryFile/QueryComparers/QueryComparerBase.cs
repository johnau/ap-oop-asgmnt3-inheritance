
namespace TaskManagerCore.Infrastructure.BinaryFile.QueryComparers
{
    internal abstract class QueryComparerBase<T> : IComparer<T>
    {
        public int Compare(T? searchCriteria, T? t)
        {
            //if (searchCriteria == null && t2 == null) return 0;
            if (searchCriteria == null) throw new Exception("No search criteria");
            if (!SearchCriteriaOk(searchCriteria)) throw new Exception("Failed search criteria check");
            if (t == null) return 1;
            if (CompareMethod(searchCriteria, t)) return 0;
            return 1;
        }

        protected abstract bool CompareMethod(T searchCriteria, T t);
        protected virtual bool SearchCriteriaOk(T searchCriteria) => true;
    }
}
