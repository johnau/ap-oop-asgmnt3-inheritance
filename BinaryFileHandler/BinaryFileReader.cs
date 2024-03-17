using System.Diagnostics;
using System.Text;

namespace BinaryFileHandler
{
    /// <summary>
    /// TODO: Make backup of file before writing incase write is interrupted for some reason
    /// Test read file after write (and during application load and other rquired times), and 
    /// restore backup if required
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BinaryFileReader<T> : BinaryFileAccessor
    {
        protected List<T> ReadList;
        protected string CurrentClassName;

        protected BinaryFileReader(BinaryFileConfig config)
            : base(config)
        {
            ReadList = new List<T>();
            CurrentClassName = "";
        }

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
            if (!FileExists(FilePath))
                throw new Exception($"An expected file was not found: {FilePath}");

            // Retry if file is in use, abort on other errors
            var retryAttempts = 100;
            List<T> fileData;
            while (!TryReadFile(out fileData, out var abort) && retryAttempts > 0)
            {
                if (abort) break;
                Thread.Sleep(30);
                retryAttempts--;
            }

            if (fileData == null)
                throw new Exception($"Unable to read from file: {FilePath}");

            return fileData;
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
        /// Method to read a single entry
        /// </summary>
        /// <param name="reader"></param>
        /// <returns>True if reached the end of the file</returns>
        protected abstract T ReadObject(BinaryReader reader);

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
                //using (var stream = File.Open(filePath, FileMode.Open))
                //using (var stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, bufferSize: 4096, useAsync: true))
                using (var stream = new FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None, bufferSize: 4096, useAsync: true))
                using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    while (HasNext(reader))
                    {
                        var entity = ReadObject(reader);
                        ReadList.Add(entity);
                    }
                }
                data.AddRange(ReadList);
                return true;
            }
            catch (IOException ex) when (IsFileInUse(ex)) // if file is in use we will retry
            {
                Debug.WriteLine("File is in use...");
                return false;
            }
            catch (Exception ex) // catch all other exceptions and fail
            {
                Debug.WriteLine($"An exception was thrown: {ex.Message} whil trying to read the file {FilePath}");
                abort = true;
                return false;
            }
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
        /// Check if string is a terminator string
        /// </summary>
        /// <param name="checking"></param>
        /// <returns></returns>
        protected static bool IsTerminator(string checking)
        {
            return checking.Equals(GenericTerminators.StringTerminator);
        }
    }
}
