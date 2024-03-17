using BinaryFileHandler;
using TaskManagerCore.Infrastructure.BinaryFile.Entity;

namespace TaskManagerCore.Infrastructure.BinaryFile.FileHandlers
{
    internal class TaskFolderFileReader : BinaryFileReader<TaskFolderEntity>
    {
        public TaskFolderFileReader(BinaryFileConfig config)
            : base(config)
        { }

        /// <summary>
        /// Read current object
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        protected override TaskFolderEntity ReadNext(BinaryReader reader)
        {
            var id = reader.ReadString();
            var name = reader.ReadString();
            var taskIds = reader.ReadString().Split(Delimiter, StringSplitOptions.RemoveEmptyEntries);

            return new TaskFolderEntity(id)
            {
                Name = name,
                TaskIds = taskIds.ToList(),
            };
        }
    }
}
