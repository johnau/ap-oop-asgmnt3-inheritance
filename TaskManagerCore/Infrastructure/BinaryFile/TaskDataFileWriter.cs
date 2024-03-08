using System;
using TaskManagerCore.Infrastructure.BinaryFile.Entity;
using TaskManagerCore.Infrastructure.BinaryFile.Helper;

namespace TaskManagerCore.Infrastructure.BinaryFile
{
    /// <summary>
    /// Need to make an async versino of these classes...
    /// </summary>
    internal class TaskDataFileWriter : BinaryFileWriter<TaskDataEntity>
    {
        public TaskDataFileWriter(string filename = "task-data", string? rootPath = null) 
            : base(filename, rootPath)
        {
        }

        protected override void WriteObject(BinaryWriter writer, TaskDataEntity entity)
        {
            var className = entity.GetType().Name;
            var interval = 0;
            var reptitions = 0;
            var streak = 0;
            byte[] bytes = new byte[sizeof(int) * 3]; // create byte array to store the 3 additional int values for derived classes

            if (entity is RepeatingTaskDataEntity repeating)
            {
                interval = (int)repeating.RepeatingInterval;
                reptitions = repeating.Repititions;
                className = repeating.GetType().Name;
            }

            if (entity is HabitualTaskDataEntity habitual)
            {
                streak = habitual.Streak;
                className = habitual.GetType().Name;
            }

            // write the values as a byte array
            var ints = new int[] { interval, reptitions, streak };
            Buffer.BlockCopy(ints, 0, bytes, 0, bytes.Length);

            writer.Write(className);
            writer.Write(entity.Id);
            writer.Write(entity.Description);
            writer.Write(entity.Notes);
            writer.Write(entity.Completed);
            if (entity.DueDate != null)
            {
                writer.Write(entity.DueDate.Value.Ticks);
            } else
            {
                writer.Write(0L);
            }
            writer.Write(bytes);
        }

        protected override void WriteTerminatorObject(BinaryWriter writer)
        {
            writer.Write(TaskDataTerminator.ClassNameTerminator);
            writer.Write(TaskDataTerminator.IdTerminator);
            writer.Write(TaskDataTerminator.DescriptionTerminator);
            writer.Write(TaskDataTerminator.NotesTerminator);
            writer.Write(TaskDataTerminator.CompletedTerminator);
            writer.Write(TaskDataTerminator.DueDateTerminator);
            writer.Write(TaskDataTerminator.XDataTerminator);
        }
    }
}
