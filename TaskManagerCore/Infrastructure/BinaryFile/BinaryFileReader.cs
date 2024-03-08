
using System.Text;

namespace TaskManagerCore.Infrastructure.BinaryFile
{
    internal abstract class BinaryFileReader<T>
    {
        protected const string _extension = ".bin";
        protected readonly string _filenameBase;
        protected readonly string _rootPath;

        protected List<T> ReadList;
        public List<T> Data => new List<T>(ReadList);

        protected BinaryFileReader(string filename = "data", string? rootPath = null)
        {
            if (rootPath == null) _rootPath = Path.GetTempPath();
            else _rootPath = rootPath;

            _filenameBase = filename;
            
            ReadList = new List<T>();
        }

        // this is common and can be abstracted with the Writer class
        string GenerateFilePath()
        {
            // handle using a new file after current file exceeds certain size
            var filename = _filenameBase + _extension;
            return Path.Combine(_rootPath, filename);
        }

        public List<T> ReadDataFile()
        {
            var path = GenerateFilePath();
            if (!File.Exists(path))
                throw new Exception($"An expected file was not found: {path}");

            using (var stream = File.Open(path, FileMode.Open))
            using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
            {
                while (HasNext(reader))
                {
                    var entity = ReadData(reader);
                    ReadList.Add(entity);
                }
            }

            return new List<T>(ReadList);
        }

        /// <summary>
        /// Should return false when the Terminator entity is reached.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public abstract bool HasNext(BinaryReader reader);

        /// <summary>
        /// Can split this out into a HasNext() and Read() pair of methods
        /// </summary>
        /// <param name="reader"></param>
        /// <returns>True if reached the end of the file</returns>
        public abstract T ReadData(BinaryReader reader);

        
    }
}
