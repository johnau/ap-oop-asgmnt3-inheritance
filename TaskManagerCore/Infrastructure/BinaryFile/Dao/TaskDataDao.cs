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

        /// <summary>
        /// Sorting functions
        /// </summary>
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
        /// !BinarySearch method to suit requirements of assignment
        /// 
        /// BinarySearch returns a zero-based index of the item matched
        /// OR
        /// A bitwise NOT ~ compliment of the index of the NEXT element that is larger than the item
        /// OR
        /// If there is no larger element, a bitwise complement (~) of collection.Count.
        /// ~
        /// Note to self: ^ Bitwise Not operator, index is flipped to negative number - 1.
        /// ie. value of 2 00000010 is flipped to 11111101 which is -3 (since 00000000 is 0, and 11111111 is -1, etc)
        /// As a list index, a negative number counts back from the end, and the shift of -1 accounts for 0 index
        /// 
        /// </summary>
        /// <param name="dueDate"></param>
        /// <returns>List of matched TaskDataEntity classes</returns>
        public List<TaskDataEntity> FindByDueDate(DateTime dueDate)
        {
            // Ascending sort, oldest dates at the beginning
            var sortedList = Cache.SortedBy(Sort.DUE_DATE + "");
            var criteriaStart = new TaskDataEntity() { DueDate = dueDate.Date };

            // Match the first date
            var firstMatch = sortedList.BinarySearch(criteriaStart, new TaskDueComparer());
            if (firstMatch < 0) // Handle inexact matches
            {
                // Bitwise Not, inexact matches are 
                firstMatch = ~firstMatch;
                // No matches found, nothing larger in list
                if (firstMatch == sortedList.Count) 
                    return new List<TaskDataEntity>();
            }

            // Match the following day date
            var remaining = sortedList.Count - firstMatch;
            var criteriaEnd = new TaskDataEntity() { DueDate = dueDate.AddDays(1).Date };

            // Search from firstMatch for next day date to find end of current day dates
            var matchNextDay = sortedList.BinarySearch(firstMatch, remaining, criteriaEnd, new TaskDueComparer());
            // Bitwise Not if we have negative index, since it will be index of next larger
            if (matchNextDay < 0) matchNextDay = ~matchNextDay; // next largest as boundary
            
            // Return the items from firstMatch to item before next days items
            return sortedList.GetRange(firstMatch, matchNextDay - firstMatch);
        }

        /// <summary>
        /// Find Tasks with description that equals or starts with provided description string
        /// </summary>
        /// <param name="description"></param>
        /// <returns>List of matched TaskDataEntity classes</returns>
        public List<TaskDataEntity> FindByDescription(string description)
        {
            // Ascending sort, A-Z, 0-9
            var sortedList = Cache.SortedBy(Sort.DESCRIPTION + "");
            var criteria = new TaskDataEntity() { Description = description };
            
            // Use Extension method to get range with irregular comparer
            return sortedList.BinarySearchMultiple(criteria, new TaskDescriptionComparer_StartsWith());
        }

        /// <summary>
        /// Find all tasks with Notes that Start With the search string
        /// </summary>
        /// <param name="notes"></param>
        /// <returns>List of matched TaskDataEntity classes</returns>
        public List<TaskDataEntity> FindByNotes(string notes)
        {
            // Ascending sort, A-Z, 0-9
            var sortedList = Cache.SortedBy(Sort.NOTES + "");
            var criteria = new TaskDataEntity() { Notes = notes };

            // Use Extension method to get range with irregular comparer
            return sortedList.BinarySearchMultiple(criteria, new TaskNotesComparer_StartsWith());
        }

        /// <summary>
        /// Finds all tasks, either complete or not complete
        /// </summary>
        /// <param name="completed"></param>
        /// <returns>List of matched TaskDataEntity classes</returns>
        public List<TaskDataEntity> FindByCompleted(bool completed)
        {
            /* 
             // This will effectively check every single matching element... and for bool that means too many checks
             var sortedList = Cache.SortedBy(Sort.COMPLETED+"", !completed);
             var criteria = new TaskDataEntity() { Completed = completed };
             return sortedList.BinarySearchMultiple(criteria, new TaskCompletedComparer(completed));
            */

            // Ascending (false -> true) or Descending sort (true -> false)
            // + Finding Completed=True, list will start with true
            // + Finding Completed=False, list will start with false
            var sortedList = Cache.SortedBy(Sort.COMPLETED + "", completed);
            var criteria = new TaskDataEntity() { Completed = completed };
            // Slightly optimized - will step right, in chunks, until it finds the edge (roughly),
            // then narrows down the exact edge
            return sortedList.BinarySearchMultiple(criteria, new TaskCompletedComparer(completed), 0);
        }


        /*
        public List<RepeatingTaskDataEntity> FindByInterval(int interval)
        {
            var list = Cache.SortedBy(Sort.INTERVAL + "");
            return list
                    .Where(task => task.)
        }

        public List<HabitualTaskDataEntity> FindByHasStreak(bool hasStreak)
        {
            var list = Cache.SortedBy(Sort.STREAK + "");

            throw new NotImplementedException();
        }
        */
    }
}
