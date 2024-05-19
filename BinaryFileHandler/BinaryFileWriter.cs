using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BinaryFileHandler
{
    /// <summary>
    /// Need to make an async version of this class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BinaryFileWriter<T> : BinaryFileAccessor, IBinaryFileWriter<T>
    {
        protected List<T> WritePendingList;
        protected bool LastFailed = false;

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
        public bool WriteValues()
        {
            // Don't overwrite if the last write failed to protect the backup copy
            BackupExistingFile(overwrite: !LastFailed);

            // Copy pending items and clear the pending list
            var toWrite = CopyAndClearPending();

            // Tries with repeats if file is in use
            return TryWriteFile(toWrite);
        }

        /// <summary>
        /// Async wrapper using Semaphore to prevent concurrent access
        /// </summary>
        /// <returns></returns>
        public Task WriteValuesAsync()
        {
            return Task.Run(async () =>
            {
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
        /// Attempts to write file with multiple retry attempts on certain failures
        /// </summary>
        /// <param name="toWrite"></param>
        /// <param name="retriesRemaining"></param>
        /// <returns></returns>
        bool TryWriteFile(List<T> toWrite, int retriesRemaining = 20)
        {
            try
            {
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

                    WriteTerminatorString(writer);
                }

                Debug.WriteLine($"Wrote {WritePendingList.Count} items");
                LastFailed = false;
                return true;
            }
            catch (IOException ex) when (FileIsInUse(ex))
            {
                Debug.WriteLine($"File in use: {ex.Message}, retrying {retriesRemaining} more times");
                LastFailed = true;

                // if file is in use we will retry
                if (retriesRemaining > 1)
                {
                    Thread.Sleep(10);
                    TryWriteFile(toWrite, retriesRemaining--);
                }
                return false;
            }
            catch (UnauthorizedAccessException ex)
            {
                Debug.WriteLine($"Access denied: {ex.Message}");
                // Change path to temp folder and try again without counting as a retry
                _rootPath = Path.Combine(Path.GetTempPath(), "taskmanager_data_" + Guid.NewGuid());

                LastFailed = true;
                
                TryWriteFile(toWrite, retriesRemaining);

                return false;
            }
            catch (Exception ex)
            {
                LastFailed = true;
                return false;
            }
            finally
            {
                WritePendingList.Clear(); // Ensure cleared
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected void WriteTerminatorString(BinaryWriter writer)
        {
            writer.Write(FileTerminator);
        }

        protected void BackupExistingFile(bool overwrite)
        {
            try
            {
                File.Copy(FilePath, FilePath + BackupExtension, overwrite);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to create file backup: {ex.Message}");
            }
        }

        List<T> CopyAndClearPending()
        {
            var copy = new List<T>(WritePendingList);
            WritePendingList.Clear();
            return copy;
        }
    }
}
