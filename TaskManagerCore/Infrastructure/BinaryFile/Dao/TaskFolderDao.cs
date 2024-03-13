using System.Diagnostics;
using TaskManagerCore.Infrastructure.BinaryFile.Entity;
using TaskManagerCore.Infrastructure.BinaryFile.FileHandlers;

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
    }
}
