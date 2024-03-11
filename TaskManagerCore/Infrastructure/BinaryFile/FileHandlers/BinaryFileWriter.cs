using System.Diagnostics;
using System.Text;

namespace TaskManagerCore.Infrastructure.BinaryFile.FileHandlers
{
    /// <summary>
    /// Need to make an async version of this class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal abstract class BinaryFileWriter<T> : BinaryFileAccessor
    {
        protected List<T> WritePendingList;

        public BinaryFileWriter(string filename = "data", string? rootPath = null)
            : base(filename, rootPath)
        {
            WritePendingList = new List<T>();
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
            //Debug.WriteLine($"Filepath={filePath}");

            using (var stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
            //using (var stream = File.Open(filePath, FileMode.OpenOrCreate))
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

        public async Task<string> WriteValuesAsync()
        {
            await accessSemaphore.WaitAsync();
            try
            {
                return WriteValues();
            }
            finally
            {
                accessSemaphore.Release();
            }
        }
    }
}
