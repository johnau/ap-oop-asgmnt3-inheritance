using System.Diagnostics;
using System.Security.Principal;

namespace InMemoryCache
{
    /// <summary>
    /// Dictionary with Subscriptions and Sorting
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SortableSubscribeableCache<T> : SubscribeableCache<T>
        where T : IComparable<T>, IIdentifiable
    {
        private readonly Dictionary<string, Comparison<T>> _sortFunctions;
        private readonly List<T> _masterList;
        private readonly Dictionary<string, List<T>> _sortedLists;
        private readonly Dictionary<string, List<T>> _reversedSortedLists;

        public SortableSubscribeableCache(Dictionary<string, Comparison<T>> sortFunctions)
            : base()
        {
            _sortFunctions = sortFunctions;
            _masterList = new List<T>(Cache.Values);
            _sortedLists = new Dictionary<string, List<T>>();
            _reversedSortedLists = new Dictionary<string, List<T>>();

            ReSortAll(true);
        }

        /// <summary>
        /// Override and seal NotifySubscribers - although shouldn't be extending any further
        /// </summary>
        /// <param name="action"></param>
        /// <param name="id"></param>
        /// <exception cref="ArgumentException"></exception>
        protected sealed override void NotifySubscribers(NotifiedAction action, string? id = null)
        {
            base.NotifySubscribers(action, id);

            if (id == null) // take new copies if added or removed
            {
                switch (action)
                {
                    case NotifiedAction.UPDATE:
                        ReSortAll(overwrite: false);
                        break;
                    case NotifiedAction.ADD:
                    case NotifiedAction.REMOVE:
                        ReSortAll(overwrite: true);
                        break;
                    default:
                        throw new ArgumentException("Ungrecognized `NotifiedAction`");
                }
            }
            else
            {
                switch (action)
                {
                    case NotifiedAction.UPDATE:
                        // do we need to do something here...
                        ReSortAll(overwrite: false);
                        break;

                    case NotifiedAction.ADD:
                        if (Cache.TryGetValue(id, out var newItem))
                            AddItemToIndexes(newItem);
                        else
                            throw new Exception("Cache is in an unexpected state");

                        ReSortAll(overwrite: false);
                        break;

                    case NotifiedAction.REMOVE:
                        RemoveItemFromIndexes(id);

                        ReSortAll(overwrite: false);
                        break;

                    default:
                        throw new ArgumentException("Ungrecognized `NotifiedAction`");
                }
            }
        }

        /// <summary>
        /// Add item to sorted and reverse sorted indexes
        /// </summary>
        /// <param name="item"></param>
        void AddItemToIndexes(T item)
        {
            foreach (var sortedEntry in _sortedLists)
            {
                var list = sortedEntry.Value;
                var revList = _reversedSortedLists[sortedEntry.Key];
                if (list.Count == 0)
                {
                    list.Add(item);
                    revList.Add(item);
                    continue;
                }
                var insertIndex = list.BinarySearch(item); // BinarySearch uses the same comparison func used to sort the list if no other func is provided
                if (insertIndex < 0) insertIndex = ~insertIndex; // bitflip -> use the position of the next larger 
                if (insertIndex == list.Count) // insert end of list, call Add instead.
                {
                    list.Add(item);
                    revList.Insert(0, item);
                    continue;
                }
                
                list.Insert(insertIndex, item); // could just pass the unflipped index here and would probably be the same
                revList.Insert(revList.Count - 1 - insertIndex, item); // could just pass the unflipped index here and would probably be the same
            }
            Debug.WriteLine("Added a new item to all indexes");
        }

        /// <summary>
        /// Remove item from sorted and reverse sorted indexes
        /// </summary>
        /// <param name="id"></param>
        void RemoveItemFromIndexes(string id)
        {
            foreach (var sortedEntry in _sortedLists)
            {
                var list = sortedEntry.Value;
                var revList = _reversedSortedLists[sortedEntry.Key];

                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    if (item.Id == id)
                    {
                        list.RemoveAt(i);
                        revList.RemoveAt(revList.Count - 1 - i);
                        break;
                    }
                }
            }
            Debug.WriteLine("Removed item from all indexes");
        }

        /// <summary>
        /// Private method to sort all lists,
        /// If overwrite is set, lists are re-copied from the cache and sorted
        /// </summary>
        /// <param name="overwrite"></param>
        void ReSortAll(bool overwrite = false)
        {
            SortMasterList(overwrite);

            foreach (var sort in _sortFunctions)
            {
                if (overwrite || !_sortedLists.TryGetValue(sort.Key, out var existingSortedListed))
                    RepopulateLists(sort.Key, sort.Value);
                else
                    SortListsIfRequired(existingSortedListed, sort.Key, sort.Value);
            }
        }

        /// <summary>
        /// The Master list is separated as it suits one of the requirements of the assessment.
        /// Will likely use it to possibly explore an elastic style search across all of the task data.
        /// </summary>
        /// <param name="overwrite"></param>
        void SortMasterList(bool overwrite)
        {
            if (overwrite)
            {
                _masterList.Clear();
                _masterList.AddRange(Cache.Values);
            }
            // Sorting with CompareTo()
            _masterList.Sort();
        }

