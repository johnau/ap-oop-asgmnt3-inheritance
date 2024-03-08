using TaskManagerCore.Infrastructure.Memory.Entity;
using TaskManagerCore.Model;

namespace TaskManagerCore.Infrastructure.Memory
{
    internal class EntityFactory : IEntityFactory
    {
        internal EntityFactory() { 
        }

        public TaskFolderEntity FromModel(TaskFolder taskFolder)
        {
            return new TaskFolderEntity(taskFolder.Id)
            {
                Name = taskFolder.Name,
                TaskIds = taskFolder.TaskIds,
            };
        }

        public TaskDataEntity FromModel(TaskData taskData)
        {
            if (taskData is HabitualTaskData habitual)
            {
                return new HabitualTaskDataEntity(habitual.Id) // generates a new ID here
                {
                    Description = habitual.Description,
                    Notes = habitual.Notes,
                    Completed = habitual.Completed,
                    DueDate = habitual.DueDate,
                    RepeatingInterval = habitual.RepeatingInterval,
                    Repititions = habitual.Repititions,
                    Streak = habitual.Streak,
                };
            }
            else if (taskData is RepeatingTaskData repeating)
            {
                return new RepeatingTaskDataEntity(repeating.Id) // generates a new ID here
                {
                    Description = repeating.Description,
                    Notes = repeating.Notes,
                    Completed = repeating.Completed,
                    DueDate = repeating.DueDate,
                    RepeatingInterval = repeating.RepeatingInterval,
                    Repititions = repeating.Repititions,
                };
            }
            else
            {
                return new TaskDataEntity(taskData.Id) // generates a new ID here
                {
                    Description = taskData.Description,
                    Notes = taskData.Notes,
                    Completed = taskData.Completed,
                    DueDate = taskData.DueDate,
                };
            }
        }

        //public RepeatingTaskDataEntity FromModel(RepeatingTaskData taskData)
        //{
        //    return new RepeatingTaskDataEntity(taskData.Id) // generates a new ID here
        //    {
        //        Description = taskData.Description,
        //        Notes = taskData.Notes,
        //        Completed = taskData.Completed,
        //        DueDate = taskData.DueDate,
        //        RepeatingInterval = taskData.RepeatingInterval,
        //        Repititions = taskData.Repititions,
        //    };
        //}

        //public HabitualTaskDataEntity FromModel(HabitualTaskData taskData)
        //{
        //    return new HabitualTaskDataEntity(taskData.Id) // generates a new ID here
        //    {
        //        Description = taskData.Description,
        //        Notes = taskData.Notes,
        //        Completed = taskData.Completed,
        //        DueDate = taskData.DueDate,
        //        RepeatingInterval = taskData.RepeatingInterval,
        //        Repititions = taskData.Repititions,
        //        Streak = taskData.Streak,
        //    };
        //}

        public TaskFolder ToModel(TaskFolderEntity taskFolder)
        {
            var id = taskFolder.Id;
            var name = taskFolder.Name;
            var taskIds = new List<string>(taskFolder.TaskIds); // is this the best place to take a copy?
            return new TaskFolder(id, name, taskIds);
        }

        public TaskData ToModel(TaskDataEntity taskDataEntity)
        {
            if (taskDataEntity is HabitualTaskDataEntity)
            {
                var id = taskDataEntity.Id;
                var description = taskDataEntity.Description;
                var notes = taskDataEntity.Notes;
                var completed = taskDataEntity.Completed;
                var dueDate = ((HabitualTaskDataEntity)taskDataEntity).DueDate;
                var interval = ((HabitualTaskDataEntity)taskDataEntity).RepeatingInterval;
                var repititions = ((HabitualTaskDataEntity)taskDataEntity).Repititions;
                var streak = ((HabitualTaskDataEntity)taskDataEntity).Streak;
                return new HabitualTaskData(id, description, notes, completed, dueDate, interval, repititions, streak);
            }
            else if (taskDataEntity is RepeatingTaskDataEntity)
            {
                var id = taskDataEntity.Id;
                var description = taskDataEntity.Description;
                var notes = taskDataEntity.Notes;
                var completed = taskDataEntity.Completed;
                var dueDate = ((RepeatingTaskDataEntity)taskDataEntity).DueDate;
                var interval = ((RepeatingTaskDataEntity)taskDataEntity).RepeatingInterval;
                var repititions = ((RepeatingTaskDataEntity)taskDataEntity).Repititions;
                return new RepeatingTaskData(id, description, notes, completed, dueDate, interval, repititions);
            }
            else
            {
                var id = taskDataEntity.Id;
                var description = taskDataEntity.Description;
                var notes = taskDataEntity.Notes;
                var completed = taskDataEntity.Completed;
                var dueDate = taskDataEntity.DueDate;
                return new TaskData(id, description, notes, completed, dueDate);
            }
        }

        //public RepeatingTaskData ToModel(RepeatingTaskDataEntity taskData)
        //{
        //    var id = taskData.Id;
        //    var description = taskData.Description;
        //    var notes = taskData.Notes;
        //    var completed = taskData.Completed;
        //    var dueDate = taskData.DueDate;
        //    var interval = taskData.RepeatingInterval;
        //    var repititions = taskData.Repititions;
        //    return new RepeatingTaskData(id, description, notes, completed, dueDate, interval, repititions);
        //}

        //public HabitualTaskData ToModel(HabitualTaskDataEntity taskData)
        //{
        //    var id = taskData.Id;
        //    var description = taskData.Description;
        //    var notes = taskData.Notes;
        //    var completed = taskData.Completed;
        //    var dueDate = taskData.DueDate;
        //    var interval = taskData.RepeatingInterval;
        //    var repititions = taskData.Repititions;
        //    var streak = taskData.Streak;
        //    return new HabitualTaskData(id, description, notes, completed, dueDate, interval, repititions, streak);
        //}
    }
}
