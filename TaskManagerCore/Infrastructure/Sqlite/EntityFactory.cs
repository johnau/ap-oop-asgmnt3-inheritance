using TaskManagerCore.Infrastructure.Sqlite.Entity;
using TaskManagerCore.Model;

namespace TaskManagerCore.Infrastructure.Sqlite
{
    internal class EntityFactory
    {
        public static List<TaskData> ToModel(List<TaskDataEntity> entities)
        {
            var modelObjects = new List<TaskData>();

            foreach (var entity in entities)
            {
                modelObjects.Add(ToModel(entity));
            }

            return modelObjects;
        }

        public static TaskData ToModel(TaskDataEntity entity)
        {
            var model = new TaskData(entity.Id, entity.Description, entity.Notes, entity.Completed, entity.DueDate);

            if (entity.RepeatingInterval > 0)
            {
                // handle repeating task
            }

            if (entity.Streak >= 0)
            {
                // handle habitual task
            }

            return model;
        }

        public static TaskDataEntity FromModel(TaskData model)
        {
            var entity = new TaskDataEntity(model.Id);
            entity.Description = model.Description;
            entity.DueDate = model.DueDate;
            entity.Notes = model.Notes;
            entity.Completed = model.Completed;

            // TODO: Handle other task types

            return entity;
        }

        public static List<TaskFolder> ToModel(List<TaskFolderEntity> entities)
        {
            var modelObjects = new List<TaskFolder>();
            foreach (var entity in entities)
            {
                modelObjects.Add(ToModel(entity));
            }

            return modelObjects;
        }

        public static TaskFolder ToModel(TaskFolderEntity entity)
        {
            return new TaskFolder(entity.Id, entity.Name, entity.Tasks.Select(t => t.Id).ToList());
        }

        public static TaskFolderEntity FromModel(TaskFolder model)
        {
            var entity = new TaskFolderEntity(model.Id);
            entity.Name = model.Name;

            return entity;
        }
    }
}
