using System.Diagnostics;
using System.Text;

namespace TaskManagerCore.Infrastructure.BinaryFile
{
    /// <summary>
    /// Need to make an async version of this class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal abstract class BinaryFileWriter<T>
    {
        protected const string _extension = ".bin";
        protected readonly string _filenameBase;
        protected readonly string _rootPath;

        protected List<T> WritePendingList;

        public BinaryFileWriter(string filename = "data", string? rootPath = null)
        {
            if (rootPath == null) _rootPath = Path.GetTempPath();
            else _rootPath = rootPath;

            _filenameBase = filename;
            WritePendingList = new List<T>(); 
        }

        string GenerateFilePath()
        {
            // handle using a new file after current file exceeds certain size
            var filename = _filenameBase + _extension;
            return Path.Combine(_rootPath, filename);
        }

        public void AddObjectsToWrite(List<T> items)
        {
            WritePendingList.AddRange(items);
        }

        public void AddObjectToWrite(T item)
        {
            WritePendingList.Add(item);
        }

        protected abstract void WriteObject(BinaryWriter writer, T item);

        protected abstract void WriteTerminatorObject(BinaryWriter writer);

        /// <summary>
        /// Writes all values added to the writer to a binary file
        /// </summary>
        public string WriteValues()
        {
            var filePath = GenerateFilePath();
            Debug.WriteLine($"Filepath={filePath}");
            using (var stream = File.Open(filePath, FileMode.OpenOrCreate))
            //using (var stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
            {
                foreach (var item in WritePendingList)
                {
                    WriteObject(writer, item);
                }

                WriteTerminatorObject(writer);
            }

            Debug.WriteLine($"Wrote {WritePendingList.Count} items");
            WritePendingList.Clear();

            return filePath;
        }
    }
}
