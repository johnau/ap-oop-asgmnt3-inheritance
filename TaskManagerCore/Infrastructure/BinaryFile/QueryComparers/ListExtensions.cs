using TaskManagerCore.Infrastructure.BinaryFile.Entity;

namespace TaskManagerCore.Infrastructure.BinaryFile.QueryComparers
{
    internal static class ListExtensions
    {
        /// <summary>
        /// List extension to exapnd the capabilities of BinarySearch, to find all matches with provided Comparer
        /// BinarySearch result index is random in the range of possible matches.  To find the related matches,
        /// we need to search either side of the matched index.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="criteria"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        internal static List<TaskDataEntity> DoBinarySearchFindAll(this List<TaskDataEntity> list, 
                                                                    TaskDataEntity criteria, 
                                                                    IComparer<TaskDataEntity> comparer)
        {
            var _list = new List<TaskDataEntity>(list);
            _list.Insert(0, new TaskDataEntity(""));

            var matches = new List<TaskDataEntity>();

            var firstMatch = _list.BinarySearch(criteria, comparer);
            if (firstMatch < 0)
                return matches;

            matches.Add(_list[firstMatch]);

            // Search right and left
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
