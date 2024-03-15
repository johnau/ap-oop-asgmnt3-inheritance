using System.Diagnostics;
using Sort = TaskManagerCore.Infrastructure.BinaryFile.Dao.Sorting.TaskFolderSortingType;
using TaskManagerCore.Infrastructure.BinaryFile.Dao.Sorting;
using TaskManagerCore.Infrastructure.BinaryFile.Entity;
using TaskManagerCore.Infrastructure.BinaryFile.FileHandlers;
using TaskManagerCore.Infrastructure.BinaryFile.QueryComparers;
using System.Xml.Linq;

namespace TaskManagerCore.Infrastructure.BinaryFile.Dao
{
    internal class TaskFolderDao : AbstractDao<TaskFolderEntity>
    {
        public TaskFolderDao(BinaryFileReader<TaskFolderEntity> reader, BinaryFileWriter<TaskFolderEntity> writer) 
            : base(reader, writer)
        { }

        protected override Dictionary<string, Comparison<TaskFolderEntity>> ComparisonMethods => [];

        public override string Save(TaskFolderEntity entity)
        {
            foreach (var folder in Cache)
            {
                if (folder.Key == entity.Id) continue;

                if (folder.Value.Name.ToLower() == entity.Name.ToLower())
                {
                    throw new InvalidDataException($"There is already a folder called: {entity.Name.ToUpper()}");
                }
            }

            if (Cache.TryAdd(entity.Id, entity))
            {
                Debug.WriteLine($"Saved new Folder: {entity.Id}");
                return entity.Id;
            }

            Debug.WriteLine($"Updating Folder: {entity.Id}");

            // Handling this here isn't the nicest, but to avoid this there needs to be
            // a concrete version of the Cache objects, for each type...
            if (!Cache.TryGetValue(entity.Id, out var existing)) throw new Exception("Missing Folder");
            if (existing == null) throw new Exception("Missing Folder");
            existing.Name = entity.Name;
            existing.TaskIds = entity.TaskIds;

            Cache.MarkDirty(); // Hacky fix for now to notify subscribers about changes

            return existing.Id;
        }

        public List<TaskFolderEntity> FindByName(string name)
        {
            (var fwdList, var revList) = SortedData(Sort.NAME);

            var criteriaObject = new TaskFolderEntity() { Name = name };
            var firstMatch = fwdList.BinarySearch(criteriaObject, new TaskFolderName_BeginsWithComparer());
            var lastMatch = ~revList.BinarySearch(criteriaObject, new TaskFolderName_BeginsWithComparer());
            lastMatch += revList.Count;

            return SelectFromList(fwdList, firstMatch, lastMatch);
        }

        /// <summary>
        /// Name is enforced as unique in the Save() method, so we can findOne here
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public TaskFolderEntity? FindOneByName(string name)
        {
            var fwdList = Cache.SortedBy(Sort.NAME + "");

            var criteriaObject = new TaskFolderEntity() { Name = name };
            var exactMatchIndex = fwdList.BinarySearch(criteriaObject, new TaskFolderName_BeginsWithComparer());

            return fwdList[exactMatchIndex];
        }

        public List<TaskFolderEntity> FindEmpty()
        {
            (var fwdList, var revList) = SortedData(Sort.TASK_COUNT);

            var criteriaObject = new TaskFolderEntity() { TaskIds = new List<string>() };
            var firstMatch = fwdList.BinarySearch(criteriaObject, new TaskFolderTaskCount_Comparer());
            var lastMatch = ~revList.BinarySearch(criteriaObject, new TaskFolderTaskCount_Comparer());
            lastMatch += revList.Count;

            return SelectFromList(fwdList, firstMatch, lastMatch);
        }

        /// <summary>
        /// Finds Non-Empty Folders.
        /// Gets list sorted by task count, finds the first element that has 'more_than' the count of the provided
        /// list in the search criteria
        /// </summary>
        /// <returns></returns>
        public List<TaskFolderEntity> FindNotEmpty()
        {
            var fwdList = Cache.SortedBy(Sort.TASK_COUNT + "");

            var criteriaObject = new TaskFolderEntity() { TaskIds = new List<string>() };
            var more_than = TaskFolderTaskCount_Comparer.Modifier.MORE_THAN;
            var firstMatch = fwdList.BinarySearch(criteriaObject, new TaskFolderTaskCount_Comparer(more_than));

            return SelectFromList(fwdList, firstMatch, fwdList.Count-1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public TaskFolderEntity? FindParentOfTask(string taskId)
        {
            var revList = Cache.SortedBy(Sort.TASK_COUNT + "", reversed: true); // is sort by task count reversed most efficient? ignore searching empties...

            var searchDescription = new TaskFolderEntity() { TaskIds = new List<string>() { taskId } };
            var more_than = TaskFolderTaskCount_Comparer.Modifier.MORE_THAN;
            var match = revList.BinarySearch(searchDescription, new TaskFolderTaskCount_Comparer(more_than));
        } 

        #region Helper methods

        private (List<TaskFolderEntity> fwdList, List<TaskFolderEntity> revList) SortedData(Sort sort)
        {
            var fwdList = Cache.SortedBy(sort + "");
            var revList = Cache.SortedBy(sort + "", true);
            return (fwdList, revList);
        }

        private static List<TaskFolderEntity> SelectFromList(List<TaskFolderEntity> list, int startIndex, int endIndex)
        {
            var subList = new List<TaskFolderEntity>();
            for (int i = startIndex; i <= endIndex; i++)
            {
                subList.Add(list[i]);
            }
            return subList;
        }

        #endregion
    }
}
