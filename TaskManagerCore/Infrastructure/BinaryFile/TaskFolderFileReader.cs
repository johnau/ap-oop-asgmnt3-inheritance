using TaskManagerCore.Infrastructure.BinaryFile.Entity;

namespace TaskManagerCore.Infrastructure.BinaryFile
{
    internal class TaskFolderFileReader : BinaryFileReader<TaskFolderEntity>
    {
        public override bool HasNext(BinaryReader reader)
        {
            throw new NotImplementedException();
        }

        public override TaskFolderEntity ReadData(BinaryReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
