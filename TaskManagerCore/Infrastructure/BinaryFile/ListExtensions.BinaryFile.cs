using TaskManagerCore.Infrastructure.BinaryFile.Entity;

namespace TaskManagerCore.Infrastructure.BinaryFile
{
    internal static partial class ListExtensions
    {
        /// <summary>
        /// List extension to expand the capabilities of BinarySearch, to find all matches with provided Comparer
        /// BinarySearch result index is random in the range of possible matches.  To find the related matches,
        /// we need to search either side of the matched index.
        /// -----------
        /// When there are multiple matches to the comparer, the binary search is less predictable.  The index matched 
        /// is essentially random within the range of possible matches.
        /// ie. With the following list:
        /// [ `Alpha`, `Bravo 7`, `Bravo 8`, `Bravo 9`, `Charlie`, `Delta 1`, `Delta 2`]
        /// If we want to match "Bravo", we may get index 1, 2, or 3 as the result so we must look either direction 
        /// from the match to get the range of matches. (Providing it is still matching within logic that aligns 
        /// with the sort algorithm)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="criteria"></param>
        /// <param name="comparer"></param>
        /// <param name="blankObject"></param>
        /// <returns></returns>
        internal static List<T> BinarySearchMultiple<T>(this List<T> list,
                                                            T criteria,
                                                            IComparer<T> comparer)
        {
            var matches = new List<T>();

            int firstMatch = 0;
            // Locate any matching item if the first item does not match
            // If the first item is not checked manually, it is missed - not sure why
            // Check the first item with Compare, if not equal, do BinarySearch
            if (comparer.Compare(list[0], criteria) != 0)
                firstMatch = list.BinarySearch(criteria, comparer);
                        
            // If there are only non-exact matches, there are no matches
            if (firstMatch < 0) 
                return matches;

            matches.Add(list[firstMatch]);

            // Search Right and Left
            foreach (int i in new int[] { 1, -1 })
            {
                int current = firstMatch; // Reset to the initial match
                // Whilst inside the bounds of the list index range
                while (current + i >= 0 && current + i < list.Count)
                {
                    // Shift search index (left or right) and check the next
                    // This could be broken down into a chunked process if the list size is large
                    current = list.BinarySearch(current + i, 1, criteria, comparer);
                    if (current < 0) break; // done with this direction if we don't get an exact match

                    matches.Add(list[current]);
                }
            }
            return matches;
        }

        internal static List<T> BinarySearchMultiple<T>(this List<T> list,
                                            T criteria,
                                            IComparer<T> comparer,
                                            int fixedLeftLimit,
                                            double stepFactor = 0.2)
        {
            // Could handle negative fixedLeftLimit as fixedRightLimit...
            if (fixedLeftLimit < 0) 
                throw new ArgumentException("Can't be negative", nameof(fixedLeftLimit));

            int firstMatch = fixedLeftLimit;
            int step = list.Count / (int)(1.0/stepFactor);

            // Check that the left limit is a match
            if (comparer.Compare(list[fixedLeftLimit], criteria) != 0)
                return new List<T>(); // Return no matches if the leftLimit item is not a match

            int current = firstMatch; // Reset to the initial match
            // Whilst current is not at max value
            while (current + 1 < list.Count)
            {
                // If step exceeds indexes, correct it to fit
                if (current + step >= list.Count)
                    step = list.Count - current - 1;

                // Search from the next step
                var _current = list.BinarySearch(current + step, 1, criteria, comparer);
                
                // Valid match, store and continue to next check
                if (_current >= 0)
                {
                    current = _current;
                    continue;
                }
                
                // Invalid match, reduce step if possible and try again, or else the edge has been found.
                if (_current < 0)
                {
                    if (step > 1) step--; // If no match, reduce step until = 1
                    else break;
                } 
            }
            return list.GetRange(fixedLeftLimit, current - fixedLeftLimit + 1);
        }

        //internal static List<T> BinarySearchMultiple<T>(this List<T> list,
        //                                            T criteria,
        //                                            IComparer<T> comparer,
        //                                            int? fixedLeftLimit,
        //                                            int? fixedRightLimit)
        //{
        //    if (fixedLeftLimit == null && fixedRightLimit == null) 
        //        throw new ArgumentNullException("fixedLeftLimit, fixedRightLimit");

        //    var matches = new List<T>();

        //    int firstMatch = 0;
        //    // Locate any matching item if the first item does not match
        //    // If the first item is not checked manually, it is missed - not sure why
        //    if (comparer.Compare(list[0], criteria) != 0)
        //        firstMatch = list.BinarySearch(criteria, comparer);

        //    // If there are only non-exact matches, there are no matches
        //    if (firstMatch < 0)
        //        return matches;

        //    matches.Add(list[firstMatch]);

        //    // Search Right and Left
        //    int i;
        //    if (fixedLeftLimit == null)
        //        i = -1; // Left
        //    else
        //        i = 1; // Right

        //    int current = firstMatch; // Reset to the initial match
        //                              // Whilst inside the bounds of the list index range
        //    while (current + i >= 0 && current + i < list.Count)
        //    {
        //        // Shift search index (left or right) and check the next
        //        // This could be broken down into a chunked process if the list size is large
        //        var _current = list.BinarySearch(current + i, 1, criteria, comparer);
        //        if (_current < 0) break; // done with this direction if we don't get an exact match

        //        current = _current;
        //        matches.Add(list[current]);
        //    }

        //    if (i == -1 && fixedRightLimit.HasValue) // Left
        //    {
        //        matches.AddRange(list.GetRange(current, fixedRightLimit.Value - current));
        //    } 
        //    else if (i == 1 && fixedLeftLimit.HasValue) // Right
        //    {
        //        matches.AddRange(list.GetRange(fixedLeftLimit.Value, current - fixedLeftLimit.Value + 1));
        //    }

        //    return matches;
        //}
    }
}