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

        public override int CompareTo(TaskDataEntity? other)
        {
            var baseResult = base.CompareTo(other);

            if (baseResult == 0 && other is HabitualTaskDataEntity otherHabitualTask)
            {
                return Streak.CompareTo(otherHabitualTask.Streak);
            }

            return baseResult;
        }
    }
}
