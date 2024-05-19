using System;
using System.Collections.Generic;
using TaskManagerCore.Infrastructure.BinaryFile.Entity;
using TaskManagerCore.Model;

namespace TaskManagerCore.Infrastructure.BinaryFile
{
    /// <summary>
    /// Static EntityFactory for simplicity
    /// </summary>
    internal static class EntityFactory
    {
        #region Helper Methods For Transporting the GlobalID from other Repository

        /**
         * 
         * Probably a better place for these
         * 
         */

        public static TaskData WithId(TaskData task, string id)
        {
            if (task is HabitualTaskData habitual)
            {
                return new HabitualTaskData(id,
                                            habitual.Description,
                                            habitual.Notes,
                                            habitual.Completed,
                                            habitual.DueDate,
                                            habitual.RepeatingInterval,
                                            habitual.Repetitions,
                                            habitual.Streak
                                            );

            }
            else if (task is RepeatingTaskData repeating)
            {
                return new RepeatingTaskData(id,
                                            repeating.Description,
                                            repeating.Notes,
                                            repeating.Completed,
                                            repeating.DueDate,
                                            repeating.RepeatingInterval,
                                            repeating.Repetitions);
            }
            else
            {
                return new TaskData(id, task.Description, task.Notes, task.Completed, task.DueDate);
            }
        }

        public static TaskFolder WithId(TaskFolder folder, string id)
        {
            return new TaskFolder(id, folder.Name, folder.TaskIds);
        }
        #endregion

        public static TaskFolderEntity FromModel(TaskFolder taskFolder)
        {
            return new TaskFolderEntity(taskFolder.Id)
            {
                Name = taskFolder.Name,
                TaskIds = taskFolder.TaskIds,
            };
        }

        public static List<TaskFolderEntity> FromModel(List<TaskFolder> taskFolders)
        {
            var list = new List<TaskFolderEntity>();
            foreach (var item in taskFolders)
            {
                list.Add(FromModel(item));
            }
            return list;
        }

        public static TaskDataEntity FromModel(TaskData taskData)
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
                    Repetitions = habitual.Repetitions,
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
                    Repetitions = repeating.Repetitions,
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

        public static List<TaskDataEntity> FromModel(List<TaskData> taskDatas)
        {
            var list = new List<TaskDataEntity>();
            foreach (var item in taskDatas)
            {
                list.Add(FromModel(item));
            }
            return list;
        }

        public static TaskDataEntity TaskFromValues(string className, string id, string description, string notes, bool completed, DateTime? dueDate, TimeInterval? interval = null, int? repetitions = null, int? streak = null)
        {
            if (className.Equals(typeof(TaskDataEntity).Name, StringComparison.Ordinal))
            {
                return new TaskDataEntity(id)
                {
                    Description = description,
                    Notes = notes,
                    Completed = completed,
                    DueDate = dueDate,
                };
            }
            else if (className.Equals(typeof(RepeatingTaskDataEntity).Name, StringComparison.Ordinal))
            {
                if (dueDate == null || interval == null || repetitions == null) throw new ArgumentNullException("dueDate, interval, repetitions");
                return new RepeatingTaskDataEntity(id)
                {
                    Description = description,
                    Notes = notes,
                    Completed = completed,
                    DueDate = dueDate.Value,
                    RepeatingInterval = interval.Value,
                    Repetitions = repetitions.Value,
                };
            }
            else if (className.Equals(typeof(HabitualTaskDataEntity).Name, StringComparison.Ordinal))
            {
                if (dueDate == null || interval == null || repetitions == null || streak == null) throw new ArgumentNullException("dueDate, interval, repetitions, streak");
                return new HabitualTaskDataEntity(id)
                {
                    Description = description,
                    Notes = notes,
                    Completed = completed,
                    DueDate = dueDate.Value,
                    RepeatingInterval = interval.Value,
                    Repetitions = repetitions.Value,
                    Streak = streak.Value
                };
            }

            throw new ArgumentException($"className was not valid or recognized: {className}", nameof(className));
        }

        public static TaskFolder ToModel(TaskFolderEntity taskFolder)
        {
            var id = taskFolder.Id;
            var name = taskFolder.Name;
            var taskIds = new List<string>(taskFolder.TaskIds); // is this the best place to take a copy?
            return new TaskFolder(id, name, taskIds);
        }

        public static List<TaskFolder> ToModel(List<TaskFolderEntity> taskFolders)
        {
            var list = new List<TaskFolder>();
            foreach (var item in taskFolders)
            {
                list.Add(ToModel(item));
            }
            return list;
        }

        public static TaskData ToModel(TaskDataEntity entity)
        {
            if (entity is HabitualTaskDataEntity h)
            {
                var dueDate = h.DueDate.HasValue ? h.DueDate.Value : DateTime.MaxValue;
                return new HabitualTaskData(h.Id, h.Description, h.Notes, h.Completed, dueDate, h.RepeatingInterval, h.Repetitions, h.Streak);
            }
            else if (entity is RepeatingTaskDataEntity r)
            {
                var dueDate = r.DueDate.HasValue ? r.DueDate.Value : DateTime.MaxValue;
                return new RepeatingTaskData(r.Id, r.Description, r.Notes, r.Completed, dueDate, r.RepeatingInterval, r.Repetitions);
            }
            else
            {
                return new TaskData(entity.Id, entity.Description, entity.Notes, entity.Completed, entity.DueDate);
            }
        }

        public static List<TaskData> ToModel(List<TaskDataEntity> taskDatas)
        {
            var list = new List<TaskData>();
            foreach (var item in taskDatas)
            {
                list.Add(ToModel(item));
            }
            return list;
        }

        public static List<TaskData> ToModel(List<RepeatingTaskDataEntity> repeatingTaskDatas)
        {
            var list = new List<TaskData>();
            foreach (var item in repeatingTaskDatas)
            {
                list.Add(ToModel(item));
            }
            return list;
        }

        public static List<TaskData> ToModel(List<HabitualTaskDataEntity> habitualTaskDatas)
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
