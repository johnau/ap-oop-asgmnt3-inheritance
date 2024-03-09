namespace TaskManagerCore.Infrastructure.BinaryFile.Entity
{
    internal class HabitualTaskDataEntity : RepeatingTaskDataEntity
    {
        public int Streak { get; set; }

        public HabitualTaskDataEntity(string? id = "")
            : base(id)
        {
            Streak = 0;
        }
    }
}
