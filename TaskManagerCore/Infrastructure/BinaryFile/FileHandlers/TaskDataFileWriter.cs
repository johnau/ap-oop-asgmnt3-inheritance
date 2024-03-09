using System;
using System.Diagnostics;
using TaskManagerCore.Infrastructure.BinaryFile.Entity;
using TaskManagerCore.Infrastructure.BinaryFile.FileHandlers.Helper;

namespace TaskManagerCore.Infrastructure.BinaryFile.FileHandlers
{
    /// <summary>
    /// Need to make an async versino of these classes...
    /// </summary>
    internal class TaskDataFileWriter : BinaryFileWriter<TaskDataEntity>
    {
        public TaskDataFileWriter(string filename = "task-data", string? rootPath = null)
            : base(filename, rootPath)
        { }

        protected override void WriteObject(BinaryWriter writer, TaskDataEntity entity)
        {
            var className = entity.GetType().Name;
            var id = entity.Id;
            var description = entity.Description;
            var notes = entity.Notes;
            var completed = entity.Completed;
            var dueDate = entity.DueDate;
            var interval = 0;
            var reptitions = 0;
            var streak = 0;
            byte[] bytes = new byte[sizeof(int) * 3]; // create byte array to store the 3 additional int values for derived classes

            // stack the values
            if (entity is RepeatingTaskDataEntity repeating)
            {
                dueDate = repeating.DueDate;
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
            writer.Write(id);
            writer.Write(description);
            writer.Write(notes);
            writer.Write(completed);
            writer.Write(dueDate != null ? dueDate.Value.Ticks : 0L);
            writer.Write(bytes);
            Debug.WriteLine($"Wrote Class:{className}, id={entity.Id}, Desc={entity.Description}, Notes={entity.Notes}, Completed={entity.Completed}, DueDate={entity.DueDate}, XDataBytes={bytes}");
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
