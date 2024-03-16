using System.Diagnostics;
using Sort = TaskManagerCore.Infrastructure.BinaryFile.Dao.Sorting.TaskSortingType;
using TaskManagerCore.Infrastructure.BinaryFile.Entity;
using TaskManagerCore.Infrastructure.BinaryFile.FileHandlers;
using TaskManagerCore.Infrastructure.BinaryFile.QueryComparers;

namespace TaskManagerCore.Infrastructure.BinaryFile.Dao
{
    internal class TaskDataDao : AbstractDao<TaskDataEntity>
    {
        public TaskDataDao(BinaryFileReader<TaskDataEntity> reader, BinaryFileWriter<TaskDataEntity> writer) 
            : base(reader, writer)
        { }

        protected override Dictionary<string, Comparison<TaskDataEntity>> ComparisonMethods => 
            new Dictionary<string, Comparison<TaskDataEntity>>() 
            {
                { Sort.DUE_DATE.ToString(), TaskDataEntity.CompareTasksByDueDate },
                { Sort.COMPLETED.ToString(), TaskDataEntity.CompareTasksByCompleted },
                { Sort.DESCRIPTION.ToString(), TaskDataEntity.CompareTasksByDescription },
                { Sort.NOTES.ToString(), TaskDataEntity.CompareTasksByNotes },
            };

        /// <summary>
        /// Handles save and update
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="InvalidDataException"></exception>
        /// <exception cref="Exception">Throw exception if unique description is violated</exception>
        public override string Save(TaskDataEntity entity)
        {
            // Try to add new task
            if (entity is HabitualTaskDataEntity habitualEntity) // TODO: move all type checks and casts to a single class in each layer.
            {
                if (Cache.TryAdd(entity.Id, habitualEntity))
                    return entity.Id;
            }
            else if (entity is RepeatingTaskDataEntity repeatingEntity)
            {
                if (Cache.TryAdd(entity.Id, repeatingEntity))
                    return entity.Id;
            }
            else
            {
                if (Cache.TryAdd(entity.Id, entity))
                    return entity.Id;
            }

            // Update existing task
            Debug.WriteLine($"Updating Task: {entity.Id} {entity.GetType()}");

            // Handling this here isn't the nicest, but to avoid this there needs to be
            // a concrete version of the Cache objects, for each type...
            if (!Cache.TryGetValue(entity.Id, out var existing)) throw new Exception("Missing Task");
            if (existing == null) throw new Exception("Missing Task");
            existing.Description = entity.Description;
            existing.Notes = entity.Notes;
            existing.Completed = entity.Completed;
            existing.DueDate = entity.DueDate;

            //var type = entity.GetType(); // Better to use for more specific type matching? using is needs to be done in specific order in some cases, but below is not one of those cases

            if (entity is RepeatingTaskDataEntity repeating) // update params if is repeating
            {
                // messy - need to clean this up
                var _existing = (RepeatingTaskDataEntity)existing;
                _existing.Completed = false;
                _existing.DueDate = repeating.DueDate;
                _existing.RepeatingInterval = repeating.RepeatingInterval;
                _existing.Repititions = repeating.Repititions;
                existing = _existing;
            }
            if (entity is HabitualTaskDataEntity habitual) // update params again if also habitual
            {
                // mess - need to clean this up
                var _existing = (HabitualTaskDataEntity)existing;
                _existing.Streak = habitual.Streak;
                existing = _existing;
            }

            Cache.MarkDirty(); // Hacky fix for now to notify subscribers about changes (Extremely jank fix)
            //Cache.ForceReplace(entity.Id, existing); // This is ugly, but needed to force the call on NotifySubscribers...

            return existing.Id;
        }

