using System.Diagnostics;
using Sort = TaskManagerCore.Infrastructure.BinaryFile.Dao.Sorting.TaskFolderSortingType;
using TaskManagerCore.Infrastructure.BinaryFile.Entity;
using TaskManagerCore.Infrastructure.BinaryFile.FileHandlers;
using TaskManagerCore.Infrastructure.BinaryFile.QueryComparers;

namespace TaskManagerCore.Infrastructure.BinaryFile.Dao
{
    internal class TaskFolderDao : AbstractDao<TaskFolderEntity>
    {
        public TaskFolderDao(BinaryFileReader<TaskFolderEntity> reader, BinaryFileWriter<TaskFolderEntity> writer) 
            : base(reader, writer)
        { }

        /// <summary>
        /// Comparison methods for sortable cache
        /// </summary>
        protected override Dictionary<string, Comparison<TaskFolderEntity>> ComparisonMethods => 
            new Dictionary<string, Comparison<TaskFolderEntity>>()
            {
                { Sort.NAME.ToString(), TaskFolderEntity.CompareFoldersByName },
                { Sort.TASK_COUNT.ToString(), TaskFolderEntity.CompareFoldersByTaskCount },
            };

        /// <summary>
        /// Save or Update entity method
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="InvalidDataException"></exception>
        /// <exception cref="Exception"></exception>
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
            // a concrete version of the Cache classes for each type...
            if (!Cache.TryGetValue(entity.Id, out var existing)) throw new Exception("Missing Folder");
            if (existing == null) throw new Exception("Missing Folder");
            existing.Name = entity.Name;
            existing.TaskIds = entity.TaskIds;

            Cache.MarkDirty(); // Hacky fix for now to notify subscribers about changes (and trigger re-sorting)

            return existing.Id;
        }

        /// <summary>
        /// !BinarySearch method to suit requirements of assignment
        /// Find folders matching or starting with provided name string
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public List<TaskFolderEntity> FindByName(string name)
        {
            // Ascending sort by name
            var sortedList = Cache.SortedBy(Sort.NAME + "");
            var criteriaObject = new TaskFolderEntity() { Name = name };
            // Find all matching with StartsWith comparer
            return sortedList.BinarySearchMultiple(criteriaObject, new FolderNameComparer_StartsWith());
        }

        /// <summary>
        /// !BinarySearch method to suit requirements of assignment
        /// Find folder with exact match 
        /// Folder names are unique (enforced by Save() method)
        /// </summary>
        /// <param name="name"></param>
        /// <returns>List of matched TaskFolderEntity classes</returns>
        public TaskFolderEntity? FindOneByName(string name)
        {
            // Ascending sort by name
            var sortedList = Cache.SortedBy(Sort.NAME + "");
            var criteriaObject = new TaskFolderEntity() { Name = name };
            // Use BinarySearch to find a unique match (Folder.Name is a unique (case-insensitive) property)
            var exactMatchIndex = sortedList.BinarySearch(criteriaObject, new FolderNameComparer());

            return sortedList[exactMatchIndex];
        }

        /// <summary>
        /// !BinarySearch method to suit requirements of assignment
        /// Find all Empty folders
        /// </summary>
        /// <returns>List of matched TaskFolderEntity classes</returns>
        public List<TaskFolderEntity> FindEmpty()
        {
            // Ascending order, empty folders will be at the start
            var sortedList = Cache.SortedBy(Sort.TASK_COUNT + "");
            var criteriaObject = TaskFolderEntity.BLANK;

            // Use match and expand
            var matches = sortedList.BinarySearchMultiple(criteriaObject, new FolderTaskCountComparer(), 0);
            return matches;
        }

        /// <summary>
        /// !BinarySearch method to suit requirements of assignment
        /// Finds all Non-Empty Folders.
        /// Inverses the FindNotEmpty method
        /// </summary>
        /// <returns>List of matched TaskFolderEntity classes</returns>
        public List<TaskFolderEntity> FindNotEmpty()
        {
            // Ascending order (Must be same sort order as FindEmpty() method)
            var sortedList = Cache.SortedBy(Sort.TASK_COUNT + "");
            var emptyCount = FindEmpty().Count;
            return sortedList.GetRange(emptyCount, sortedList.Count - emptyCount);
        }

        /// <summary>
        /// Find the parent folder of a task
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns>Matched TaskFolderEntity classe or null</returns>
        public TaskFolderEntity? FindParentOfTask(string taskId)
        {
            // Descending order by Task Count, check the bigger folders first
            var sortedList = Cache.SortedBy(Sort.TASK_COUNT + "", reversed: true);
            foreach (var item in sortedList)
            {
                if (item.TaskIds.Contains(taskId))
                    return item;
            }

            return null;
        }
    }
}
