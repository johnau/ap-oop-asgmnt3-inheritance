
namespace TaskManagerCore.Infrastructure.BinaryFile.QueryComparers
{
    internal abstract class QueryComparerBase<T> : IComparer<T>
    {
        public int Compare(T? t1, T? t2)
        {
            //if (searchCriteria == null && t2 == null) return 0;
            if (t1 == null) return -1;
            //if (!IsOk(t1)) throw new Exception("Failed search criteria check");
            if (t2 == null) return 1;
            if (Equals(t1, t2)) 
                return 0;
            if (GreaterThan(t1, t2)) return 1;
            return -1;
        }

        protected abstract bool Equals(T t1, T t2);
        protected virtual bool GreaterThan(T t1, T t2) => true;

        //protected virtual bool IsOk(T t1) => true;
    }
}
