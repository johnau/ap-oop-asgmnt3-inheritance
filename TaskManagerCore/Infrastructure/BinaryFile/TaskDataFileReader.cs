using System.IO;
using System.Reflection.PortableExecutable;
using TaskManagerCore.Infrastructure.BinaryFile.Entity;
using TaskManagerCore.Infrastructure.BinaryFile.Helper;
using TaskManagerCore.Model;

namespace TaskManagerCore.Infrastructure.BinaryFile
{
    internal struct DataStruct
    {
        public DataStruct() { }
        public string Id { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public bool Completed { get; set; } = false;
        public long DueDate { get; set; } = 0L;
        public int Interval { get; set; } = 0;
        public int Repetitions { get; set; } = 0;
        public int Streak { get; set; } = 0;
    }

    internal class TaskDataFileReader : BinaryFileReader<TaskDataEntity>
    {
        readonly string TaskDataClassName = typeof(TaskDataEntity).Name;
        readonly string RepeatingTaskDataClassName = typeof(RepeatingTaskDataEntity).Name;
        readonly string HabitualTaskDataClassName = typeof(HabitualTaskDataEntity).Name;

        DataStruct? currentData = null;

        public TaskDataFileReader(string filename = "task-data", string? rootPath = null)
            : base(filename, rootPath)
        { }

        public override bool HasNext(BinaryReader reader)
        {
            var data = new DataStruct();
            data.ClassName = reader.ReadString();
            data.Id = reader.ReadString();
            data.Description = reader.ReadString();
            data.Notes = reader.ReadString();
            data.Completed = reader.ReadBoolean();
            data.DueDate = reader.ReadInt64();
            
            byte[] xDataBytes = reader.ReadBytes(sizeof(int) * 3);

            using (MemoryStream stream = new MemoryStream(xDataBytes))
            using (BinaryReader byteReader = new BinaryReader(stream))
            {
                data.Interval = byteReader.ReadInt32();
                data.Repetitions = byteReader.ReadInt32();
                data.Streak = byteReader.ReadInt32();
            }

            if (IsTerminator(data))
            {
                currentData = null;
                return false;
            }

            currentData = data;
            return true;
        }

        public override TaskDataEntity ReadData(BinaryReader reader)
        {
            if (currentData == null) 
                throw new Exception("Call HasNext() first");

            if (currentData.Value.ClassName.Equals(TaskDataClassName, StringComparison.Ordinal))
            {
                return new TaskDataEntity(currentData.Value.Id)
                {
                    Description = currentData.Value.Description,
                    Notes = currentData.Value.Notes,
                    Completed = currentData.Value.Completed,
                    DueDate = currentData.Value.DueDate > 0 ? new DateTime(currentData.Value.DueDate) : null,
                };
            } 
            else if (currentData.Value.ClassName.Equals(RepeatingTaskDataClassName, StringComparison.Ordinal))
            {
                return new RepeatingTaskDataEntity(currentData.Value.Id)
                {
                    Description = currentData.Value.Description,
                    Notes = currentData.Value.Notes,
                    Completed = currentData.Value.Completed,
                    DueDate = new DateTime(currentData.Value.DueDate),
                    RepeatingInterval = (TimeInterval)currentData.Value.Interval,
                    Repititions = currentData.Value.Repetitions,
                };
            } 
            else if (currentData.Value.ClassName.Equals(HabitualTaskDataClassName, StringComparison.Ordinal))
            {
                return new HabitualTaskDataEntity(currentData.Value.Id)
                {
                    Description = currentData.Value.Description,
                    Notes = currentData.Value.Notes,
                    Completed = currentData.Value.Completed,
                    DueDate = new DateTime(currentData.Value.DueDate),
                    RepeatingInterval = (TimeInterval)currentData.Value.Interval,
                    Repititions = currentData.Value.Repetitions,
                    Streak = currentData.Value.Streak
                };
            }

            throw new Exception("Unhandled class type");
        }

        static bool IsTerminator(DataStruct data)
        {
            if (data.Id.Equals(TaskDataTerminator.IdTerminator) 
                && data.ClassName.Equals(TaskDataTerminator.ClassNameTerminator)
                && data.Description.Equals(TaskDataTerminator.DescriptionTerminator)
                && data.Notes.Equals(TaskDataTerminator.NotesTerminator)
                && data.Completed == TaskDataTerminator.CompletedTerminator
                )
            {
                return true;
            }

            return false;
        }

    }

}
