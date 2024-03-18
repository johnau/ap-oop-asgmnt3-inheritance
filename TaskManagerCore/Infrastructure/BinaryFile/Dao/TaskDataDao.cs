using BinaryFileHandler;
using System.Diagnostics;
using TaskManagerCore.Infrastructure.BinaryFile.Entity;

namespace TaskManagerCore.Infrastructure.BinaryFile.Dao
{
    internal class TaskDataDao : AbstractDao<TaskDataEntity>
    {
        public TaskDataDao(BinaryFileReader<TaskDataEntity> reader, BinaryFileWriter<TaskDataEntity> writer) 
            : base(reader, writer)
        {
        }

        /// <summary>
        /// Handles Save and Update
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override string Save(TaskDataEntity entity)
        {
            /* 
             * Try Add new Task
             */
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

            /* 
             * Update existing task
             */
            Debug.WriteLine($"Updating Task: {entity.Id} {entity.GetType()}");

            if (!Cache.TryGetValue(entity.Id, out var existing) || existing == null)
                throw new Exception("Missing Task");

            existing.Description = entity.Description;
            existing.Notes = entity.Notes;
            existing.Completed = entity.Completed;
            existing.DueDate = entity.DueDate;

            // update params if is repeating
            // messy - need to clean this up
            if (entity is RepeatingTaskDataEntity repeating)
            {
                var _existing = (RepeatingTaskDataEntity)existing;
                _existing.Completed = false;
                _existing.RepeatingInterval = repeating.RepeatingInterval;
                _existing.Repetitions = repeating.Repetitions;
                existing = _existing;
            }
            // update params again if also habitual
            // messy - need to clean this up
            if (entity is HabitualTaskDataEntity habitual)
            {
                var _existing = (HabitualTaskDataEntity)existing;
                _existing.Streak = habitual.Streak;
                existing = _existing;
            }

            // Notify subscribers about changes - not greatest solution having to manually trigger just for this update
            Cache.MarkDirty();

            return existing.Id;
        }
    }
}
