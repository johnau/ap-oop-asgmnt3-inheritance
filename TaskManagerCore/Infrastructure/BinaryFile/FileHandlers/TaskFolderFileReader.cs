using TaskManagerCore.Infrastructure.BinaryFile.Entity;
using TaskManagerCore.Infrastructure.BinaryFile.FileHandlers.Helper;

namespace TaskManagerCore.Infrastructure.BinaryFile.FileHandlers
{
    internal class TaskFolderFileReader : BinaryFileReader<TaskFolderEntity>
    {
        // TODO: switch this out for the task folder entity
        internal struct DataStruct
        {
            public DataStruct() { }
            public string Id { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string[] TaskIds { get; set; } = new string[0];
        }

        DataStruct? currentData = null;

        public TaskFolderFileReader(string filename = "folder-data", string? rootPath = null)
            : base(filename, rootPath)
        { }

        protected override bool HasNext(BinaryReader reader)
        {
            var data = new DataStruct();
            data.Id = reader.ReadString();
            data.Name = reader.ReadString();
            data.TaskIds = reader.ReadString().Split(Delimiter, StringSplitOptions.RemoveEmptyEntries);

            if (IsTerminator(data))
            {
                currentData = null;
                return false;
            }

            currentData = data;
            return true;
        }

        protected override TaskFolderEntity ReadData(BinaryReader reader)
        {
            if (currentData == null)
                throw new Exception("Call HasNext() first");

            return new TaskFolderEntity(currentData.Value.Id)
            {
                Name = currentData.Value.Name,
                TaskIds = currentData.Value.TaskIds.ToList(),
            };
        }

        static bool IsTerminator(DataStruct data)
        {
            if (data.Id.Equals(TaskFolderTerminator.IdTerminator)
                && data.Name.Equals(TaskFolderTerminator.NameTerminator)
                && data.TaskIds.Length > 0 && data.TaskIds[0] == TaskFolderTerminator.TaskIdsTerminator[0])
            {
                return true;
            }

            return false;
        }
    }
}
