using System.Collections.Immutable;
using TaskManagerCore.Infrastructure.BinaryFile.Compare;

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
    /// Currently, the entity objects are stored in the 'Cache'.  
    /// References to them are never passed outside of Infrastructure package (atm = .BinaryFile for the assignment), they are updated in situ.  
    /// Entity objects are used as transients/dtos but they are not stored, their values are used and references are discarded.
    /// Model data is transient, and no references to it are held anywhere, all instances are destroyed after a call to a method.
    /// Model objects are stateless, and immutable to preserve data/system integrity.  
    /// 
    /// ----------
    /// Task Requirements:
    /// Sorts the tasks by date and re-sorts as required. Test by doing a search for a task on a particular day (and providing a date sorted listing)
    /// Has an index sorted by name.This will need to be kept up to date and re-sorted as required.Test by doing search for a task by its name.
    /// 
    /// Don’t forget that if you add or remove an sort from a list, you will need to do the same in the index as well.
    /// ----------
    /// TODO:
    /// Override add, remove, etc methods - need to trigger re-sorts on these events
    /// Need to ensure that control over editing this list is maintained?? Or would that be another class extending this one.... "ImmutableIndexedSubscribeableCache<T>"
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class IndexedSubscribeableCache<T> : SubscribeableCache<T> 
        where T : IComparable<T>, ITextSearchable
    {
        List<T> MasterList; // if we don't use lists
        Dictionary<string, List<T>> SortedLists;
        //new readonly Dictionary<string, T> Cache; // override Cache
        //Dictionary<string, Dictionary<string, T>> SortedIndexes;
        public IndexedSubscribeableCache(Dictionary<string, Comparison<T>> sortFunctions)
            : base()
        {
            MasterList = new List<T>(Cache.Values);
            SortedLists = new Dictionary<string, List<T>>();

            // use provided sort functions to generate sorted lists
            foreach (var sort in sortFunctions)
            {
                var list = new List<T>(MasterList);
                list.Sort(sort.Value);
                SortedLists.Add(sort.Key, list);
            }
        }

        //public IndexedSubscribeableCache(Dictionary<string, Comparison<T>> sortFunctions)
        //    : base()
        //{
        //    Cache = new Dictionary<string, T>();
        //    SortedIndexes = new Dictionary<string, Dictionary<string, T>>();

        //    // use provided sort functions to generate sorted lists

        //    foreach (var sort in sortFunctions)
        //    {
        //        var dict = new Dictionary<string, T>(Cache);
        //        dict.OrderBy(x => x.Value, new ComparisonToComparerWrapper<T>(sort.Value));
        //        SortedIndexes.Add(sort.Key, dict);
        //    }
        //}


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

        public List<T> SortedBy(string sorting)
        {
            if (!SortedLists.TryGetValue(sorting, out var sortedList))
                throw new Exception($"Sorting not found: {sorting}");

            return sortedList;
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
