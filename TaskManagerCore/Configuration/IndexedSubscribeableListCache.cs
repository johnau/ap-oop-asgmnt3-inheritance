using System.Linq;

namespace TaskManagerCore.Configuration
{
    internal class IndexedSubscribeableListCache<T> : SubscribeableList<T>
        where T : IComparable<T>, ITextSearchable
    {
        //protected readonly Dictionary<string, Comparison<T>> SortFunctions;
        protected readonly Dictionary<string, List<T>> SortedLists;

        public IndexedSubscribeableListCache(Dictionary<string, Comparison<T>> sortFunctions)
        {
            //SortFunctions = sortFunctions;
            SortedLists = new Dictionary<string, List<T>>();

            foreach (var sort in sortFunctions)
            {
                var list = new List<T>(this);
                list.Sort(sort.Value);
                SortedLists.Add(sort.Key, list);
            }
        }

        public IEnumerable<T> SearchSorted(string sorting, string searchString)
        {
            var sortedList = SortedBy(sorting);
            //if (!SortedLists.TryGetValue(sorting, out var sortedList))
            //    throw new Exception($"Sorting not found: {sorting}");

            for (int i = 0; i < sortedList.Count; i++)
            {
                if (sortedList[i].GetTextStringForSearch().StartsWith(searchString))
                {
                    yield return sortedList[i];
                }
            }
        }

        /// <summary>
        /// Get a list based on certain sorting
        /// </summary>
        /// <param name="sorting"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<T> SortedBy(string sorting)
        {
            if (!SortedLists.TryGetValue(sorting, out var sortedList))
                throw new Exception($"Sorting not found: {sorting}");

            return sortedList;
        }

        ///// <summary>
        ///// Returns index
        ///// </summary>
        ///// <param name="item"></param>
        ///// <returns></returns>
        //public T BinarySearch(T item)
        //{
        //    var index = BinarySearch(item);
        //    return this[index];
        //}
    }
}
