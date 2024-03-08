namespace TaskManagerCore.Infrastructure.BinaryFile.Helper
{
    internal struct TaskFolderTerminator
    {
        public const string NameTerminator = "!**##END##**!";
        public static readonly List<string> StringListTerminator = new List<string> { "!**##END##**" };
    }
}
