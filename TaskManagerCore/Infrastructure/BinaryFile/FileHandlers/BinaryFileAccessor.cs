using System.Diagnostics;

namespace TaskManagerCore.Infrastructure.BinaryFile.FileHandlers
{
    /// <summary>
    /// Could move the filestream to here, this handles the file throughout the duration of the app lifecycle
    /// Could also accept a dict of types and values which could be used to ensure that every write and read match.
    /// -- Is that level of flexibility ever required though? If you are writing and reading a binary file from the same program,
    /// -- once you have it working you will not change it often.
    /// -- Instead of making a class that extends BinaryFileWriter<T> and Reader<T>, would it be better to make them accept generic data in a dict...
    /// </summary>
    internal abstract class BinaryFileAccessor
    {
        protected const string Delimiter = ";;";
        protected const string _extension = ".bin";
        protected readonly string _filenameBase;
        protected readonly string _rootPath;

        protected SemaphoreSlim accessSemaphore = new SemaphoreSlim(1, 1);

        protected BinaryFileAccessor(string filename = "data", string? rootPath = null)
        {
            if (rootPath == null) _rootPath = Path.GetTempPath();
            else _rootPath = rootPath;

            _filenameBase = filename;
        }

        protected string GenerateFilePath()
        {
            // handle using a new file after current file exceeds certain size
            var filename = _filenameBase + _extension;
            return Path.Combine(_rootPath, filename);
        }

        /// <summary>
        /// Multiple attempts check for file
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
        protected static bool IsFileInUse(IOException ex)
        {
            // Check if the specific IOException indicates that the file is in use
            return ex.HResult == -2147024864; // This is the HRESULT for "The process cannot access the file because it is being used by another process."
        }
    }
}
