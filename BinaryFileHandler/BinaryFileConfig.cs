namespace BinaryFileHandler
{
    public struct BinaryFileConfig
    {
        public readonly string FileName { get; }
        public readonly string RootPath { get; }
        public BinaryFileConfig(string fileName, string rootPath = "") : this()
        {
            FileName = fileName;
            RootPath = rootPath;
        }
    }
}
