namespace MinimumReproduceableBinarySearchStrangeBehavior
{
    internal abstract class BinaryComparerBase<T> : IComparer<T>
    {
        public int Compare(T? x, T? y)
        {
            //if (x == null || y == null) throw new ArgumentNullException($"x: {x}, y: {y}");
            if (x == null && y == null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            return CompareNonNull(x, y);
        }

        protected abstract int CompareNonNull(T x, T y);
    }
}