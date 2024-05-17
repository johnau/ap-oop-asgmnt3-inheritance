using Microsoft.VisualBasic;
using TaskManagerCore.Infrastructure.Sqlite.Entity;
using TaskManagerCore.Model;

namespace TaskManagerCore.Infrastructure.Sqlite
{
    internal class EntityFactory
    {
        public static TaskData WithId(TaskData task, string id)
        {
            return new TaskData(id, task.Description, task.Notes, task.Completed, task.DueDate);
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
                model = new RepeatingTaskData(entity.GlobalId, entity.Description, entity.Notes, entity.Completed, entity.DueDate!.Value, (TimeInterval)entity.RepeatingInterval, (int)entity.Repetitions);
            }

            if (entity.Streak >= 0)
            {
                model = new HabitualTaskData(entity.GlobalId, entity.Description, entity.Notes, entity.Completed, entity.DueDate!.Value, (TimeInterval)entity.RepeatingInterval, (int)entity.Repetitions, (int)entity.Streak);
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
            return new TaskFolder(entity.GlobalId, entity.Name, entity.Tasks.Select(t => t.GlobalId).ToList());
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
