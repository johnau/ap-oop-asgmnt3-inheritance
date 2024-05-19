namespace BinaryFileHandler
{
    public struct BinaryFileConfig
    {
        public string FileName { get; }
        public string RootPath { get; }

        public BinaryFileConfig(string fileName, string rootPath = "")
        {
            FileName = fileName;
            RootPath = rootPath;
        }
    }
}
