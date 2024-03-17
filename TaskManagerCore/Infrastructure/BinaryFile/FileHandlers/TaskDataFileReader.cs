using BinaryFileHandler;
using System.IO;
using TaskManagerCore.Infrastructure.BinaryFile.Entity;
using TaskManagerCore.Model;

namespace TaskManagerCore.Infrastructure.BinaryFile.FileHandlers
{
    internal class TaskDataFileReader : BinaryFileReader<TaskDataEntity>
    {
        private readonly List<string> acceptedClasses = new List<string>() {typeof(TaskDataEntity).Name,
                                                                            typeof(RepeatingTaskDataEntity).Name,
                                                                            typeof(HabitualTaskDataEntity).Name};

        public TaskDataFileReader(BinaryFileConfig config) : base(config) { }

        /// <summary>
        /// Reads current TaskDataEntity with provided BinaryReader instance
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        protected override TaskDataEntity ReadObject(BinaryReader reader)
        {
            if (!acceptedClasses.Contains(CurrentClassName))
                throw new ArgumentException("A recognized Class name was not detected");

            var id = reader.ReadString();
            var description = reader.ReadString();
            var notes = reader.ReadString();
            var completed = reader.ReadBoolean();
            var dueDate = reader.ReadInt64();
            var interval = reader.ReadInt32();
            var repetitions = reader.ReadInt32();
            var streak = reader.ReadInt32();

            return EntityFactory.TaskFromValues(CurrentClassName, 
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
