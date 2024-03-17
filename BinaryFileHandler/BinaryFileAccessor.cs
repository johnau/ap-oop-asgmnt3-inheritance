using System.Diagnostics;

namespace BinaryFileHandler
{
    /// <summary>
    /// Base class to control access for Binary File assignment
    /// </summary>
    public abstract class BinaryFileAccessor
    {
        protected const string Delimiter = ";;";
        protected const string Extension = ".bin";
        protected readonly string _filenameBase;
        protected readonly string _rootPath;

        /// <summary>
        /// Access semaphore to prevent concurrent access through the BinaryFileAccessor from implementations
        /// </summary>
        protected SemaphoreSlim accessSemaphore = new SemaphoreSlim(1, 1);

        protected BinaryFileAccessor(BinaryFileConfig config)
        {
            if (config.FileName == null || config.FileName == string.Empty)
                throw new ArgumentNullException("config.FileName");

            _filenameBase = config.FileName;

            if (config.RootPath == null || config.RootPath == string.Empty)
            {
                // Path.GetTempPath();
                _rootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data_files");
                if (!Directory.Exists(_rootPath))
                    Directory.CreateDirectory(_rootPath);

                Debug.WriteLine($"Root Path: {_rootPath}");
            }
            else
            {
                _rootPath = config.RootPath;
            }
        }

        /// <summary>
        /// Generate file path
        /// </summary>
        /// <returns></returns>
        protected string FilePath => Path.Combine(_rootPath, _filenameBase + Extension);

        /// <summary>
        /// Multiple attempts check for file in-case it is just being created
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected static bool FileExists(string path, int checkLimit = 10, int checkIntervalMs = 10)
        {
            while (checkLimit > 0)
            {
                if (File.Exists(path)) return true;
                checkLimit--;
                Thread.Sleep(checkIntervalMs);
                Debug.WriteLine($"Checking again ({checkLimit} left)...");
            }

            return false;
        }

        /// <summary>
        /// Convenience method for FileInUse exception
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        protected static bool IsFileInUse(IOException ex)
        {
            // Check if the specific IOException indicates that the file is in use
            return ex.HResult == -2147024864; // This is the HRESULT for "The process cannot access the file because it is being used by another process."
        }
    }
}