        /// <summary>
        /// Assessed Task 5 Method
        /// Conducting a binary search for item by due date
        /// 
        /// BinarySearch returns a zero-based index of the item matched
        /// OR
        /// A bitwise NOT ~ compliment of the index of the NEXT element that is larger than the item
        /// OR
        /// If there is no larger element, a bitwise complement ~ of Count.
        /// ~
        /// Note to self: ^ Bitwise Not operator, index is flipped to negative number - 1.
        /// ie. value of 2 00000010 is flipped to 11111101 which is -3 (since 00000000 is 0, and 11111111 is -1, etc)
        /// Which as a list index, counts backwards from end, however we adjust for the loop, or is there a fancy way to 
        /// do that in the loop...
        /// 
        /// </summary>
        /// <param name="dueDate"></param>
        /// <returns></returns>
        public List<TaskDataEntity> FindByDueDate(DateTime dueDate)
        {
            var sortedList = Cache.SortedBy(Sort.DUE_DATE + "");

            // is this better here, or inside the cache object (Searchable cache implementation)
            var criteriaStart = new TaskDataEntity() { DueDate = dueDate.Date };
            var firstMatch = sortedList.BinarySearch(criteriaStart, new TaskDueComparer());
            if (firstMatch < 0) // handle inexact matches
            {
                firstMatch = ~firstMatch;
                if (firstMatch == sortedList.Count)
                {
                    Debug.WriteLine($"DateSearch[Lower]: No match, and none in the list are larger");
                    return new List<TaskDataEntity>();
                }
                else
                {
                    Debug.WriteLine($"DateSearch[Lower]: No exact match, closest larger element is at {firstMatch}");
                }
            }

            // Match the following day date (and then use the previous list index)
            var remaining = sortedList.Count - firstMatch;
            var criteriaEnd = new TaskDataEntity() { DueDate = dueDate.AddDays(1).Date };
            var matchNextDay = sortedList.BinarySearch(firstMatch, remaining, criteriaEnd, new TaskDueComparer());
            if (matchNextDay < 0) // handle inexact matches
            {
                matchNextDay = ~matchNextDay;
                if (matchNextDay == sortedList.Count)
                    Debug.WriteLine($"DateSearch[Upper]: No match, and none in the list are larger");
                else
                    Debug.WriteLine($"DateSearch[Upper]: No exact match, closest larger element is at {firstMatch}");
            }

            // to implement this on the reverse list, we search backwards, flip if required, and subtract from list.Count?

            return SelectFromList(sortedList, firstMatch, matchNextDay - 1);
        }

        /// <summary>
        /// When using a custom comparison, and when forcing a matched return of 0, the BinarySearch
        /// method starts to behave not as nicely.  The index matched is essentially random within the
        /// range of matches.
        /// ie. in the following list
        /// 0: Alpha
        /// 1: Bravo 1
        /// 2: Bravo 2
        /// 3: Bravo 3
        /// 4: Charlie
        /// 5: Delta 1
        /// 6: Delta 2
        /// If we want to match "Bravo", we may get index 1, 2, or 3 as the result. Meaning we must
        /// look either direction from the match to get the range of matches. (Providing it is still
        /// matching within a logic in-line with the sort algo)
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public List<TaskDataEntity> FindByDescription(string description)
        {
            var sortedList = Cache.SortedBy(Sort.DESCRIPTION + "");
            //(var sortedList, var revSortedList) = SortedData(Sort.DESCRIPTION);
            // could use the reversed list if description starts after M
            var criteria = new TaskDataEntity() { Description = description };
            return sortedList.BinarySearchFindAll(criteria, new TaskDescriptionComparer_StartsWith());

            //// Adding a buffer because otherwise doesnt match the first entry...
            //// This is inefficient though, an insert is undeseriable, shifting every element [O(n)]
            //// This being needed and the difference between Notes and Description means I'm missing something.
            //sortedList.Insert(0, new TaskDataEntity(""));

            //var criteria = new TaskDataEntity() { Description = description };
            //var matches = new List<TaskDataEntity>();
            
            //var firstMatch = sortedList.BinarySearch(criteria, new TaskDescriptionStartsWithComparer());
            //if (firstMatch < 0)
            //{
            //    return matches; // no matches
            //}
                
            //matches.Add(sortedList[firstMatch]);

            ////var remaining = sortedList.Count - firstMatch;
            ////var firstNonMatch = sortedList.BinarySearch(Math.Min(firstMatch+1, sortedList.Count-1), remaining, criteria, new TaskDescriptionBeginsWithComparer(false));
            ////var lastMatch = firstNonMatch - 1;

            ////return SelectFromList(sortedList, firstMatch, lastMatch);

            //int current = firstMatch;
            //while (current >= firstMatch && current+1 < sortedList.Count)
            //{
            //    current = sortedList.BinarySearch(++current, 1, criteria, new TaskDescriptionStartsWithComparer()); // can probably be made more efficient
            //    if (current >= 0)
            //        matches.Add(sortedList[current]);
            //}
            ////Change the comparer so that we can also search for when the description doesnt match

            //return matches;
        }

