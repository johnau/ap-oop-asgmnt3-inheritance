using BinaryFileHandler;
using System.IO;
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
            writer.Write(entity.Id);
            writer.Write(entity.Description);
            writer.Write(entity.Notes);
            writer.Write(entity.Completed);
            writer.Write(entity.DueDate != null ? entity.DueDate.Value.Ticks : 0L);
            writer.Write(entity is RepeatingTaskDataEntity repeatingTask1 ? (int)repeatingTask1.RepeatingInterval : -1);
            writer.Write(entity is RepeatingTaskDataEntity repeatingTask2 ? repeatingTask2.Repetitions : -1);
            writer.Write(entity is HabitualTaskDataEntity habitualTask ? habitualTask.Streak : -1);
        }
    }
}
