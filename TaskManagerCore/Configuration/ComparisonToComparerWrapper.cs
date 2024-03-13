namespace TaskManagerCore.Configuration
{
    internal class ComparisonToComparerWrapper<T> : IComparer<T>
    {
        private readonly Comparison<T> comparison;

        public ComparisonToComparerWrapper(Comparison<T> comparison)
        {
            this.comparison = comparison;
        }

        public int Compare(T? t1, T? t2)
        {
            if (t1 == null && t2 == null) return 0;
            if (t1 == null || t2 == null) return t1 == null ? -1 : 1;
            return comparison(t1, t2);
        }
    }
}
