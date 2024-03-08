using System.Diagnostics;
using TaskManagerCore.Infrastructure.Memory.Entity;

namespace TaskManagerCore.Infrastructure.Memory.Dao
{
    //public class TaskDataDao : ICrudRepository<TaskDataEntity, string>
    public class TaskDataDao : AbstractDao<TaskDataEntity>
    {
        public TaskDataDao() { }

        /// <summary>
        /// Handles Save and Update
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override string Save(TaskDataEntity entity)
        {
            // Try to add new task

            if (entity is HabitualTaskDataEntity habitualEntity)
            {
                if (InMemoryData.TryAdd(entity.Id, habitualEntity)) 
                    return entity.Id;
            }
            else if (entity is RepeatingTaskDataEntity repeatingEntity)
            {
                if (InMemoryData.TryAdd(entity.Id, repeatingEntity))
                    return entity.Id;
            }
            else
            {
                if (InMemoryData.TryAdd(entity.Id, entity))
                    return entity.Id;
            }

            // Update existing task

            Debug.WriteLine($"Updating Task: {entity.Id} {entity.GetType()}");

            var existing = InMemoryData[entity.Id];
            existing.Description = entity.Description;
            existing.Notes = entity.Notes;
            existing.Completed = entity.Completed;
            existing.DueDate = entity.DueDate;

            //var type = entity.GetType(); // Better to use for more specific type matching?

            if (entity is RepeatingTaskDataEntity repeating) // update params if is repeating
            {
                var _existing = ((RepeatingTaskDataEntity)existing);
                var _entity = repeating;
                _existing.DueDate = _entity.DueDate;
                _existing.RepeatingInterval = _entity.RepeatingInterval;
                _existing.Repititions = _entity.Repititions;
            }
            if (entity is HabitualTaskDataEntity habitual) // update params again if also habitual
            {
                var _existing = ((HabitualTaskDataEntity)existing);
                var _entity = habitual;
                _existing.Streak = _entity.Streak;
            }

            return existing.Id;
        }
    }
}

