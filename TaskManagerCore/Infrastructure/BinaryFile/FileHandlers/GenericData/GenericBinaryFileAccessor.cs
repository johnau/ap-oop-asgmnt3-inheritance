using TaskManagerCore.Infrastructure.BinaryFile.FileHandlers;

namespace TaskManagerCore.Infrastructure.BinaryFile.FileHandlers.GenericData
{
    /// <summary>
    /// TODO: Finish
    /// Need to setup a builder pattern taht constructs the data structure
    /// Then that can be used to guide the reading and writing
    /// These classes are not currently used as the logic has not been fully implemented
    /// </summary>
    internal class GenericBinaryFileAccessor : BinaryFileAccessor
    {
        internal struct Entry
        {
            public Entry(Type type, object obj)
            {
                Type = type;
                Obj = obj;
            }

            public Type Type { get; set; }
            public object Obj { get; set; }
        }

        protected Dictionary<string, Entry> DataStructure;

        protected GenericBinaryFileAccessor(Dictionary<string, Entry> dataStructure, string filename = "data", string? rootPath = null)
            : base(filename, rootPath)
        {
            DataStructure = dataStructure;
            throw new NotImplementedException();
        }

    }
}
