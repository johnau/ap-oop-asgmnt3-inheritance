using BinaryFileHandler;
using System.Diagnostics;
using TaskManagerCore.Infrastructure.BinaryFile.Entity;

namespace TaskManagerCore.Infrastructure.BinaryFile.Dao
{
    internal class TaskFolderDao : AbstractDao<TaskFolderEntity>
    {
        public TaskFolderDao(BinaryFileReader<TaskFolderEntity> reader, BinaryFileWriter<TaskFolderEntity> writer) 
            : base(reader, writer)
        {
        }

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

    }
}
