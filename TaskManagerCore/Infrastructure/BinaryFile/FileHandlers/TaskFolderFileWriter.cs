using BinaryFileHandler;
using TaskManagerCore.Infrastructure.BinaryFile.Entity;

namespace TaskManagerCore.Infrastructure.BinaryFile.FileHandlers
{
    internal class TaskFolderFileWriter : BinaryFileWriter<TaskFolderEntity>
    {
        public TaskFolderFileWriter(BinaryFileConfig config) : base(config) { }

        /// <summary>
        /// Write current object
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="entity"></param>
        protected override void WriteObject(BinaryWriter writer, TaskFolderEntity entity)
        {
            writer.Write(entity.Id);
            writer.Write(entity.Name);
            writer.Write(string.Join(Delimiter, entity.TaskIds));
        }
    }
}
