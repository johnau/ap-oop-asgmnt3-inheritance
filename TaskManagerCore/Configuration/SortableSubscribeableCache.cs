using System.Diagnostics;
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
    /// References to them are never passed outside of Infrastructure.Persistence package (atm = .BinaryFile for the assignment), they are updated in situ.  
    /// Entity objects are used as transients/dtos but they are not stored, their values are used and references are discarded.
    /// Model data is transient, and no references to it are held anywhere, all instances are destroyed after a call to a method.
    /// Model objects are stateless, and immutable to preserve data/system integrity.  
    /// *** Have been trying to use Visual Studio debug tools to analyze object creation and lifecycle.....need tutorial
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
    /// 13-March-2024
    /// ---------
    /// Started to think about it the wrong way.  
    /// - It should NOT extend the Dictionary object, or implement the IDictionary interface (delete those few classes)
    /// ...... don't want to have to override every collection, enumerable, dictionary method (can't have exposed methods that don't notify)
    /// - It should be lists, not dictionaries, dictionaries are not ordered, sortableDictionary doesn't provide the exact functionality
    /// ...... lists are convenient for this application as indexes
    /// - These indexes will be primarily used for sorting - if we introduce pagination, or result limits, then these lists might be important 
    /// ...... for returning the right segments of results 
    /// 
    /// 
    /// --------
    /// Need methods for searching each sorting just their property?
    /// Need to implement updating for the lists based on cache updates - override but still call Notify() method
    /// 
    /// 
    /// Need to store the lists as original and reversed order
    /// This way for binary searches the same comparer can be used to find the lower and upper bounds
    /// of the search results.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class SortableSubscribeableCache<T> : SubscribeableCache<T> 
        where T : IComparable<T>//, ISearchable // Can do a SearchableSortableSubscribeableCache - lol
    {
        List<T> MasterList;
        Dictionary<string, List<T>> SortedLists;
        Dictionary<string, List<T>> ReversedSortedLists;
        private readonly Dictionary<string, Comparison<T>> SortFunctions;

        public SortableSubscribeableCache(Dictionary<string, Comparison<T>> sortFunctions)
            : base()
        {
            SortFunctions = sortFunctions;
            MasterList = new List<T>(Cache.Values); // is this needed? master list is the cache dictionary
            SortedLists = new Dictionary<string, List<T>>();
            ReversedSortedLists = new Dictionary<string, List<T>>();

            ReSortAll(true);
        }

        protected sealed override void NotifySubscribers(NotifiedAction action, string? id = null)
        {
            base.NotifySubscribers(action);
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

        void ReSortAll(bool overwrite = false)
        {
            if (overwrite)
            {
                MasterList.Clear();
                MasterList.AddRange(Cache.Values);
            }
            MasterList.Sort();  // Sorting with CompareTo()

            foreach (var sort in SortFunctions)
            {
                // generate or resort all lists
                if (overwrite || !SortedLists.TryGetValue(sort.Key, out var originalOrder))
                {
                    var list = new List<T>(Cache.Values); // copy, sort, save
                    list.Sort(sort.Value);
                    SortedLists[sort.Key] = list;

                    var listRev = new List<T>(list); // same for reverse
                    listRev.Reverse();
                    ReversedSortedLists[sort.Key] = listRev;
                }
                else
                {
                    // manually check if already sorted - save on some swaps
                    // C# List.Sort(Comparison<T> ...) method incurs overheads for sorting an already/almost
                    // sorted list.  It will still perform swaps to check order.  (Need to verify 100%)
                    // but checking the sorting manually could be a performance increase for longer lists...
                    // This is a task manager though.... so how long are the lists going to get in reality...
                    if (!originalOrder.IsSortedBy(sort.Value)) 
                    {
                        originalOrder.Sort(sort.Value);

                        var reversedOrder = new List<T>(originalOrder);
                        reversedOrder.Reverse();
                        ReversedSortedLists[sort.Key] = reversedOrder;
                    } 
                    else
                    {
                        Debug.WriteLine("List already sorted as expected");
                    }
                }
            }
        }

        public sealed override bool TryAdd(string id, T item)
        {
            var result = base.TryAdd(id, item);
            ReSortAll(true);
            return result;
        }

        /// <summary>
        /// Remove from Cache by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public sealed override bool Remove(string id)
        {
            var result = base.Remove(id);
            ReSortAll(true);
            return result;
        }

        /// <summary>
        /// Flush changes to items
        /// Is Flush the wrong term for what this method does?
        /// a Flush() method should probably be calling ReSortAll(true) - this method calls ReSortAll(false)
        /// Should this be CommitChanges() or Dirty() or something like that...?
        /// </summary>
        /// <returns></returns>
        public sealed override void MarkDirty()
        {
            base.MarkDirty();
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
                if (!ReversedSortedLists.TryGetValue(sorting, out var reversedSortedList))
                    throw new Exception($"Sorting not found in Reversed Lists: {sorting}");

                return new List<T>(reversedSortedList);
            }

            if (!SortedLists.TryGetValue(sorting, out var sortedList))
                throw new Exception($"Sorting not found in Forward Lists: {sorting}");

            return new List<T>(sortedList);
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
