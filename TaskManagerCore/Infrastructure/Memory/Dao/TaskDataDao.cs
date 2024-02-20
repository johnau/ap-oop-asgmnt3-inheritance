using System.Diagnostics;
using TaskManagerCore.Configuration;
using TaskManagerCore.Infrastructure.Memory.Entity;

namespace TaskManagerCore.Infrastructure.Memory.Dao
{
    //public class TaskDataDao : ICrudRepository<TaskDataEntity, string>
    public class TaskDataDao : AbstractDao<TaskDataEntity>
    {
        public TaskDataDao()
        {
        }

        //public List<TaskDataEntity> FindAll()
        //{
        //    return new List<TaskDataEntity>(_data.Values);
        //}

        //public TaskDataEntity? FindById(string id)
        //{
        //    return _data[id];
        //}

        /// <summary>
        /// Handles Save and Update
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override string Save(TaskDataEntity entity)
        {
            // Try to add new task
            if (entity is HabitualTaskDataEntity)
            {
                if (_data.TryAdd(entity.Id, (HabitualTaskDataEntity)entity)) 
                    return entity.Id;
            }
            else if (entity is RepeatingTaskDataEntity)
            {
                if (_data.TryAdd(entity.Id, (RepeatingTaskDataEntity)entity))
                    return entity.Id;
            }
            else
            {
                if (_data.TryAdd(entity.Id, entity))
                    return entity.Id;
            }

            // Update existing task
            Debug.WriteLine($"Updating Task: {entity.Id} {entity.GetType()}");
            var existing = _data[entity.Id];
            existing.Description = entity.Description;
            existing.Notes = entity.Notes;
            existing.Completed = entity.Completed;
            existing.DueDate = entity.DueDate;

            var type = entity.GetType();

            if (entity is RepeatingTaskDataEntity)
            {
                var _existing = ((RepeatingTaskDataEntity)existing);
                var _entity = ((RepeatingTaskDataEntity)entity);
                _existing.DueDate = _entity.DueDate;
                _existing.RepeatingInterval = _entity.RepeatingInterval;
                _existing.Repititions = _entity.Repititions;
            }
            if (entity is HabitualTaskDataEntity)
            {
                var _existing = ((HabitualTaskDataEntity)existing);
                var _entity = ((HabitualTaskDataEntity)entity);
                _existing.Streak = _entity.Streak;
            }

            return existing.Id;
        }

        //public bool Delete(string id)
        //{
        //    if (_data.ContainsKey(id))
        //    {
        //        Debug.WriteLine($"Deleting TaskData: {id}");
        //        return _data.Remove(id);
        //    }

        //    Debug.WriteLine($"Can't remove TaskData with Id={id}. It does not exist");
        //    return false;
        //}

    }
}

