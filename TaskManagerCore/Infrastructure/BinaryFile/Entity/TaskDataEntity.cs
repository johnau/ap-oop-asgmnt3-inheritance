using BinaryFileHandler;
using TaskManagerCore.Model;

namespace TaskManagerCore.Infrastructure.BinaryFile.Entity
{
    internal class TaskDataEntity : EntityBase, IBinaryWritableReadable<TaskDataEntity>
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

        /*
         * The BinaryFileHandler dll can be used by extending the abstract base classes
         * (See TaskManagerCore.Infrastructure.BinaryFile.FileHandlers) and that is what
         * is used in the application.
         * 
         * The below is an example of the second method of use, implementing
         * the IBinaryWritableReadable<T> interface, which can contain the write and read
         * logic to the Data object itself.
         * 
         * * These methods are never called and are not covered in test cases *
         */
        public void WriteObject(BinaryWriter writer, TaskDataEntity entity)
        {
            writer.Write(entity.Id);
            writer.Write(entity.Description);
            writer.Write(entity.Notes);
            writer.Write(entity.Completed);
            writer.Write(entity.DueDate != null ? entity.DueDate.Value.Ticks : 0L);
            writer.Write(entity is RepeatingTaskDataEntity repeatingTask1 ? (int)repeatingTask1.RepeatingInterval : -1);
            writer.Write(entity is RepeatingTaskDataEntity repeatingTask2 ? repeatingTask2.Repetitions : -1);
            writer.Write(entity is HabitualTaskDataEntity habitualTask ? habitualTask.Streak : -1);
        }

        public TaskDataEntity PopulateThis(BinaryReader reader, string className)
        {
            var id = reader.ReadString();
            var description = reader.ReadString();
            var notes = reader.ReadString();
            var completed = reader.ReadBoolean();
            var dueDate = reader.ReadInt64();
            var interval = reader.ReadInt32();
            var repetitions = reader.ReadInt32();
            var streak = reader.ReadInt32();

            return EntityFactory.TaskFromValues(className,
                                                id,
                                                description,
                                                notes,
                                                completed,
                                                dueDate > 0L ? new DateTime(dueDate) : null,
                                                (TimeInterval)interval,
                                                repetitions,
                                                streak);
        }
    }
}
