namespace TaskManagerCore.Infrastructure.BinaryFile.Entity
{
    internal class TaskDataEntity : EntityBase
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

    }
}
