using System.Diagnostics;
using TaskManagerCore.Infrastructure.BinaryFile.Entity;
using TaskManagerCore.Infrastructure.BinaryFile.FileHandlers;

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
                { "duedate", TaskDataEntity.CompareTasksByDueDate },
                { "completed", TaskDataEntity.CompareTasksByCompleted },
                { "description", TaskDataEntity.CompareTasksByDescription },
                { "notes", TaskDataEntity.CompareTasksByNotes },
            };

        /// <summary>
        /// Handles Save and Update
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override string Save(TaskDataEntity entity)
        {
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

            Cache.Flush(); // Hacky fix for now to notify subscribers about changes
            //Cache.ForceReplace(entity.Id, existing); // This is ugly, but needed to force the call on NotifySubscribers...

            return existing.Id;
        }
    }
}
