using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagerCore.Infrastructure.BinaryFile.FileHandlers.GenericData
{
    internal class GenericDataFileWriter : GenericBinaryFileAccessor
    {
        protected GenericDataFileWriter(Dictionary<string, Entry> dataStructure, string filename = "data", string? rootPath = null)
            : base(dataStructure, filename, rootPath)
        {
            throw new NotImplementedException();
        }
    }
}