        public List<TaskDataEntity> FindByNotes(string notes)
        {
            var sortedList = Cache.SortedBy(Sort.NOTES + "");
            //(var sortedList, var revSortedList) = SortedData(Sort.NOTES);

            var criteria = new TaskDataEntity() { Notes = notes };
            return sortedList.BinarySearchFindAll(criteria, new TaskNotesComparer_StartsWith());
            //var matches = new List<TaskDataEntity>();

            //// Why does the BinarySearch return the last element with the below search.
            //// Above, Description is search in the same way but the first element is found
            //// Both Description and Notes are effectively the same, both strings with no constraints
            //// Both are sorted the same way (string.Compare(IgnoreCase)), handle same way, etc
            //// Both comparers are set up in the same way
            //// Description matches the start of the range, and the method moves forwards, as expected
            //// Notes matches the end of the range, and the method must move backwards, Why is this?
            //// Can confirm both sortedLists appear as expected.
            //var lastMatchIndex = sortedList.BinarySearch(criteria, new TaskNotesStartsWithComparer());
            //if (lastMatchIndex < 0) // no matches, exit
            //{
            //    return new List<TaskDataEntity>();
            //}
            //matches.Add(sortedList[lastMatchIndex]);

            //int current = lastMatchIndex;
            //while (current <= lastMatchIndex && current-1 >= 0)
            //{
            //    current = sortedList.BinarySearch(--current, 1, criteria, new TaskNotesStartsWithComparer());

            //    // No more matches
            //    if (current < 0) break; 

            //    matches.Add(sortedList[current]);
            //}

            //return matches;
        }

        public List<TaskDataEntity> FindByCompleted(bool completed)
        {
            var sortedList = Cache.SortedBy(Sort.COMPLETED + "");
            //(var sortedList, var revList) = SortedData(Sort.COMPLETED);
            //var matches = new List< TaskDataEntity>();
            var criteria = new TaskDataEntity() { Completed = completed };

            //var firstMatch = sortedList.BinarySearch(criteria, new TaskCompletedComparer());

            return sortedList.BinarySearchFindAll(criteria, new TaskCompletedComparer());

            //return [];
            //var lastMatch = ~revList.BinarySearch(criteria, new TaskCompletedComparer());
            //lastMatch += revList.Count;

            //return SelectFromList(sortedList, firstMatch, lastMatch);

            //var list = Cache.SortedBy(Sort.COMPLETED + "");
            //return list
            //        .Where(task => task.Completed == completed)
            //        .ToList();
        }

        #region Helper methods

        private (List<TaskDataEntity> sorted, List<TaskDataEntity> reversedSorted) SortedData(Sort sort)
        {
            var sorted = Cache.SortedBy(sort + "");
            var reversed = Cache.SortedBy(sort + "", reversed: true);
            return (sorted, reversed);
        }

        private static List<TaskDataEntity> SelectFromList(List<TaskDataEntity> list, int startIndex, int endIndex)
        {
            var subList = new List<TaskDataEntity>();
            if (startIndex < 0 || endIndex < 0) return subList;
            if (startIndex >= list.Count || endIndex >= list.Count) return subList;

            for (int i = startIndex; i <= endIndex; i++)
            {
                subList.Add(list[i]);
            }
            return subList;
        }

        #endregion

        //public List<RepeatingTaskDataEntity> FindByInterval(int interval)
        //{
        //    var list = Cache.SortedBy(Sort.INTERVAL + "");
        //    return list
        //            .Where(task => task.)
        //}

        //public List<HabitualTaskDataEntity> FindByHasStreak(bool hasStreak)
        //{
        //    var list = Cache.SortedBy(Sort.STREAK + "");

        //    throw new NotImplementedException();
        //}
    }
}
