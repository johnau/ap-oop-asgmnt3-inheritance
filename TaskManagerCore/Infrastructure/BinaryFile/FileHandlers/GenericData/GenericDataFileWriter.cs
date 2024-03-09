
namespace TaskManagerCore.Infrastructure.BinaryFile.FileHandlers.GenericData
{
    /// <summary>
    /// TODO: Finish
    /// </summary>
    internal class GenericDataFileWriter : GenericBinaryFileAccessor
    {
        protected GenericDataFileWriter(Dictionary<string, Entry> dataStructure, string filename = "data", string? rootPath = null)
            : base(dataStructure, filename, rootPath)
        {
            throw new NotImplementedException();
        }
    }
}
