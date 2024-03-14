using TaskManagerCore.Infrastructure.BinaryFile.Entity;
using TaskManagerCore.Model;

namespace TaskManagerCore.Infrastructure.BinaryFile
{
    /// <summary>
    /// Split this out into seperate factories?
    /// 
    /// Handles Model -> Entity conversion
    /// Should be made responsible for all casts between different class types
    /// </summary>
    internal class EntityFactory : IEntityFactory
    {
        internal EntityFactory()
        {
        }

        public TaskFolderEntity FromModel(TaskFolder taskFolder)
        {
            return new TaskFolderEntity(taskFolder.Id)
            {
                Name = taskFolder.Name,
                TaskIds = taskFolder.TaskIds,
            };
        }

        public List<TaskFolderEntity> FromModel(List<TaskFolder> taskFolders)
        {
            var list = new List<TaskFolderEntity>();
            foreach (var item in taskFolders)
            {
                list.Add(FromModel(item));
            }
            return list;
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

        public List<TaskDataEntity> FromModel(List<TaskData> taskDatas)
        {
            var list = new List<TaskDataEntity>();
            foreach (var item in taskDatas)
            {
                list.Add(FromModel(item));
            }
            return list;
        }

        public TaskFolder ToModel(TaskFolderEntity taskFolder)
        {
            var id = taskFolder.Id;
            var name = taskFolder.Name;
            var taskIds = new List<string>(taskFolder.TaskIds); // is this the best place to take a copy?
            return new TaskFolder(id, name, taskIds);
        }

        public List<TaskFolder> ToModel(List<TaskFolderEntity> taskFolders)
        {
            var list = new List<TaskFolder>();
            foreach (var item in taskFolders)
            {
                list.Add(ToModel(item));
            }
            return list;
        }

        public TaskData ToModel(TaskDataEntity entity)
        {
            if (entity is HabitualTaskDataEntity h)
            {
                return new HabitualTaskData(h.Id, h.Description, h.Notes, h.Completed, h.DueDate, h.RepeatingInterval, h.Repititions, h.Streak);
            }
            else if (entity is RepeatingTaskDataEntity r)
            {
                return new RepeatingTaskData(r.Id, r.Description, r.Notes, r.Completed, r.DueDate, r.RepeatingInterval, r.Repititions);
            }
            else
            {
                return new TaskData(entity.Id, entity.Description, entity.Notes, entity.Completed, entity.DueDate);
            }
        }

        public List<TaskData> ToModel(List<TaskDataEntity> taskDatas)
        {
            var list = new List<TaskData>();
            foreach (var item in taskDatas)
            {
                list.Add(ToModel(item));
            }
            return list;
        }

        public List<TaskData> ToModel(List<RepeatingTaskDataEntity> repeatingTaskDatas)
        {
            var list = new List<TaskData>();
            foreach (var item in repeatingTaskDatas)
            {
                list.Add(ToModel(item));
            }
            return list;
        }

        public List<TaskData> ToModel(List<HabitualTaskDataEntity> habitualTaskDatas)
        {
            var list = new List<TaskData>();
            foreach (var item in habitualTaskDatas)
            {
                list.Add(ToModel(item));
            }
            return list;
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
