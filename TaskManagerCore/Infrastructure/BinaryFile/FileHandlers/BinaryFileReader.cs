
using System.Diagnostics;
using System.Text;

namespace TaskManagerCore.Infrastructure.BinaryFile.FileHandlers
{
    /// <summary>
    /// Is it better to keep the files open over the session of the app, and share with FileStream?
    /// Or is it better to just handle the conflicts...
    /// What happens when the app crashes with the open filestream?
    /// Can we corrupt the file or something? Does that make it better to keep opening and closing the file.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal abstract class BinaryFileReader<T> : BinaryFileAccessor
    {
        protected List<T> ReadList;
        public List<T> Data => new List<T>(ReadList);

        protected BinaryFileReader(string filename = "data", string? rootPath = null)
            : base(filename, rootPath)
        {
            ReadList = new List<T>();
        }

        /// <summary>
        /// Should return false when the Terminator entity is reached.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected abstract bool HasNext(BinaryReader reader);

        /// <summary>
        /// Can split this out into a HasNext() and Read() pair of methods
        /// </summary>
        /// <param name="reader"></param>
        /// <returns>True if reached the end of the file</returns>
        protected abstract T ReadData(BinaryReader reader);

        /// <summary>
        /// Reads values from file using the HasNext() and ReadData() methods provided by the concrete class
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<T> ReadValues()
        {
            var filePath = GenerateFilePath();
            if (!FileExists(filePath))
                throw new Exception($"An expected file was not found: {filePath}");

            // Retry a few times incase the file is still being written.  100 tries at 100ms = 10000ms = 10s, already too long...
            var retryAttempts = 100;
            while (retryAttempts > 0)
            {
                try
                {
                    //using (var stream = File.Open(filePath, FileMode.Open))
                    //using (var stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, bufferSize: 4096, useAsync: true))
                    using (var stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None, bufferSize: 4096, useAsync: true))
                    using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                    {
                        while (HasNext(reader))
                        {
                            var entity = ReadData(reader);
                            ReadList.Add(entity);
                        }
                    }
                    retryAttempts = 0;
                    return new List<T>(ReadList);
                }
                catch (IOException ex) when (IsFileInUse(ex)) // if file is in use we will retry
                {
                    retryAttempts--;
                    Debug.WriteLine("File is in use, waiting 100ms and retrying...");
                    Thread.Sleep(100);
                }
                catch (Exception ex) // catch all other exceptions and fail
                {
                    Debug.WriteLine($"An exception was thrown: {ex.Message} whil trying to read the file {filePath}");
                    throw;
                }
            }

            throw new Exception($"Unable to read from file: {filePath}");
        }

        public async Task<List<T>> ReadValuesAsync()
        {
            Debug.WriteLine($"[{DateTime.Now.Ticks}] Wait for semaphore...");
            await accessSemaphore.WaitAsync(5_000);
            Debug.WriteLine($"[{DateTime.Now.Ticks}] Got semaphore...");
            try
            {
                return ReadValues();
            }
            finally
            {
                accessSemaphore.Release();
            }
        }


    }
}
