using System.Diagnostics;
using System.Text;

namespace BinaryFileHandler
{
    /// <summary>
    /// Need to make an async version of this class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BinaryFileWriter<T> : BinaryFileAccessor
    {
        protected List<T> WritePendingList;

        public BinaryFileWriter(BinaryFileConfig config)
            : base(config)
        {
            WritePendingList = new List<T>();
        }

        /// <summary>
        /// Abstract method for Concrete implementation of type T for specific write implementation per type
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="item"></param>
        protected abstract void WriteObject(BinaryWriter writer, T item);

        /// <summary>
        /// Populate the Write list
        /// </summary>
        /// <param name="items"></param>
        public void AddObjectsToWrite(List<T> items) => WritePendingList.AddRange(items);
        
        /// <summary>
        /// Populate the Write list
        /// </summary>
        /// <param name="item"></param>
        public void AddObjectToWrite(T item) => WritePendingList.Add(item);

        /// <summary>
        /// Writes all values added to the writer to a binary file
        /// </summary>
        public bool WriteValues(int retriesRemaining = 20)
        {
            // Copy the list and clear it on Write call
            var toWrite = new List<T>(WritePendingList);
            WritePendingList.Clear();

            // Catch any I/O errors
            try
            {
                //using (var stream = File.Open(filePath, FileMode.OpenOrCreate))
                using (var stream = new FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
                using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                {
                    foreach (var item in toWrite)
                    {
                        if (item == null) continue;
                        
                        // Write class name on first row (first row is either ClassName or Terminator)
                        writer.Write(item.GetType().Name);

                        Debug.WriteLine($"Writing object of type: {item.GetType().Name}");
                        WriteObject(writer, item);
                    }

                    WriteTerminatorObject(writer);
                }

                Debug.WriteLine($"Wrote {WritePendingList.Count} items");
                return true;
            }
            catch (Exception ex) 
            {
                Debug.WriteLine($"Could not write data: {ex.Message}, retrying {retriesRemaining} more times");
                
                // Try again if there are retries remaining.
                if (retriesRemaining > 1)
                {
                    Thread.Sleep(50);
                    WriteValues(retriesRemaining--);
                }
                return false;
            }
            finally
            {
                // Make sure list is cleared after write
                WritePendingList.Clear();
            }
        }

        /// <summary>
        /// Async wrapper using Semaphore to prevent concurrent access
        /// </summary>
        /// <returns></returns>
        public Task WriteValuesAsync()
        {
            return Task.Run(async () => { 
                await accessSemaphore.WaitAsync();
                try
                {
                    WriteValues();
                }
                finally
                {
                    accessSemaphore.Release();
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected static void WriteTerminatorObject(BinaryWriter writer)
        {
            writer.Write(GenericTerminators.StringTerminator);
        }
    }
}
