namespace TaskManagerCore.Configuration
{
    /// <summary>
    /// Notes to consider from provided docs:
    /// -------------------------------
    /// Objects are passed to methods by reference. changes affect the original
    /// Simpler variables like integers and floats are instead passed by copy.  changes do not affect original
    /// Structs are passed by copy. (Only difference between Objects)
    /// You should be careful with passing structs.
    /// Structures and objects can get very large and copying one to give it to a
    /// method is slower than passing a reference. 
    /// Usually, structures tend to be small for this reason
    /// ---------
    /// Currently, the entity objects are currently stored in the Cache.  
    /// References to them are never passed outside of this package, they are updated in place*.  Temporary entities
    /// exist during updates, but no references to these are kept.
    /// Model data is immutable and transient, and no references to it are held anywhere
    /// 
    /// Task Requirements:
    /// Sorts the tasks by date and re-sorts as required. Test by doing a search for a task on a particular day.
    /// Has an index sorted by name.This will need to be kept up to date and re-sorted as required.Test by doing search
    /// for a task by its name using the index.
    /// Don’t forget that if you add or remove an item from a list, you will need to do the same in the index as well.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class IndexedSubscribeableCache<T> : SubscribeableCache<T> where T : IComparable<T>, ITextSearchable
    {
        List<T> MasterList;
        Dictionary<string, List<T>> SortedLists;
        public IndexedSubscribeableCache(Dictionary<string, Comparison<T>> sortFunctions)
            : base()
        {
            MasterList = new List<T>(Cache.Values);
            SortedLists = new Dictionary<string, List<T>>();

            // use provided sort functions to generate sorted lists
            foreach (var item in sortFunctions)
            {
                var list = new List<T>(MasterList);
                list.Sort(item.Value);
                SortedLists.Add(item.Key, list);
            }
        }
        //public IndexedSubscribeableCache() 
        //    : base()
        //{
        //     MasterList = new List<T>(Cache.Values);
        //}

        public IEnumerable<T> SearchSorted(string sorting, string searchString)
        {
            if (!SortedLists.TryGetValue(sorting, out var sortedList))
                throw new Exception($"Sorting not found: {sorting}");

            for (int i = 0; i < sortedList.Count; i++)
            {
                if (sortedList[i].GetTextStringForSearch().Contains(searchString))
                {
                    yield return sortedList[i];
                }
            }
        }

        /// <summary>
        /// Returns index
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public T BinarySearch(T item)
        {
            var index = MasterList.BinarySearch(item);
            return MasterList[index];
        }
    }
}
