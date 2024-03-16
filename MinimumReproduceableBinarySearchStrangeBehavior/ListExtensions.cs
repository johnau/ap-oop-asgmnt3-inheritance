namespace MinimumReproduceableBinarySearchStrangeBehavior
{
    internal static class ListExtensions
    {
        internal static List<DataObject> DoBinarySearchFindAll(this List<DataObject> list, DataObject criteria, BinaryComparerBase<DataObject> comparer)
        {
            var _list = new List<DataObject>(list);
            _list.Insert(0, new DataObject("", ""));

            var matches = new List<DataObject>();

            var firstMatch = _list.BinarySearch(criteria, comparer);
            if (firstMatch < 0) 
                return matches;
            matches.Add(_list[firstMatch]);

            // Search left and right
            foreach (int i in new int[] { 1, -1 })
            {
                // Reset to the initial match
                int current = firstMatch;
                while (current + i < _list.Count && current + i >= 0)
                {
                    current = _list.BinarySearch(current + i, 1, criteria, comparer);
                    if (current < 0) break;

                    matches.Add(_list[current]);
                }
            }
            return matches;
        }
    }
}
