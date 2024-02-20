using TaskManagerCore.Model;

namespace TaskManagerCore.Infrastructure.Memory.Entity
{
    public class RepeatingTaskDataEntity : TaskDataEntity
    {
        public new DateTime DueDate { get; set; }
        public TimeInterval RepeatingInterval { get; set; }
        public int Repititions { get; set; }

        //public DateTime StartFrom { get; set; }  // retain the start date?
        public RepeatingTaskDataEntity(string? id = "") 
            : base(id)
        { 
        }
    }
}
