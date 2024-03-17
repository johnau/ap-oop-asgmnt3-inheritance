using BinaryFileHandler;
using System.Diagnostics;
using TaskManagerCore.Infrastructure.BinaryFile.Entity;

namespace TaskManagerCore.Infrastructure.BinaryFile.FileHandlers
{
    internal class TaskDataFileWriter : BinaryFileWriter<TaskDataEntity>
    {
        public TaskDataFileWriter(BinaryFileConfig config) : base(config) { }

        /// <summary>
        /// Writes current TaskDataEntity using provided BinaryWriter instance
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="entity"></param>
        protected override void WriteObject(BinaryWriter writer, TaskDataEntity entity)
        {
            // Values of derived classes stored in byte array
            // Serves no real purpose now, could remove those bits
            //var interval = -1;
            //var reptitions = -1;
            //var streak = -1;

            //if (entity is RepeatingTaskDataEntity repeating)
            //{
            //    interval = (int)repeating.RepeatingInterval;
            //    reptitions = repeating.Repetitions;
            //}

            //if (entity is HabitualTaskDataEntity habitual)
            //{
            //    streak = habitual.Streak;
            //}

            //var bytes = ToByteArray([interval, reptitions, streak]);

            writer.Write(entity.Id);
            writer.Write(entity.Description);
            writer.Write(entity.Notes);
            writer.Write(entity.Completed);
            writer.Write(entity.DueDate != null ? entity.DueDate.Value.Ticks : 0L);
            //writer.Write(bytes);
            
            writer.Write(entity is RepeatingTaskDataEntity repeatingTask ? (int)repeatingTask.RepeatingInterval : -1);
            writer.Write(entity is RepeatingTaskDataEntity repeatingTask1 ? repeatingTask1.Repetitions : -1);
            writer.Write(entity is HabitualTaskDataEntity habitualTask ? habitualTask.Streak : -1);
            
            //Debug.WriteLine($"[BinaryFile]: id={entity.Id}, Desc={entity.Description}, Notes={entity.Notes}, Completed={entity.Completed}, DueDate={entity.DueDate}, XDataBytes={BitConverter.ToString(bytes).Replace("-", "")}");
            Debug.WriteLine($"[BinaryFile]: id={entity.Id}, Desc={entity.Description}, Notes={entity.Notes}, Completed={entity.Completed}, DueDate={entity.DueDate}");
        }

        //private byte[] ToByteArray(int[] ints)
        //{
        //    byte[] bytes = new byte[sizeof(int) * 3];
        //    Buffer.BlockCopy(ints, 0, bytes, 0, bytes.Length);
        //    return bytes;
        //}
    }
}
