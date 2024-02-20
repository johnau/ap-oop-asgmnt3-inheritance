namespace TaskManagerCore.Infrastructure.Memory.Entity
{
    public class HabitualTaskDataEntity : RepeatingTaskDataEntity
    {
        public int Streak { get; set; }

        public HabitualTaskDataEntity(string? id = "") 
            : base(id)
        {
        }
    }
}
