namespace TaskManagerCore.Infrastructure.Memory.Entity
{
    public class TaskDataEntity : EntityBase
    {
        public string Description { get; set; }

        public string Notes { get; set; }

        public bool Completed { get; set; }

        public DateTime? DueDate { get; set; }

        public TaskDataEntity(string? id = "")
            : base(id)
        {
            Description = "";
            Notes = "";
            Completed = false;
        }

        //public TaskData ToModel()
        //{
        //    return new TaskData(Id, Description, Notes, Completed, DueDate);
        //}

        // this needs to be refactored so that we can override or have all types handled in one place.
        // either take out of here, somehow make instance method? Impossible...
        //public static TaskDataEntity FromModel(TaskData model) 
        //{
        //    return new TaskDataEntity(model.Id) { // generates a new ID here
        //        Description = model.Description, 
        //        Notes = model.Notes,
        //        Completed = model.Completed,
        //        DueDate = model.DueDate,
        //    };
        //}
    }
}
