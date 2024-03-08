
using TaskManagerCore.Infrastructure.BinaryFile.Entity;
using TaskManagerCore.Infrastructure.BinaryFile.Helper;

namespace TaskManagerCore.Infrastructure.BinaryFile
{
    internal class TaskFolderFileWriter : BinaryFileWriter<TaskFolderEntity>
    {
        private const string Delimiter = ";;";

        protected override void WriteObject(BinaryWriter writer, TaskFolderEntity entity)
        {
            writer.Write(entity.Name);
            var concatString = string.Join(Delimiter, entity.TaskIds);
            writer.Write(concatString);
        }

        protected override void WriteTerminatorObject(BinaryWriter writer)
        {
            writer.Write(TaskFolderTerminator.NameTerminator);
            var concatString = string.Join(Delimiter, TaskFolderTerminator.StringListTerminator);
            writer.Write(concatString);
        }
    }
}
