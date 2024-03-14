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
            //foreach (var task in Cache)
            //{
            //    if (task.Key == entity.Id) continue;
            //    if (task.Value.Description.Equals(entity.Description, StringComparison.OrdinalIgnoreCase))
            //    {
            //        throw new InvalidDataException($"There is already a task with description: {entity.Description.ToUpper()}");
            //    }
            //}

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

            
            // this isn't very nice here, but would need to abstract SubscribeableCache to move it
            if (!Cache.TryGetValue(entity.Id, out var existing)) throw new Exception("Missing Task");
            if (existing == null) throw new Exception("Missing Task");
            existing.Description = entity.Description;
            existing.Notes = entity.Notes;
            existing.Completed = entity.Completed;
            existing.DueDate = entity.DueDate;

            //var type = entity.GetType(); // Better to use for more specific type matching? using is needs to be done in specific order in some cases, but below is not one of those cases

            if (entity is RepeatingTaskDataEntity repeating) // update params if is repeating
            {
                // messy - temporary variables wasting memory?
                var _existing = (RepeatingTaskDataEntity)existing;
                _existing.Completed = false;
                _existing.DueDate = repeating.DueDate;
                _existing.RepeatingInterval = repeating.RepeatingInterval;
                _existing.Repititions = repeating.Repititions;
                existing = _existing;
            }
            if (entity is HabitualTaskDataEntity habitual) // update params again if also habitual
            {
                var _existing = (HabitualTaskDataEntity)existing;
                _existing.Streak = habitual.Streak;
                existing = _existing;
            }

            Cache.Flush(); // Hacky fix for now to notify subscribers about changes (Extremely jank fix)
            //Cache.ForceReplace(entity.Id, existing); // This is ugly, but needed to force the call on NotifySubscribers...

            return existing.Id;
        }

        /// <summary>
        /// Assessed Task 5 Method
        /// Conducting a binary search for item by due date
        /// </summary>
        /// <param name="dueDate"></param>
        /// <returns></returns>
        public List<TaskDataEntity> FindByDueDate(DateTime dueDate)
        {
            //var fwdList = Cache.SortedBy(Sort.DUE_DATE+"");
            //var revList = Cache.SortedBy(Sort.DUE_DATE + "", true);
            (var fwdList, var revList) = SortedData(Sort.DUE_DATE);

            // is this better here, or inside the cache object (Searchable cache implementation)
            var searchDate = new TaskDataEntity() { DueDate = dueDate };
            var firstMatch = fwdList.BinarySearch(searchDate, new TaskDataDueDate_DateOnlyComparer());
            var lastMatch = ~revList.BinarySearch(searchDate, new TaskDataDueDate_DateOnlyComparer());
            lastMatch += revList.Count;
            // Note to self: ^ Bitwise Not operator, index is flipped to negative number - 1.
            // ie. value of 2 00000010 is flipped to 11111101 which is -3 (since 00000000 is 0, and 11111111 is -1, etc)
            // Which as a list index, counts backwards from end, however we adjust for the loop, or is there a fancy way to 
            // do that in the loop...

            return SelectFromList(fwdList, firstMatch, lastMatch);

            //var subList = new List<TaskDataEntity>();
            //for (int i = firstMatch; i <= lastMatch; i++)
            //{
            //    subList.Add(fwdList[i]);
            //}

            //return subList;

            // With LINQ
            //return list
            //        .Where(task => task.DueDate.HasValue && task.DueDate.Value.Date == dueDate.Date)
            //        .ToList();
        }

        public List<TaskDataEntity> FindByDescription(string description)
        {
            //var fwdList = Cache.SortedBy(Sort.DESCRIPTION + "");
            //var revList = Cache.SortedBy(Sort.DESCRIPTION + "", true);
            (var fwdList, var revList) = SortedData(Sort.DESCRIPTION);

            var searchDescription = new TaskDataEntity() { Description = description };
            var firstMatch = fwdList.BinarySearch(searchDescription, new TaskDataDescription_BeginsWithComparer());
            var lastMatch = ~revList.BinarySearch(searchDescription, new TaskDataDescription_BeginsWithComparer());
            lastMatch += revList.Count;

            return SelectFromList(fwdList, firstMatch, lastMatch);

            //var subList = new List<TaskDataEntity>();
            //for (int i = firstMatch; i <= lastMatch; i++)
            //{
            //    subList.Add(fwdList[i]);
            //}

            //return subList;

            //return fwdList
            //        .Where(task => task.Description.StartsWith(description, StringComparison.OrdinalIgnoreCase))
            //        .ToList();
        }

        ///// <summary>
        ///// Description is NOT enforced to be unique for tasks, so we shouldn't check for One
        ///// </summary>
        ///// <param name="description"></param>
        ///// <returns></returns>
        //public TaskDataEntity? FindOneByDescription(string description)
        //{
        //    //var fwdList = Cache.SortedBy(Sort.DESCRIPTION + "");
        //    //var revList = Cache.SortedBy(Sort.DESCRIPTION + "", true);
        //    //(var fwdList, var revList) = SortedData(Sort.DESCRIPTION);
        //    var fwdList = Cache.SortedBy(Sort.DESCRIPTION + "");

        //    var searchDescription = new TaskDataEntity() { Description = description };
        //    var firstMatch = fwdList.BinarySearch(searchDescription, new TaskDataDescription_Comparer());
        //    //var lastMatch = ~revList.BinarySearch(searchDescription, new TaskDataDescription_Comparer());
        //    //lastMatch += revList.Count;

            

        //    //return list
        //    //        .FirstOrDefault(task => task.Description.Equals(description, StringComparison.OrdinalIgnoreCase));
        //}

        public List<TaskDataEntity> FindByNotes(string notes)
        {
            //var list = Cache.SortedBy(Sort.NOTES + "");
            (var fwdList, var revList) = SortedData(Sort.NOTES);

            var searchDescription = new TaskDataEntity() { Notes = notes };
            var firstMatch = fwdList.BinarySearch(searchDescription, new TaskDataNotes_ContainsComparer());
            var lastMatch = ~revList.BinarySearch(searchDescription, new TaskDataNotes_ContainsComparer());
            lastMatch += revList.Count;

            return SelectFromList(fwdList, firstMatch, lastMatch);

            //return list
            //        .Where(task => task.Notes.Contains(notes, StringComparison.OrdinalIgnoreCase))
            //        .ToList();
        }

        public List<TaskDataEntity> FindByCompleted(bool completed)
        {
            (var fwdList, var revList) = SortedData(Sort.COMPLETED);

            var searchDescription = new TaskDataEntity() { Completed = completed };
            var firstMatch = fwdList.BinarySearch(searchDescription, new TaskDataCompleted_Comparer());
            var lastMatch = ~revList.BinarySearch(searchDescription, new TaskDataCompleted_Comparer());
            lastMatch += revList.Count;

            return SelectFromList(fwdList, firstMatch, lastMatch);

            //var list = Cache.SortedBy(Sort.COMPLETED + "");
            //return list
            //        .Where(task => task.Completed == completed)
            //        .ToList();
        }

        #region Helper methods

        private (List<TaskDataEntity> fwdList, List<TaskDataEntity> revList) SortedData(Sort sort)
        {
            var fwdList = Cache.SortedBy(sort + "");
            var revList = Cache.SortedBy(sort + "", true);
            return (fwdList, revList);
        }

        private static List<TaskDataEntity> SelectFromList(List<TaskDataEntity> list, int startIndex, int endIndex)
        {
            var subList = new List<TaskDataEntity>();
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
