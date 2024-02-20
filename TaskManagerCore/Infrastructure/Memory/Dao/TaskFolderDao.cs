using System.Diagnostics;
using TaskManagerCore.Configuration;
using TaskManagerCore.Infrastructure.Memory.Entity;

namespace TaskManagerCore.Infrastructure.Memory.Dao
{
    //public class TaskFolderDao : ICrudRepository<TaskFolderEntity, string>
    public class TaskFolderDao : AbstractDao<TaskFolderEntity>
    {
        public TaskFolderDao()
        {
        }

        //public List<TaskFolderEntity> FindAll()
        //{
        //    return new List<TaskFolderEntity>(_data.Values);
        //}

        //public TaskFolderEntity? FindById(string id)
        //{
        //    return _data[id];
        //}

        public override string Save(TaskFolderEntity entity)
        {
            foreach (var folder in _data)
            {
                if (folder.Key == entity.Id) continue;

                if (folder.Value.Name.ToLower() == entity.Name.ToLower())
                {
                    throw new InvalidDataException($"There is already a folder called: {entity.Name.ToUpper()}");
                }
            }

            if (_data.TryAdd(entity.Id, entity))
            {
                Debug.WriteLine($"Saved new Folder: {entity.Id}");
                return entity.Id;
            }

            Debug.WriteLine($"Updating Folder: {entity.Id}");

            // assume entity is present and Adding did not fail for some other reason
            var existing = _data[entity.Id];
            existing.Name = entity.Name;
            existing.TaskIds = entity.TaskIds; 

            return existing.Id;
        }

        //public bool Delete(string id)
        //{
        //    if (_data.ContainsKey(id))
        //    {
        //        Debug.WriteLine($"Deleting TaskFolder: {id}");
        //        return _data.Remove(id);
        //    }

        //    Debug.WriteLine($"Can't remove TaskFolder with Id={id}. It does not exist");
        //    return false;
        //}

    }
}
