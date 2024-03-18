using System.Diagnostics;
using System.Text;

namespace BinaryFileHandler
{
    /// <summary>
    /// Concrete Binary File Reader and Writer that takes IBinaryWritable<typeparamref name="T"/> classes
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BasicBinaryFileReaderWriter<T> : BinaryFileAccessor, IBinaryFileReader<T>, IBinaryFileWriter<T>
        where T : IBinaryWritableReadable<T>, new()
    {
        protected List<T> WritePendingList;
        protected bool LastFailed = false;

        protected readonly List<T> ReadList;
        protected string CurrentClassName;

        public BasicBinaryFileReaderWriter(BinaryFileConfig config) : base(config)
        {
            WritePendingList = new List<T>();

            ReadList = new List<T>();
            CurrentClassName = "";
        }

        public List<T> Data => throw new NotImplementedException();

        public void AddObjectsToWrite(List<T> items)
        {
            throw new NotImplementedException();
        }

        public void AddObjectToWrite(T item)
        {
            throw new NotImplementedException();
        }

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

        public Task<List<T>> ReadValuesAsync()
        {
            throw new NotImplementedException();
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
                        item.WriteObject(writer, item);
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
                LastFailed = true;

                // Change path to temp folder and try again without counting as a retry
                _rootPath = Path.Combine(Path.GetTempPath(), "bin_dat_" + Guid.NewGuid());
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
                        var entity = new T();
                        entity.PopulateThis(reader, CurrentClassName);
                        data.Add(entity);
                    }
                }

                return true;
            }
            catch (IOException ex) when (FileIsInUse(ex)) // if file is in use we will retry
            {
                Debug.WriteLine($"The file is in use: {FilePath}");

                return false;
            }
            catch (Exception ex) // catch all other exceptions and fail
            {
                Debug.WriteLine($"An exception was thrown: {ex.Message} whil trying to read the file {FilePath}");

                abort = true;
                return false;
            }
        }

        public bool WriteValues()
        {
            throw new NotImplementedException();
        }

        public Task WriteValuesAsync()
        {
            throw new NotImplementedException();
        }

        void StashFileData(List<T> fileData)
        {
            ReadList.Clear();
            ReadList.AddRange(fileData);
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
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected void WriteTerminatorString(BinaryWriter writer)
        {
            writer.Write(FileTerminator);
        }

    }
}
