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

            if (!Cache.TryGetValue(entity.Id, out var existing)) throw new Exception("Missing Folder");
            if (existing == null) throw new Exception("Missing Folder");
            existing.Name = entity.Name;
            existing.TaskIds = entity.TaskIds; // these task ids are not getting written properly here

            Cache.Flush(); // Hacky fix for now to notify subscribers about changes

            return existing.Id;
        }

        public List<TaskFolderEntity> FindByName(string name)
        {
            (var fwdList, var revList) = SortedData(Sort.NAME);

            var searchDescription = new TaskFolderEntity() { Name = name };
            var firstMatch = fwdList.BinarySearch(searchDescription, new TaskFolderName_BeginsWithComparer());
            var lastMatch = ~revList.BinarySearch(searchDescription, new TaskFolderName_BeginsWithComparer());
            lastMatch += revList.Count;

            return SelectFromList(fwdList, firstMatch, lastMatch);

            //var list = Cache.SortedBy(TaskFolderSortingType.NAME+"");
            //return list
            //        .Where(folder => folder.Name.StartsWith(name, StringComparison.OrdinalIgnoreCase))
            //        .ToList();
        }

        /// <summary>
        /// Name is enforced as unique in the Save() method, so we can findOne here
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public TaskFolderEntity? FindOneByName(string name)
        {
            var fwdList = Cache.SortedBy(Sort.NAME + "");

            var searchDescription = new TaskFolderEntity() { Name = name };
            var exactMatchIndex = fwdList.BinarySearch(searchDescription, new TaskFolderName_BeginsWithComparer());

            return fwdList[exactMatchIndex];

            //var list = Cache.SortedBy(TaskFolderSortingType.TASK_COUNT+"");
            //return list
            //        .FirstOrDefault(folder => folder.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        }

        public List<TaskFolderEntity> FindEmpty()
        {
            (var fwdList, var revList) = SortedData(Sort.TASK_COUNT);

            var searchDescription = new TaskFolderEntity() { TaskIds = new List<string>() };
            var firstMatch = fwdList.BinarySearch(searchDescription, new TaskFolderTaskCount_Comparer());
            var lastMatch = ~revList.BinarySearch(searchDescription, new TaskFolderTaskCount_Comparer());
            lastMatch += revList.Count;

            return SelectFromList(fwdList, firstMatch, lastMatch);

            //var list = Cache.SortedBy(TaskFolderSortingType.TASK_COUNT+"");
            //return list
            //        .Where(folder => folder.TaskIds.Count == 0)
            //        .ToList();
        }

        public List<TaskFolderEntity> FindNotEmpty()
        {
            (var fwdList, var revList) = SortedData(Sort.TASK_COUNT);

            var searchDescription = new TaskFolderEntity() { TaskIds = new List<string>() };
            var firstMatch = fwdList.BinarySearch(searchDescription, new TaskFolderTaskCount_Comparer(TaskFolderTaskCount_Comparer.Modifier.MORE_THAN));
            
            // sorted by task_count, so we don't need to check from the other end
            //var lastMatch = ~revList.BinarySearch(searchDescription, new TaskFolderTaskCount_Comparer(TaskFolderTaskCount_Comparer.Modifier.MORE_THAN));
            //lastMatch += revList.Count;

            return SelectFromList(fwdList, firstMatch, fwdList.Count-1);

            //var list = Cache.SortedBy(TaskFolderSortingType.TASK_COUNT + "");
            //return list
            //        .Where(folder => folder.TaskIds.Count > 0)
            //        .ToList();
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
