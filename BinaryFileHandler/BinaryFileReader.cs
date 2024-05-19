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
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BinaryFileReader<T> : BinaryFileAccessor, IBinaryFileReader<T>
    {
        protected readonly List<T> ReadList;
        protected string CurrentClassName;

        protected BinaryFileReader(BinaryFileConfig config)
            : base(config)
        {
            ReadList = new List<T>();
            CurrentClassName = "";
        }

        /// <summary>
        /// Method to read a single entry
        /// </summary>
        /// <param name="reader"></param>
        /// <returns>True if reached the end of the file</returns>
        protected abstract T ReadObject(BinaryReader reader);

        /// <summary>
        /// Get a copy of last read data
        /// </summary>
        public List<T> Data => new List<T>(ReadList);

        /// <summary>
        /// Reads values from file using the HasNext() and ReadNext() methods provided by the concrete class
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<T> ReadValues()
        {
            ReadList.Clear();

            if (!FileExists(FilePath))
                throw new Exception($"An expected file was not found: {FilePath}");

            // Retry if file is in use, abort on other errors
            var retryAttempts = 20;
            List<T> fileData;
            while (!TryReadFile(out fileData, out var abort) && retryAttempts > 0)
            {
                if (abort) break;
                Thread.Sleep(30);
                retryAttempts--;
            }

            // Store the list so it can be accessed with `Data` property
            StashFileData(fileData);
            return fileData;
        }

        /// <summary>
        /// Async wrapper to read values
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Method to check if the current entry is the Terminator object
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected virtual bool HasNext(BinaryReader reader)
        {
            var classNameOrTerminator = reader.ReadString();
            Debug.WriteLine($"Next: {classNameOrTerminator}");

            if (IsTerminator(classNameOrTerminator))
            {
                CurrentClassName = string.Empty;
                return false;
            }

            // Save the class name for use in the ReadNext method
            CurrentClassName = classNameOrTerminator;
            return true;
        }

        /// <summary>
        /// Try Read file data, returns true on success, and returns a list of objects
        /// If failed, the abort bool is true if file access was denied for reasons other than
        /// file in use (ie. does not exist, or permissions)
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="data"></param>
        /// <param name="abort"></param>
        /// <returns></returns>
        bool TryReadFile(out List<T> data, out bool abort)
        {
            data = new List<T>();
            abort = false;
            try
            {
                using (var stream = new FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None, bufferSize: 4096, useAsync: true))
                using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    while (HasNext(reader))
                    {
                        var entity = ReadObject(reader);
                        data.Add(entity);
                    }
                }

                return true;
            }
            catch (IOException ex) when (FileIsInUse(ex)) // if file is in use we will retry
            {
                Debug.WriteLine($"The file is in use: {FilePath}");

                abort = false;
                return false;
            }
            catch (Exception ex) // catch all other exceptions and fail
            {
                Debug.WriteLine($"An exception was thrown: {ex.Message} whil trying to read the file {FilePath}");

                abort = true;
                return false;
            }
        }

        void StashFileData(List<T> fileData)
        {
            ReadList.Clear();
            ReadList.AddRange(fileData);
        }
    }
}
