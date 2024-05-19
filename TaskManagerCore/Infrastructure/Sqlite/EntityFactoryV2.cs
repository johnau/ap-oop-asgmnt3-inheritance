using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using TaskManagerCore.Infrastructure.Sqlite.Entity;
using TaskManagerCore.Model;

namespace TaskManagerCore.Infrastructure.Sqlite
{
    internal class EntityFactoryV2
    {
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
            } else
            {
                return new TaskData(id, task.Description, task.Notes, task.Completed, task.DueDate);
            }
        }

        public static TaskFolder WithId(TaskFolder folder, string id)
        {
            return new TaskFolder(id, folder.Name, folder.TaskIds);
        }

        public static List<TaskData> ToModel(List<TaskDataEntityV2> entities)
        {
            var modelObjects = new List<TaskData>();

            foreach (var entity in entities)
            {
                modelObjects.Add(ToModel(entity));
            }

            return modelObjects;
        }

        public static TaskData ToModel(TaskDataEntityV2 entity)
        {
            var model = new TaskData(entity.GlobalId, entity.Description, entity.Notes, entity.Completed, entity.DueDate);

            if (entity.RepeatingInterval > 0)
            {
                if (!entity.DueDate.HasValue)
                    throw new Exception("Repeating task should have a DueDate");
                
                model = new RepeatingTaskData(entity.GlobalId, 
                                                entity.Description, 
                                                entity.Notes, 
                                                entity.Completed, 
                                                entity.DueDate.Value, 
                                                (TimeInterval)entity.RepeatingInterval, 
                                                (int)entity.Repetitions
                                                );
            }

            if (entity.Streak >= 0)
            {
                if (!entity.DueDate.HasValue)
                    throw new Exception("Repeating task should have a DueDate");

                model = new HabitualTaskData(entity.GlobalId, 
                                                entity.Description, 
                                                entity.Notes, 
                                                entity.Completed, 
                                                entity.DueDate.Value, 
                                                (TimeInterval)entity.RepeatingInterval, 
                                                (int)entity.Repetitions, 
                                                (int)entity.Streak
                                                );
            }

            return model;
        }

        public static TaskDataEntityV2 FromModel(TaskData model)
        {
            var entity = new TaskDataEntityV2(model.Id);
            entity.Description = model.Description;
            entity.DueDate = model.DueDate;
            entity.Notes = model.Notes;
            entity.Completed = model.Completed;

            if (model is RepeatingTaskData repeating)
            {
                entity.RepeatingInterval = (int)repeating.RepeatingInterval;
                entity.Repetitions = (int)repeating.Repetitions;
            }
            
            if (model is HabitualTaskData habitual)
            {
                entity.Streak = (int)habitual.Streak;
            }

            return entity;
        }

        public static List<TaskFolder> ToModel(List<TaskFolderEntityV2> entities)
        {
            var modelObjects = new List<TaskFolder>();
            foreach (var entity in entities)
            {
                modelObjects.Add(ToModel(entity));
            }

            return modelObjects;
        }

        public static TaskFolder ToModel(TaskFolderEntityV2 entity)
        {
            var taskIds = entity.Tasks
                .Select(t => t.GlobalId)
                .ToList();

            //return new TaskFolder(entity.GlobalId, entity.Name, entity.TaskIds);
            return new TaskFolder(entity.GlobalId, entity.Name, taskIds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Need to incorporate the entity refs
        /// </remarks>
        /// <param name="model"></param>
        /// <returns></returns>
        public static TaskFolderEntityV2 FromModel(TaskFolder model)
        {
            var entity = new TaskFolderEntityV2(model.Id);
            entity.Name = model.Name;
            entity.TaskIds = model.TaskIds;

            return entity;
        }
    }
}
