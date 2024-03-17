using BinaryFileHandler;
using System.Diagnostics;
using TaskManagerCore.Infrastructure.BinaryFile.Entity;

namespace TaskManagerCore.Infrastructure.BinaryFile.FileHandlers
{
    /// <summary>
    /// Concrete implementation for TaskDataEntity
    /// </summary>
    internal class TaskDataFileWriter : BinaryFileWriter<TaskDataEntity>
    {
        public TaskDataFileWriter(BinaryFileConfig config) : base(config) { }

        protected override void WriteObject(BinaryWriter writer, TaskDataEntity entity)
        {
            var interval = -1;
            var reptitions = -1;
            var streak = -1;

            // Additional values from Repeating type
            if (entity is RepeatingTaskDataEntity repeating)
            {
                interval = (int)repeating.RepeatingInterval;
                reptitions = repeating.Repetitions;
            }

            // Additional value from Habitual type
            if (entity is HabitualTaskDataEntity habitual)
            {
                streak = habitual.Streak;
            }

            var bytes = ToByteArray([interval, reptitions, streak]);

            writer.Write(entity.Id);
            writer.Write(entity.Description);
            writer.Write(entity.Notes);
            writer.Write(entity.Completed);
            writer.Write(entity.DueDate != null ? entity.DueDate.Value.Ticks : 0L);
            writer.Write(bytes);

            Debug.WriteLine($"[BinaryFile]: id={entity.Id}, Desc={entity.Description}, Notes={entity.Notes}, Completed={entity.Completed}, DueDate={entity.DueDate}, XDataBytes={BitConverter.ToString(bytes).Replace("-", "")}");
        }

        private byte[] ToByteArray(int[] ints)
        {
            byte[] bytes = new byte[sizeof(int) * 3];
            Buffer.BlockCopy(ints, 0, bytes, 0, bytes.Length);
            return bytes;
        }
    }
}