        /// <summary>
        /// Creates new sorted lists and overwrites existing
        /// </summary>
        /// <param name="sortingName"></param>
        /// <param name="comparison"></param>
        void RepopulateLists(string sortingName, Comparison<T> comparison)
        {
            // forwards
            var list = new List<T>(Cache.Values); // copy, sort, save
            list.Sort(comparison);
            _sortedLists[sortingName] = list;

            // reverse
            var listRev = new List<T>(list); // same for reverse
            listRev.Reverse();
            _reversedSortedLists[sortingName] = listRev;
        }

        /// <summary>
        /// Sorts lists if required
        /// </summary>
        /// <param name="original"></param>
        /// <param name="sortingName"></param>
        /// <param name="comparison"></param>
        void SortListsIfRequired(List<T> original, string sortingName, Comparison<T> comparison)
        {
            // manually check if already sorted - save on some swaps
            // C# List.Sort(Comparison<T> ...) method incurs overheads for sorting an already/almost
            // sorted list.  It will still perform swaps to check order.  (Need to verify 100%)
            // but checking the sorting manually could be a performance increase for longer lists...
            // This is a task manager though.... so how long are the lists going to get in reality...
            if (!original.IsSortedBy(comparison))
            {
                original.Sort(comparison);

                var reversedOrder = new List<T>(original);
                reversedOrder.Reverse();
                _reversedSortedLists[sortingName] = reversedOrder;
            }
            else
            {
                Debug.WriteLine("List already sorted as expected");
            }
        }

        #region Dictionary methods
        // Providing these methods so that this Cache can be easily substituted where a Dictionary is currently used
        // If enough are implemented, might as well just implement IDictionary, IEnumerable, etc and override all

        /// <summary>
        /// Simulating a method from Dictionary class that was in use, since this class
        /// has replaced a Dictionary in the DAO objects
        /// </summary>
        /// <param name="id"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public sealed override bool TryAdd(string id, T item)
        {
            var result = base.TryAdd(id, item);
            if (result)
                ReSortAll(true);

            return result;
        }

        ///// <summary>
        ///// Simulating a method from Dictionary class that was in use, since this class
        ///// has replaced a Dictionary in the DAO objects
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public sealed override bool Remove(string id)
        //{
        //    var result = base.Remove(id);
        //    //if (result)
        //    //    ReSortAll(true);  // no need to resort on remove?
        //    return result;
        //}

        #endregion

        /// <summary>
        /// Flush changes to items
        /// Is Flush the wrong term for what this method does?
        /// a Flush() method should probably be calling ReSortAll(true) - this method calls ReSortAll(false)
        /// Should this be CommitChanges() or Dirty() or something like that...?
        /// </summary>
        /// <returns></returns>
        public sealed override void MarkDirty(string? id = null)
        {
            base.MarkDirty(id);
            ReSortAll();
        }

        /// <summary>
        /// This method is not very safe and is not used anymore...
        /// </summary>
        /// <param name="id"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        internal sealed override bool ForceReplace(string id, T item)
        {
            var result = base.ForceReplace(id, item);
            ReSortAll(true);
            return result;
        }

        /// <summary>
        /// Acquire sorted list
        /// </summary>
        /// <param name="sorting"></param>
        /// <returns></returns>
        /// <exception cref="Exception">Throws if sorting not found</exception>
        public List<T> SortedBy(string sorting, bool reversed = false)
        {
            if (reversed)
            {
                if (!_reversedSortedLists.TryGetValue(sorting, out var reversedSortedList))
                    throw new Exception($"Sorting not found in Reversed Lists: {sorting}");

                return new List<T>(reversedSortedList);
            }

            if (!_sortedLists.TryGetValue(sorting, out var sortedList))
                throw new Exception($"Sorting not found in Forward Lists: {sorting}");

            return new List<T>(sortedList);
        }

        /// <summary>
        /// Pass on BinarySearch to master list
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public T BinarySearch(T item)
        {
            var index = _masterList.BinarySearch(item);
            return _masterList[index];
        }

        ///// <summary>
        ///// Does this make sense to exist here?  Just for this method we need to have the requirement on the ISearchable interface...
        ///// </summary>
        ///// <param name="sorting"></param>
        ///// <param name="searchString"></param>
        ///// <param name="order"></param>
        ///// <param name="caseInsensitive"></param>
        ///// <returns></returns>
        //public IEnumerable<T> SearchSorted_AllProperties(string sorting, string searchString, Order order = Order.ASCENDING, bool caseInsensitive = true)
        //{
        //    if (caseInsensitive) searchString = searchString.ToLower();
        //    var pattern = "\\b" + Regex.Escape(searchString) + "\\b";
        //    var regex = new Regex(pattern, RegexOptions.IgnoreCase);
        //    var sortedList = SortedBy(sorting, order);
        //    //if (!SortedLists.TryGetValue(sorting, out var sortedList))
        //    //    throw new Exception($"Sorting not found: {sorting}");

        //    for (int i = 0; i < sortedList.Count; i++)
        //    {
        //        //if (sortedList[i].GetTextStringForSearch().Contains(searchString))
        //        var objectAsString = sortedList[i].ToString_ValuesOnly();
        //        if (caseInsensitive)
        //        if (regex.IsMatch(objectAsString))
        //        {
        //            yield return sortedList[i];
        //        }
        //    }
        //}

    }
}
