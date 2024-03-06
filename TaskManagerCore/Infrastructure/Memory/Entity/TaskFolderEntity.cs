namespace TaskManagerCore.Infrastructure.Memory.Entity
{
    public class TaskFolderEntity : EntityBase
    {
        public string Name { get; set; }
        public List<string> TaskIds { get; set; } // Uni-directional one-to-many relationship

        public TaskFolderEntity(string? id = "")
            : base(id)
        {
            Name = "";
            TaskIds = new List<string>();
        }

        //public TaskFolder ToModel()
        //{
        //    return new TaskFolder(Id, Name, TaskIds);
        //}

        //public static TaskFolderEntity FromModel(TaskFolder model)
        //{
        //    return new TaskFolderEntity(model.Id)
        //    {
        //        Name = model.Name,
        //        TaskIds = model.TaskIds,
        //    };
        //}
    }
}
